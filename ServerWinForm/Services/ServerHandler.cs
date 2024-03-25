using ServerWinForm.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using OpCode = ServerWinForm.Enums.OpCode;

namespace ServerWinForm.Services
{
    public static class ServerHandler
    {
        //public event EventHandler<string> onChanged;
        private static Object _lockObject = new Object();
        private static TcpListener server;
        public static ObservableCollection<Device> connectedDevices { get; private set; } = new ObservableCollection<Device>();
        public static ObservableCollection<Proposal> proposalList { get; private set; } = new ObservableCollection<Proposal>();


        public static async Task InitializeServer()
        {
            var ipAddress = GetIPv4Address(NetworkInterfaceType.Wireless80211) ?? GetIPv4Address(NetworkInterfaceType.Ethernet);
            //var ipAddress = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(ipAddress!, 11000); // port 11_000 (LAB)
            server.Start();
            Debug.WriteLine($"Сервер прослушивает подключения на {server.LocalEndpoint}");
            while (true)
            {
                if (server.Pending())
                {
                    var client = await server.AcceptTcpClientAsync();
                    Task.Run(async () => await HandleIncomingConnection(client)).ConfigureAwait(false);
                    //await HandleIncomingConnection(await server.AcceptTcpClientAsync());
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
                var userProfile = await getUserProfileByAuthDataAsync(tcpClient);
                if (userProfile == null)
                {
                    Debug.WriteLine($"Устройство {tcpClient.Client.RemoteEndPoint} не прошло процесс авторизации, соединение прервано");
                    return;
                }
                Debug.WriteLine($"Устройство {userProfile.deviceName} ({tcpClient.Client.RemoteEndPoint}) подключено");
                device = new Device(tcpClient, userProfile);
                lock (_lockObject)
                {
                    connectedDevices.Add(device);
                }
                // чтение тестовой заявки из json-файла
                var productList = JsonFileReader.ReadFile<List<Product>>(@"C:\Users\PC-SIPR-08\source\repos\ServerWinForm\ServerWinForm\Request.json");
                proposalList.Add(new Proposal(productList));

                byte[] data = new byte[500];
                int readBytes;
                string message;
                while (device.isConnected())
                {
                    //token.ThrowIfCancellationRequested();
                    readBytes = tcpClient.GetStream().Read(data, 0, data.Length);
                    if (readBytes == 0) continue;
                    message = Encoding.UTF8.GetString(data, 1, readBytes - 1);
                    Debug.WriteLine(
                        $"client message: \"{message}\", length: {message.Length}"
                    );
                    var prop = proposalList.First();
                    device.AttachedProposal = prop;
                    prop.Status = Enums.ProposalStatus.IN_PROCESS;
                    proposalList.Insert(proposalList.IndexOf(prop), prop);
                }
                // отключение устройства
                Debug.WriteLine($"Устройство с адресом {device.TcpClient.Client.RemoteEndPoint} прекратило соединение");
                lock (_lockObject)
                {
                    connectedDevices.Remove(device);
                }
                if (device.AttachedProposal != null)
                {
                    var proposal = proposalList.First(item => item == device.AttachedProposal);
                    proposal.Status = Enums.ProposalStatus.UNPROCESSED;
                    proposalList.Insert(proposalList.IndexOf(proposal), proposal);
                }
                Debug.WriteLine($"Удалено устройство {device.ClientProfile.deviceName} из списка (Size: {connectedDevices.Count})");
                return;
            }
            catch (SocketException)
            {
                Debug.WriteLine($"Устройство с адресом {tcpClient.Client.RemoteEndPoint} отключено");
                if (device != null)
                {
                    lock (_lockObject) { connectedDevices.Remove(device); }
                    Debug.WriteLine($"Удалено устройство {device.ClientProfile.deviceName} из списка (Size: {connectedDevices.Count})");
                }
            }
            catch (IOException)
            {
                Debug.WriteLine($"Устройство с адресом {tcpClient.Client.RemoteEndPoint} прекратило соединение");
                if (device != null)
                {
                    lock (_lockObject) { connectedDevices.Remove(device); }
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
            var streamReader = new StreamReader(client.GetStream());
            TimeSpan timeOut = TimeSpan.FromSeconds(10);
            var timeoutTask = Task.Delay(timeOut);
            byte[] inputMessage = new byte[500];
            var readMessageTask = client.GetStream().ReadAsync(inputMessage, 0, inputMessage.Length);
            var completedTask = await Task.WhenAny(timeoutTask, readMessageTask);
            if (completedTask == timeoutTask)
            {
                Debug.WriteLine($"Превышено время ожидания ({timeOut.Seconds} сек.)");
                return null;
            }
            //int end = Array.IndexOf(inputMessage, (byte)0);
            int readBytes = readMessageTask.Result;
            if (readBytes == 0)
                return null;

            Debug.WriteLine($"OpCode = {inputMessage[0]}");
            string authData = Encoding.UTF8.GetString(inputMessage, 1, readBytes - 1);
            Debug.WriteLine($"Сообщение от {client.Client.RemoteEndPoint}: \"{authData}\"");
            if (inputMessage[0] == 0 || inputMessage[0] != (byte)OpCode.START_AUTH)
                return null;

            if (authData.Contains("login=", StringComparison.InvariantCultureIgnoreCase) &&
                authData.Contains("password=", StringComparison.InvariantCultureIgnoreCase))
            {
                // login=klhjkh=password=165484
                var splitedMes = authData.Trim().Split('=');
                var config = new ConfigService();
                var userProfile = config.GetProfiles().Find(
                    (elem) => elem.login?.Equals(splitedMes[1]) ?? false
                );
                if (userProfile == null || (userProfile.password?.Equals(splitedMes[3]) ?? false))
                    return null;
                return userProfile;
            }
            if (authData.Contains("macAddress=", StringComparison.InvariantCultureIgnoreCase))
            {
                // macAddr=2F-19-15-24
                var splitedMes = authData.Trim().Split('=');
                var config = new ConfigService();
                var userProfile = config.GetProfiles().Find(
                    (elem) => elem.deviceMacAddress?.Equals(splitedMes[1]) ?? false
                );
                return userProfile;
            }
            return null;
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

    }
}
