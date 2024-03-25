namespace ServerWinForm
{
    partial class ProposalDetailsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "Сметана 20л", "20" }, -1);
            ListViewItem listViewItem2 = new ListViewItem(new string[] { "Йогурт 0,5", "10" }, -1);
            listView1 = new ListView();
            productNameHeader = new ColumnHeader();
            productCountHeader = new ColumnHeader();
            label1 = new Label();
            cb_SelectDevice = new ComboBox();
            label2 = new Label();
            btn_ok = new Button();
            label3 = new Label();
            txt_ProposalStatus = new Label();
            btn_uploadProposal = new Button();
            im_UploadStatus = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)im_UploadStatus).BeginInit();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { productNameHeader, productCountHeader });
            listView1.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2 });
            listView1.Location = new Point(12, 38);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new Size(345, 224);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // productNameHeader
            // 
            productNameHeader.Text = "Название";
            productNameHeader.Width = 200;
            // 
            // productCountHeader
            // 
            productCountHeader.Text = "Количество";
            productCountHeader.Width = 80;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 13);
            label1.Name = "label1";
            label1.Size = new Size(112, 15);
            label1.TabIndex = 1;
            label1.Text = "Продукты в заявке:";
            // 
            // cb_SelectDevice
            // 
            cb_SelectDevice.FormattingEnabled = true;
            cb_SelectDevice.Items.AddRange(new object[] { "test1", "test2", "test3", "test4", "test5", "test6", "test7", "test8", "test9", "test10" });
            cb_SelectDevice.Location = new Point(217, 326);
            cb_SelectDevice.Name = "cb_SelectDevice";
            cb_SelectDevice.Size = new Size(140, 23);
            cb_SelectDevice.TabIndex = 2;
            cb_SelectDevice.Text = "Выбрать устр-во";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 319);
            label2.Name = "label2";
            label2.Size = new Size(80, 30);
            label2.TabIndex = 3;
            label2.Text = "Назначенное\r\nустройство:";
            // 
            // btn_ok
            // 
            btn_ok.Location = new Point(137, 377);
            btn_ok.Name = "btn_ok";
            btn_ok.Size = new Size(85, 36);
            btn_ok.TabIndex = 4;
            btn_ok.Text = "ОК";
            btn_ok.UseVisualStyleBackColor = true;
            btn_ok.Click += btn_ok_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 284);
            label3.Name = "label3";
            label3.Size = new Size(69, 15);
            label3.TabIndex = 6;
            label3.Text = "Состояние:";
            // 
            // txt_ProposalStatus
            // 
            txt_ProposalStatus.AutoSize = true;
            txt_ProposalStatus.Location = new Point(217, 284);
            txt_ProposalStatus.Name = "txt_ProposalStatus";
            txt_ProposalStatus.Size = new Size(91, 15);
            txt_ProposalStatus.TabIndex = 7;
            txt_ProposalStatus.Text = "Не обработано";
            // 
            // btn_uploadProposal
            // 
            btn_uploadProposal.Image = Properties.Resources.im_uploadProposal;
            btn_uploadProposal.Location = new Point(161, 326);
            btn_uploadProposal.Name = "btn_uploadProposal";
            btn_uploadProposal.Size = new Size(41, 23);
            btn_uploadProposal.TabIndex = 8;
            btn_uploadProposal.UseVisualStyleBackColor = true;
            btn_uploadProposal.Click += btn_uploadProposal_Click;
            // 
            // im_UploadStatus
            // 
            im_UploadStatus.Image = Properties.Resources.gif_uploadProcess;
            im_UploadStatus.Location = new Point(161, 326);
            im_UploadStatus.Name = "im_UploadStatus";
            im_UploadStatus.Size = new Size(41, 23);
            im_UploadStatus.SizeMode = PictureBoxSizeMode.CenterImage;
            im_UploadStatus.TabIndex = 9;
            im_UploadStatus.TabStop = false;
            im_UploadStatus.Visible = false;
            // 
            // ProposalDetailsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(369, 425);
            Controls.Add(im_UploadStatus);
            Controls.Add(txt_ProposalStatus);
            Controls.Add(label3);
            Controls.Add(btn_ok);
            Controls.Add(label2);
            Controls.Add(cb_SelectDevice);
            Controls.Add(label1);
            Controls.Add(listView1);
            Controls.Add(btn_uploadProposal);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ProposalDetailsForm";
            Text = "Заявка";
            ((System.ComponentModel.ISupportInitialize)im_UploadStatus).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private ColumnHeader productNameHeader;
        private ColumnHeader productCountHeader;
        private Label label1;
        private ComboBox cb_SelectDevice;
        private Label label2;
        private Button btn_ok;
        private Button button2;
        private Label label3;
        private Label txt_ProposalStatus;
        private Button btn_uploadProposal;
        private PictureBox im_UploadStatus;
    }
}