using ServerWinForm.Data;
using ServerWinForm.Enums;
using ServerWinForm.Extensions;
using ServerWinForm.Services;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ServerWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            HandleBackgroundProcesses();
        }

        private void HandleBackgroundProcesses()
        {
            //serverHandler.connectedDevices.CollectionChanged += ((sender, e) => {
            //    Control.BeginInvoke
            //});
            ServerHandler.connectedDevices.CollectionChanged += UpdateDeviceList;
            ServerHandler.proposalList.CollectionChanged += UpdateProposalList;
            ServerHandler.InitializeServer();
        }

        private async void btnTest_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("TEST HERE");
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            var json = JsonSerializer.Serialize(ServerHandler.proposalList, options);
            var device = ServerHandler.connectedDevices.First();
            try
            {
                await device.NetworkStream.WriteMessageAsync(MessageCode.SEND_PROPOSAL, json);
                var code = await device.NetworkStream.ReadCodeAsync();
                if (code == MessageCode.PROPOSAL_ACCEPTED) { Debug.WriteLine($"({device.TcpClient.Client.RemoteEndPoint} -> Server) Заявка принята"); }
                else Debug.WriteLine($"({device.TcpClient.Client.RemoteEndPoint} -> Server) Заявка отклонена");
            }
            catch (IOException) { Debug.WriteLine($"Устройство с адресом {device.TcpClient.Client.RemoteEndPoint} прекратило соединение"); }
            catch (SocketException) { Debug.WriteLine($"Устройство с адресом {device.TcpClient.Client.RemoteEndPoint} прекратило соединение"); }
            finally { }
        }

        private void UpdateDeviceList(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        List<ListViewItem> itemList = new List<ListViewItem>(e.NewItems.Count);
                        foreach (Device device in e.NewItems)
                        {
                            ListViewItem item = new ListViewItem(device.ClientProfile.deviceName);
                            item.SubItems.Add(device.TcpClient.Client.RemoteEndPoint?.ToString());
                            var deviceType = device.ClientProfile.deviceType.Value;
                            if (deviceType == DeviceType.SCANNER.Value) { item.SubItems.Add("Сканер"); }
                            else if (deviceType == DeviceType.SMART_PHONE.Value) { item.SubItems.Add("Телефон"); }
                            else item.SubItems.Add("Не определено");
                            itemList.Add(item);
                        }
                        if (lst_Connections.InvokeRequired)
                        {
                            lst_Connections.BeginInvoke(new Action(() => lst_Connections.Items.AddRange(itemList.ToArray())));
                        }
                        else lst_Connections.Items.AddRange(itemList.ToArray());
                        if (e.NewItems.Count < 2) { Debug.WriteLine("Добавлен новый элемент в connectionList"); }
                        else Debug.WriteLine("Добавлены новые элементы в connectionList");
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        var index = e.OldStartingIndex;
                        if (lst_Connections.InvokeRequired)
                        {
                            lst_Connections.BeginInvoke(new Action(() => lst_Connections.Items.RemoveAt(index)));
                        }
                        else lst_Connections.Items.RemoveAt(index);
                        Debug.WriteLine("Удалён элемент из connectionList");
                        break;
                    }
            }
        }
        private void UpdateProposalList(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        List<ListViewItem> itemList = new List<ListViewItem>(e.NewItems.Count);
                        foreach (Proposal proposal in e.NewItems)
                        {
                            ListViewItem item = new ListViewItem(proposal.Id.ToString());
                            item.SubItems.Add($"{proposal.DateTime.ToShortDateString()}, {proposal.DateTime.ToShortTimeString()}");
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
                                            device = ServerHandler.connectedDevices.First(item => item.AttachedProposal == proposal);
                                        }
                                        if (device != null) { status = $"Обрабатывается ({device.ClientProfile.deviceName})"; }
                                        else
                                        {
                                            proposal.Status = ProposalStatus.UNPROCESSED;
                                            status = "Не обработано";
                                            Debug.WriteLine("Не удалось найти назначенное устройство. Статус заявки изменён на \"Не обработано\"");
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
                        }
                        if (lst_Proposals.InvokeRequired)
                        {
                            lst_Proposals.BeginInvoke(new Action(() => lst_Proposals.Items.AddRange(itemList.ToArray())));
                        }
                        else lst_Proposals.Items.AddRange(itemList.ToArray());
                        if (e.NewItems.Count < 2) { Debug.WriteLine("Добавлен новый элемент в proposalList"); }
                        else Debug.WriteLine("Добавлены новые элементы в proposalList");
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        var index = e.OldStartingIndex;
                        if (lst_Proposals.InvokeRequired)
                        {
                            lst_Proposals.BeginInvoke(new Action(() => lst_Proposals.Items.RemoveAt(index)));
                        }
                        else lst_Proposals.Items.RemoveAt(index);
                        Debug.WriteLine("Удалён элемент из proposalList");
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        var index = e.OldStartingIndex;
                        string status = "";
                        switch (ServerHandler.proposalList.ElementAt(index).Status)
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
                                        device = ServerHandler.connectedDevices.First(item => item.AttachedProposal == e.OldItems[0] as Proposal);
                                    }
                                    status = $"Обрабатывается ({device.ClientProfile.deviceName})";
                                    break;
                                }
                            case ProposalStatus.PROCESSED:
                                {
                                    status = "Выполнено";
                                    break;
                                }
                        }
                        if (lst_Proposals.InvokeRequired)
                        {
                            lst_Proposals.BeginInvoke(new Action(() =>
                            {
                                lst_Proposals.Items[index].SubItems[2].Text = status;
                                Debug.WriteLine($"Изменено поле статуса у объекта в proposalList ({lst_Proposals.Items[index].SubItems[0].Text})");
                            }));
                        }
                        else
                        {
                            lst_Proposals.Items[index].SubItems[2].Text = status;
                            Debug.WriteLine($"Изменено поле статуса у объекта в proposalList ({lst_Proposals.Items[index].SubItems[0].Text})");
                        }
                        break;
                    }
            }
        }

        private void ShowProposalDetailsDialog()
        {
            ProposalDetailsForm proposalDetailsForm = new ProposalDetailsForm();
            string propId = lst_Proposals.SelectedItems[0].Text;
            var proposal = ServerHandler.proposalList.First(item => item.Id.Equals(Guid.Parse(propId)));
            proposalDetailsForm.SelectedProposal = proposal;
            var result = proposalDetailsForm.ShowDialog();
        }

        private void lst_Proposals_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Item double-clicked by mouse");
            ShowProposalDetailsDialog();
        }

        private void lst_Proposals_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                Debug.WriteLine("Item cliked by Enter button");
                ShowProposalDetailsDialog();
            }
        }

        private void lst_Connections_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var device = lst_Connections.SelectedItems[0].Text;
            var prop = ServerHandler.connectedDevices.First(item => item.ClientProfile.deviceName.Equals(device)).AttachedProposal;
            Debug.WriteLine($"Attached prop = {prop} ({prop?.Id})");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ServerHandler.SaveData();
        }
    }
}
