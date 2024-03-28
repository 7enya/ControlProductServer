using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerWinForm.Enums;

namespace ServerWinForm.Data
{
    public class Device
    {
        public TcpClient TcpClient { get; set; }
        public ClientProfile ClientProfile { get; private set; }
        public NetworkStream NetworkStream { get; set; }
        public Proposal? AttachedProposal { get; set; }

        public Device(TcpClient client, ClientProfile profile)
        {
            TcpClient = client;
            ClientProfile = profile;
            NetworkStream = client.GetStream();
            AttachedProposal = null;
        }

        //public byte[]? GetMessage()
        //{
        //    byte[] message = new byte[256];
        //    int readBytes = networkStream.Read(message, 0, message.Length);
        //    if (readBytes == 0) return null;
        //    return message;
        //    tcpClient.Client.R
        //}

        public bool isConnected()
        {
            return (TcpClient.Client.Poll(1, SelectMode.SelectRead) && !NetworkStream.DataAvailable) ? false : true;
        }
    }
}
