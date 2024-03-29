using ServerWinForm.Data;
using ServerWinForm.Extensions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using OpCode = ServerWinForm.Enums.OpCode;

namespace ServerWinForm.Services
{
    public static class ServerHandler
    {
        //public event EventHandler<string> onChanged;
        public static object _lockDeviceObject = new object();
        private static object _lockAuthProcessObject = new object();
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
                ClientProfile? userProfile;
                lock (_lockAuthProcessObject)
                {
                    userProfile = getUserProfileByAuthDataAsync(tcpClient).Result;
                    if (userProfile == null)
                    {
                        Debug.WriteLine($"Устройство {tcpClient.Client.RemoteEndPoint} не прошло процесс авторизации, соединение прервано");
                        tcpClient.Close();
                        return;
                    }
                    Debug.WriteLine($"Устройство {userProfile.deviceName} ({tcpClient.Client.RemoteEndPoint}) подключено");
                    device = new Device(tcpClient, userProfile);
                    connectedDevices.Add(device);
                }
                // приём сообщений от клиента
                byte[] data = new byte[500];
                int readBytes;
                string message;
                while (device.isConnected())
                {
                    readBytes = await tcpClient.GetStream().ReadAsync(data, 0, data.Length);
                    if (readBytes == 0) continue;
                    message = Encoding.UTF8.GetString(data, 1, readBytes - 1);
                    Debug.WriteLine(
                        $"({device.ClientProfile.deviceName}) сообщение: \"{message}\", длина: {message.Length}"
                    );
                }
                Debug.WriteLine($"Устройство с адресом {device.TcpClient.Client.RemoteEndPoint} прекратило соединение");
            }
            catch (IOException)
            {
                Debug.WriteLine($"Устройство с адресом {tcpClient.Client.RemoteEndPoint} прекратило соединение");
            }
            catch (SocketException)
            {
                Debug.WriteLine($"Устройство с адресом {tcpClient.Client.RemoteEndPoint} отключено");
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
                        proposal.Status = Enums.ProposalStatus.UNPROCESSED;
                        int index = proposalList.IndexOf(proposal);
                        proposalList[index] = proposal;
                    }
                    Debug.WriteLine($"Удалено устройство {device.ClientProfile.deviceName} из списка (Size: {connectedDevices.Count})");
                }
            }
            //catch (AggregateException taskExc)
            //{
            //    foreach (var ex in taskExc.InnerExceptions)
            //    {
            //        if (ex is SocketException)
            //        {

            //        }
            //        if (ex is IOException)
            //        {

            //        }
            //    }
            //}
            //catch (TaskCanceledException) { }
        }


        private static async Task<ClientProfile?> getUserProfileByAuthDataAsync(TcpClient client)
        {
            Debug.WriteLine($"Начат процесс авторизации устройства {client.Client.RemoteEndPoint}");
            var streamReader = new StreamReader(client.GetStream());
            TimeSpan timeOut = TimeSpan.FromSeconds(10);
            var timeoutTask = Task.Delay(timeOut);
            byte[] inputMessage = new byte[500];
            var readMessageTask = client.GetStream().ReadAsync(inputMessage, 0, inputMessage.Length);
            var completedTask = await Task.WhenAny(timeoutTask, readMessageTask);
            if (completedTask == timeoutTask)
            {
                Debug.WriteLine($"(Клиент: {client.Client.RemoteEndPoint}) Превышено время ожидания ({timeOut.Seconds} сек.)");
                return null;
            }
            //int end = Array.IndexOf(inputMessage, (byte)0);
            int readBytes = readMessageTask.Result;
            if (readBytes == 0)
                return null;

            Debug.WriteLine($"OpCode = {inputMessage[0]}");
            string authData = Encoding.UTF8.GetString(inputMessage, 1, readBytes - 1);
            Debug.WriteLine($"Сообщение от {client.Client.RemoteEndPoint}: \"{authData}\"");
            if (inputMessage[0] != (byte)OpCode.START_AUTH)
                return null;

            ClientProfile? userProfile = null;
            if (authData.Contains("login=", StringComparison.InvariantCultureIgnoreCase) &&
                authData.Contains("password=", StringComparison.InvariantCultureIgnoreCase))
            {
                // login=klhjkh=password=165484
                var splitedMes = authData.Trim().Split('=');
                var config = new ConfigService();
                userProfile = config.GetProfiles().Find(
                    (elem) => (elem.login?.Equals(splitedMes[1]) ?? false) && (elem.password?.Equals(splitedMes[3]) ?? false)
                );
            }
            else if (authData.Contains("macAddress=", StringComparison.InvariantCultureIgnoreCase))
            {
                // macAddr=2F-19-15-24
                var splitedMes = authData.Trim().Split('=');
                var config = new ConfigService();
                userProfile = config.GetProfiles().Find(
                    (elem) => elem.deviceMacAddress?.Equals(splitedMes[1]) ?? false
                );
            }
            bool profileIsBusy = connectedDevices.Any(device => device.ClientProfile.deviceName == userProfile?.deviceName);
            if (profileIsBusy)
            {
                Debug.WriteLine($"({client.Client.RemoteEndPoint}) Профиль устройства \"{userProfile?.deviceName}\" занят другим пользователем");
                return null;
            }
            return userProfile;
        }

        private static async Task StartListenForServerProposals()
        {
            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
            var proposalCache = @$"{projectDirectory}\cache\Proposals.txt";
            if (!File.Exists(proposalCache))
            {
                File.Create(proposalCache).Close();
                Debug.WriteLine("Создан кэш-файл Proposal.txt");
            }
            if (!new FileInfo(proposalCache).IsEmpty())
            {
                var cache = JsonFileReader.ReadFile<List<Proposal>>(proposalCache);
                if (cache != null) { cache.ForEach(proposalList.Add); }
                else Debug.WriteLine("Не удалось прочитать содержимое файла Proposal.txt");
            }
            await Task.Delay(3000);
            var productList = JsonFileReader.ReadFile<List<Product>>(@$"{projectDirectory}\Request.json");
            proposalList.Add(new Proposal(productList!));
            await Task.Delay(5000);
            proposalList.Add(new Proposal(productList!));
            
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

        public static void SaveData()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            var cache = JsonSerializer.Serialize(proposalList, options);
            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
            var proposalCache = @$"{projectDirectory}\cache\Proposals.txt";
            File.WriteAllLines(proposalCache, [cache]);
            Debug.WriteLine("Данные сохранены");
        }

    }
}
