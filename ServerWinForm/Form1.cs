using ServerWinForm.Data;
using ServerWinForm.Enums;
using ServerWinForm.Services;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

namespace ServerWinForm
{
    public partial class Form1 : Form
    {

        ServerHandler serverHandler;
        public Form1()
        {
            InitializeComponent();
            HandleBackgroundProcesses();
        }

        private void HandleBackgroundProcesses()
        {
            serverHandler = new ServerHandler();
            //serverHandler.connectedDevices.CollectionChanged += ((sender, e) => {
            //    Control.BeginInvoke
            //});
            serverHandler.connectedDevices.CollectionChanged += UpdateDeviceList;
            serverHandler.proposalList.CollectionChanged += UpdateProposalList;
            serverHandler.InitializeServer();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {

        }

        private void UpdateDeviceList(object? sender, NotifyCollectionChangedEventArgs e)
        {

            List<ListViewItem> itemList = [];
            IList? connectionList = e.NewItems;
            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    connectionList = e.NewItems;
            //}
            //else if (e.Action == NotifyCollectionChangedAction.Remove)
            //{
            //    connectionList = e.OldItems;
            //}
            if (connectionList == null)
            {
                if (lstConnections.InvokeRequired)
                {
                    lstConnections.BeginInvoke(new Action(lstConnections.Items.Clear));
                }
                else lstConnections.Items.Clear();
                return;
            }

            foreach (Device device in connectionList)
            {
                ListViewItem item = new ListViewItem(device.ClientProfile.deviceName);
                item.SubItems.Add(device.TcpClient.Client.RemoteEndPoint?.ToString());
                var deviceType = device.ClientProfile.deviceType.Value;
                if (deviceType == DeviceType.SCANNER.Value) { item.SubItems.Add("Сканер"); }
                else if (deviceType == DeviceType.SMART_PHONE.Value) { item.SubItems.Add("Телефон"); }
                else item.SubItems.Add("Не определено");
                itemList.Add(item);
                Debug.WriteLine("Добавлен новый элемент в connectionList");
            }
            if (lstConnections.InvokeRequired)
            {
                lstConnections.BeginInvoke(new Action(() =>
                {
                    lstConnections.Items.Clear();
                    lstConnections.Items.AddRange(itemList.ToArray());
                }));
            }
            else
            {
                lstConnections.Items.Clear();
                lstConnections.Items.AddRange(itemList.ToArray());
            }
        }
        private void UpdateProposalList(object? sender, NotifyCollectionChangedEventArgs e)
        {
            List<ListViewItem> itemList = [];
            IList? proposalList = e.NewItems;

            if (proposalList == null)
            {
                if (lstProposals.InvokeRequired)
                {
                    lstProposals.BeginInvoke(new Action(lstProposals.Items.Clear));
                }
                else lstProposals.Items.Clear();
                return;
            }

            foreach (Proposal proposal in proposalList)
            {
                ListViewItem item = new ListViewItem(proposal.Id.ToString());
                item.SubItems.Add(proposal.DataTime.ToShortDateString());
                string status = "";
                switch (proposal.Status)
                {
                    case ProposalStatus.UNPROCESSED:
                        {
                            status = "Не обработано";
                            break;
                        }
                    case ProposalStatus.IN_PROCESS:
                        {
                            var _lock = new Object();
                            Device device;
                            lock (_lock)
                            {
                                device = serverHandler.connectedDevices.First(item => item.AttachedProposal == proposal);
                            }
                            if (device != null)
                            {
                                status = $"Обрабатывается ({device.ClientProfile.deviceName})";
                            }
                            else
                            {
                                proposal.Status = ProposalStatus.UNPROCESSED;
                                status = "Не обработано";
                                Debug.WriteLine($"Не найдено закреплённое устройство. Статус заявки {proposal.Id} изменён на \"Не обработано\"");
                            }
                            break;
                        }
                    case ProposalStatus.PROCESSED:
                        {
                            status = "Выполнено";
                            break;
                        }
                }
                item.SubItems.Add(status);
                itemList.Add(item);
                Debug.WriteLine("Добавлен новый элемент в proposalList");
            }
            if (lstProposals.InvokeRequired)
            {
                lstProposals.BeginInvoke(new Action(() =>
                {
                    lstProposals.Items.Clear();
                    lstProposals.Items.AddRange(itemList.ToArray());
                }));
            }
            else
            {
                lstProposals.Items.Clear();
                lstProposals.Items.AddRange(itemList.ToArray());
            }
        }

        private void lstProposals_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Item double-clicked by mouse");
        }

        private void lstProposals_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                Debug.WriteLine("Item cliked by Enter button");
            }
        }
    }
}
