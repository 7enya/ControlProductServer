using ServerWinForm.Data;
using ServerWinForm.Enums;
using ServerWinForm.Extensions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using MessageCode = ServerWinForm.Enums.MessageCode;

namespace ServerWinForm.Services
{
    public static class ServerHandler
    {
        public static object _lockDeviceObject = new object();
        private static Semaphore authSem = new Semaphore(1, 1);
        private static TcpListener server;
        public static ObservableCollection<Device> connectedDevices { get; private set; } = new ObservableCollection<Device>();
        public static ObservableCollection<Proposal> proposalList { get; private set; } = new ObservableCollection<Proposal>();


        public static async Task InitializeServer()
        {
            //var ipAddress = GetIPv4Address(NetworkInterfaceType.Wireless80211) ?? GetIPv4Address(NetworkInterfaceType.Ethernet);
            var ipAddress = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(ipAddress!, 11000);
            server.Start();
            Debug.WriteLine($"Сервер прослушивает подключения на {server.LocalEndpoint}");
            Task.Run(async () => StartListenForServerProposals());
            while (true)
            {
                if (server.Pending())
                {
                    var client = await server.AcceptTcpClientAsync();
                    Task.Run(async () => HandleIncomingConnection(client));
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }
        private static async Task HandleIncomingConnection(TcpClient tcpClient)
        {
            Debug.WriteLine($"Подключение устройства с адресом {tcpClient.Client.RemoteEndPoint} ...");
            Device? device = null;
            try
            {
                authSem.WaitOne();
                device = await getAuthorizedDeviceAsync(tcpClient);
                if (device == null)
                {
                    await tcpClient.GetStream().WriteMessageAsync(MessageCode.ACCESS_DENIED, null);
                    Debug.WriteLine($"Устройство {tcpClient.Client.RemoteEndPoint} не прошло процесс авторизации, соединение прервано");
                    tcpClient.Close();
                    authSem.Release();
                    return;
                }
                Debug.WriteLine($"Устройство {device.ClientProfile.deviceName} ({tcpClient.Client.RemoteEndPoint}) подключено");
                connectedDevices.Add(device);
                authSem.Release();
                await device.NetworkStream.WriteMessageAsync(MessageCode.ACCESS_GRANTED, null);
                while (device.isConnected()) {
                    await device.DoJobIfThereIs(); 
                }
                Debug.WriteLine($"(GOOD) Устройство с адресом {device.TcpClient.Client.RemoteEndPoint} прекратило соединение");
            }
            catch (IOException)
            {
                Debug.WriteLine($"(BAD) Потеряно соединение с устройством {tcpClient.Client.RemoteEndPoint}");
            }

            catch (SocketException)
            {
                Debug.WriteLine($"(BAD) Устройство с адресом {tcpClient.Client.RemoteEndPoint} отключено");
            }
            catch (FormatException)
            {
                Debug.WriteLine($"({tcpClient.Client.RemoteEndPoint}) Получено сообщение с неверным форматом данных");
            }
            finally
            {
                if (device != null)
                {
                    lock (_lockDeviceObject) { connectedDevices.Remove(device); }
                    if (device.AttachedProposal != null)
                    {
                        Debug.WriteLine($"Отвязка заявки от устройства {device.ClientProfile.deviceName}");
                        var proposal = proposalList.First(proposal => proposal == device.AttachedProposal);
                        proposal.Status = ProposalStatus.UNPROCESSED;
                        int index = proposalList.IndexOf(proposal);
                        proposalList[index] = proposal;
                    }
                    Debug.WriteLine($"Удалено устройство {device.ClientProfile.deviceName} из списка (Size: {connectedDevices.Count})");
                }
            }
        }


        private static async Task<Device?> getAuthorizedDeviceAsync(TcpClient tcpClient)
        {
            Debug.WriteLine($"Начат процесс авторизации устройства {tcpClient.Client.RemoteEndPoint}");
            TimeSpan timeOut = TimeSpan.FromSeconds(10);
            var timeoutTask = Task.Delay(timeOut);
            var readCodeTask = tcpClient.GetStream().ReadCodeAsync();
            var completedTask = await Task.WhenAny(timeoutTask, readCodeTask);
            if (completedTask == timeoutTask)
            {
                Debug.WriteLine($"(Сервер <- {tcpClient.Client.RemoteEndPoint}) Превышено время ожидания ({timeOut.Seconds} сек.)");
                return null;
            }
            var messageCode = readCodeTask.Result;
            if (messageCode != MessageCode.START_AUTH && messageCode != MessageCode.RECONNECTION)
                return null;
            string authData = Encoding.UTF8.GetString(await tcpClient.GetStream().ReadMessageAsync());
            ClientProfile? userProfile = null;
            string proposalId = string.Empty;
            if (authData.Contains("login=", StringComparison.InvariantCultureIgnoreCase) &&
                authData.Contains("password=", StringComparison.InvariantCultureIgnoreCase))
            {
                // login=klhjkh=password=165484
                var splitedMes = authData.Trim().Split('=');
                var config = new ConfigService();
                userProfile = config.GetProfiles().Find(
                    (elem) => (elem.login?.Equals(splitedMes[1]) ?? false) && (elem.password?.Equals(splitedMes[3]) ?? false)
                );
                // login=klhjkh=password=165484=propId=465421654564
                if (messageCode == MessageCode.RECONNECTION)
                {
                    proposalId = splitedMes[5];
                }
            }
            else if (authData.Contains("macAddress=", StringComparison.InvariantCultureIgnoreCase))
            {
                // macAddress=2F-19-15-24
                var splitedMes = authData.Trim().Split('=');
                var config = new ConfigService();
                userProfile = config.GetProfiles().Find(
                    (elem) => elem.deviceMacAddress?.Equals(splitedMes[1]) ?? false
                );
                // macAddress=2F-19-15-24=propId=465421654564
                if (messageCode == MessageCode.RECONNECTION)
                {
                    proposalId = splitedMes[3];
                }
            }
            if (userProfile == null) return null;
            bool profileIsBusy = connectedDevices.Any(device => userProfile?.deviceName == device.ClientProfile.deviceName);
            if (profileIsBusy)
            {
                Debug.WriteLine($"({tcpClient.Client.RemoteEndPoint}) Профиль устройства \"{userProfile?.deviceName}\" занят другим пользователем");
                return null;
            }
            var device = new Device(tcpClient, userProfile);
            if (proposalId != string.Empty)
            {
                var proposal = proposalList.FirstOrDefault(prop => prop!.Id == proposalId, null);
                if (proposal != null) 
                {
                    device.AttachedProposal = proposal;
                    device.job = StartWaitingForResultOfProposalProcessing;
                    proposalList.Insert(proposalList.IndexOf(proposal), proposal);
                }
            }
            return device;
        }

        private static async Task StartListenForServerProposals()
        {
            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
            //var proposalCache = @$"{projectDirectory}\cache\Proposals.txt";
            //if (!File.Exists(proposalCache))
            //{
            //    File.Create(proposalCache).Close();
            //    Debug.WriteLine("Создан кэш-файл Proposal.txt");
            //}
            //if (!new FileInfo(proposalCache).IsEmpty())
            //{
            //    var cache = JsonFileReader.ReadFile<List<Proposal>>(proposalCache);
            //    if (cache != null) { cache.ForEach(proposalList.Add); }
            //    else Debug.WriteLine("Не удалось прочитать содержимое файла Proposal.txt");
            //}
            //await Task.Delay(3000);

            var proposalsFromServer = JsonFileReader.ReadFile<List<Proposal>>(@$"{projectDirectory}\Request.json");

            foreach (Proposal prop in proposalsFromServer)
            {
                proposalList.Add(prop);
            }


            //Чтение заявки из http-запроса

            //HttpClient httpClient = new HttpClient();
            //using var response = await httpClient.GetAsync("https://localhost:7094/1");
            //var stream = await response.Content.ReadAsStreamAsync();
            //var productList = JsonSerializer.Deserialize<List<Product>>(stream);
            //proposalList.Add(new Proposal(productList));

            //if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound)
            //{
            //    // получаем информацию об ошибке
            //    Error? error = await response.Content.ReadFromJsonAsync<Error>();
            //    Console.WriteLine(response.StatusCode);
            //    Console.WriteLine(error?.Message);
            //}
            //else
            //{
            //    response.Dispose
            //    // если запрос завершился успешно, получаем объект Person
            //    Proposal person = await response.Content.ReadFromJsonAsync<Proposal>();
            //    Console.WriteLine($"Name: {person?.Name}   Age: {person?.Age}");
            //}
        }


        public static async Task<bool> AttachProposalTo(Device device, Proposal proposal)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            var jsonfile = JsonSerializer.Serialize(proposal, options);
            try
            {
                Debug.WriteLine($"(Server -> {device.TcpClient.Client.RemoteEndPoint}) Началась отправка заявки");
                var isSent = await device.NetworkStream.WriteMessageAsync(MessageCode.SEND_PROPOSAL, jsonfile);
                Debug.WriteLine($"(Server -> {device.TcpClient.Client.RemoteEndPoint}) Заявка отправлена");
                var responseCode = await device.NetworkStream.ReadCodeAsync();
                if (responseCode == MessageCode.PROPOSAL_ACCEPTED)
                {
                    await device.NetworkStream.ClearStreamAsync();
                    device.AttachedProposal = proposal;
                    proposal.Status = ProposalStatus.IN_PROCESS;
                    int index = proposalList.IndexOf(proposal);
                    proposalList[index] = proposal;
                    device.job = StartWaitingForResultOfProposalProcessing;
                    return true;
                }
                else
                {
                    await device.NetworkStream.ClearStreamAsync();
                    return false;
                }
            }
            catch (IOException) { return false; }
            catch (SocketException) { return false; }
        }

        private static async Task StartWaitingForResultOfProposalProcessing(Device device)
        {
            switch(await device.NetworkStream.ReadCodeAsync())
            {
                case MessageCode.JOB_DONE:
                    {
                        // Отправить результат на сервер 1С
                        // Отправить в базу данных json-файл с отмеченными продуктами

                        var proposal = device.AttachedProposal;
                        device.AttachedProposal = null;
                        proposal!.Status = ProposalStatus.PROCESSED;
                        var index = proposalList.IndexOf(proposal);
                        proposalList[index] = proposal;

                        break;
                    }
                case MessageCode.PROPOSAL_REJECTED:
                    {
                        var proposal = device.AttachedProposal;
                        device.AttachedProposal = null;
                        proposal!.Status = ProposalStatus.UNPROCESSED;
                        var index = proposalList.IndexOf(proposal);
                        proposalList[index] = proposal;

                        break;
                    }
            }
            await device.NetworkStream.ClearStreamAsync();
        }


        private static IPAddress? GetIPv4Address(NetworkInterfaceType type)
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }
            return null;
        }

        //public static void SaveData()
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        //    };
        //    var cache = JsonSerializer.Serialize(proposalList, options);
        //    var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        //    var proposalCache = @$"{projectDirectory}\cache\Proposals.txt";
        //    File.WriteAllLines(proposalCache, [cache]);
        //    Debug.WriteLine("Данные сохранены");
        //}

    }
}
