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
        public delegate Task Job(Device device);
        public Job? job { private get; set; }

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


        public bool isConnected()
        {
            //return (TcpClient.Client.Poll(2000, SelectMode.SelectRead) && !NetworkStream.DataAvailable) ? false : true;
            return !((TcpClient.Client.Poll(1000, SelectMode.SelectRead) && (TcpClient.Client.Available == 0)) || !TcpClient.Client.Connected);
        }

        public async Task DoJobIfThereIs()
        {
            if (job != null) await job(this);
        }
    }
}
