using ServerWinForm.Data;
using ServerWinForm.Properties;
using ServerWinForm.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerWinForm
{
    public partial class ProposalDetailsForm : Form
    {
        public Proposal SelectedProposal { get; set; }
        public string ProposalStatus { get; set; }
        public string SelectedDeviceName { get; private set; }

        public ProposalDetailsForm()
        {
            InitializeComponent();
            if (SelectedProposal.Status == Enums.ProposalStatus.UNPROCESSED)
            {
                btn_uploadProposal.Visible = true;
                btn_uploadProposal.Enabled = true;
                cb_SelectDevice.Enabled = true;
            }
            else cb_SelectDevice.Enabled = false;
            txt_ProposalStatus.Text = ProposalStatus;
            im_UploadStatus.Visible = false;
            cb_SelectDevice.Items.AddRange(
                ServerHandler.connectedDevices
                .Where(device => device.AttachedProposal == null && device.ClientProfile.deviceName != null)
                .Select(device => device.ClientProfile.deviceName!)
                .ToArray()
            );
            //im_UploadStatus.Image = (Image).GetObject("im_UploadStatus.Image");

        }

        private async void btn_uploadProposal_Click(object sender, EventArgs e)
        {
            btn_uploadProposal.Enabled = false;
            btn_uploadProposal.Visible = false;
            im_UploadStatus.Visible = true;
            //im_UploadStatus.Image = Resources.gif_uploadProcess;
            await Task.Delay(3000);
            im_UploadStatus.Image = Resources.im_uploadFinished;
            txt_ProposalStatus.Text = "Обрабатывается";
            SelectedDeviceName = cb_SelectDevice.
            cb_SelectDevice.Enabled = false;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
