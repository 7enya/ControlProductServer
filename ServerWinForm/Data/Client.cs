using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerWinForm.Enums;

namespace ServerWinForm.Data
{
    public class Client
    {
        private TcpClient tcpClient { get; set; }
        private ClientProfile clientProfile { get; set; }
        private StreamReader streamReader { get; set; }
        private StreamWriter streamWriter { get; set; }

        public Client(TcpClient client, ClientProfile profile)
        {
            tcpClient = client;
            clientProfile = profile;
            streamReader = new StreamReader(client.GetStream());
            streamWriter = new StreamWriter(client.GetStream());
        }

        public TcpClient GetConnection()
        {
            return tcpClient;
        }

        public StreamReader GetReadStream()
        {
            return streamReader;
        }

        public ClientProfile GetProfile()
        {
            return clientProfile;
        }
    }
}
