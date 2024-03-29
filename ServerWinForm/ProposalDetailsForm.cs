using ServerWinForm.Data;
using ServerWinForm.Properties;
using ServerWinForm.Services;
using System.Data;
using System.Diagnostics;

namespace ServerWinForm
{
    public partial class ProposalDetailsForm : Form
    {
        public Proposal SelectedProposal { get; set; }
        public string? SelectedDeviceName { get; private set; }

        public ProposalDetailsForm()
        {
            InitializeComponent();
        }

        private void ProposalDetailsForm_Load(object sender, EventArgs e)
        {
            if (SelectedProposal.Status == Enums.ProposalStatus.UNPROCESSED)
            {
                btn_uploadProposal.Visible = true;
                btn_uploadProposal.Enabled = true;
                cb_SelectDevice.Enabled = true;
            }
            else cb_SelectDevice.Enabled = false;
            switch (SelectedProposal.Status)
            {
                case Enums.ProposalStatus.UNPROCESSED:
                    {
                        txt_ProposalStatus.Text = "Не обработано";
                        cb_SelectDevice.Items.AddRange(
                            ServerHandler.connectedDevices
                            .Where(device => device.AttachedProposal == null && device.ClientProfile.deviceName != null)
                            .Select(device => device.ClientProfile.deviceName!)
                            .ToArray()
                        );
                        break;
                    }
                case Enums.ProposalStatus.IN_PROCESS:
                    {
                        txt_ProposalStatus.Text = "Обрабатывается";
                        cb_SelectDevice.Items.Add(
                            ServerHandler.connectedDevices
                            .First(device => device.AttachedProposal == SelectedProposal)
                            .ClientProfile.deviceName!
                        );
                        break;
                    }
                case Enums.ProposalStatus.PROCESSED:
                    {
                        txt_ProposalStatus.Text = "Выполнено";
                        break;
                    }
            }
            im_UploadStatus.Visible = false;

            List<ListViewItem> items = new List<ListViewItem>(SelectedProposal.Products.Count);
            SelectedProposal.Products.ForEach(product =>
            {
                if (product == null) return;
                var item = new ListViewItem(product.title);
                item.SubItems.Add(product.count.ToString());
                items.Add(item);
            });
            lst_Products.Items.AddRange(items.ToArray());
            if (cb_SelectDevice.Items.Count != 0) cb_SelectDevice.SelectedIndex = 0;
        }

        private async void btn_uploadProposal_Click(object sender, EventArgs e)
        {
            SelectedDeviceName = (string?)cb_SelectDevice.SelectedItem;
            Debug.WriteLine("Selected item = " + SelectedDeviceName);
            if (string.IsNullOrEmpty(SelectedDeviceName))
                return;
            btn_uploadProposal.Enabled = false;
            btn_uploadProposal.Visible = false;
            im_UploadStatus.Visible = true;
            im_UploadStatus.Image = Resources.gif_uploadProcess;
            await Task.Delay(3000);
            im_UploadStatus.Image = Resources.im_uploadFinished;
            txt_ProposalStatus.Text = "Обрабатывается";
            cb_SelectDevice.Enabled = false;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (SelectedDeviceName != null)
            {
                DialogResult = DialogResult.OK;
            }
            else DialogResult = DialogResult.Cancel;
            Debug.WriteLine($"Dialog Status = {DialogResult}");
            Close();
        }

    }
}
