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
            lst_Products = new ListView();
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
            // lst_Products
            // 
            lst_Products.Columns.AddRange(new ColumnHeader[] { productNameHeader, productCountHeader });
            lst_Products.Location = new Point(12, 38);
            lst_Products.MultiSelect = false;
            lst_Products.Name = "lst_Products";
            lst_Products.Size = new Size(345, 224);
            lst_Products.TabIndex = 0;
            lst_Products.UseCompatibleStateImageBehavior = false;
            lst_Products.View = View.Details;
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
            cb_SelectDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_SelectDevice.FormattingEnabled = true;
            cb_SelectDevice.Location = new Point(217, 326);
            cb_SelectDevice.Name = "cb_SelectDevice";
            cb_SelectDevice.Size = new Size(140, 23);
            cb_SelectDevice.TabIndex = 2;
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
            im_UploadStatus.Size = new Size(41, 25);
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
            Controls.Add(lst_Products);
            Controls.Add(btn_uploadProposal);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ProposalDetailsForm";
            Text = "Заявка";
            Load += ProposalDetailsForm_Load;
            ((System.ComponentModel.ISupportInitialize)im_UploadStatus).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView lst_Products;
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