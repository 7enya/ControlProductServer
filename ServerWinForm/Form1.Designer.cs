namespace ServerWinForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabRequests = new TabPage();
            lbl_UpdateProposalsFail = new Label();
            btn_UpdateProposals = new Button();
            lst_Proposals = new ListView();
            columnRequestId = new ColumnHeader();
            columnDataTime = new ColumnHeader();
            columnStatus = new ColumnHeader();
            tabPage2 = new TabPage();
            lst_Connections = new ListView();
            colDeviceName = new ColumnHeader();
            colIpAddress = new ColumnHeader();
            colDeviceType = new ColumnHeader();
            tabControl1.SuspendLayout();
            tabRequests.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabRequests);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(18, 16);
            tabControl1.Margin = new Padding(3, 2, 3, 2);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(612, 416);
            tabControl1.TabIndex = 0;
            // 
            // tabRequests
            // 
            tabRequests.Controls.Add(lbl_UpdateProposalsFail);
            tabRequests.Controls.Add(btn_UpdateProposals);
            tabRequests.Controls.Add(lst_Proposals);
            tabRequests.Location = new Point(4, 24);
            tabRequests.Margin = new Padding(3, 2, 3, 2);
            tabRequests.Name = "tabRequests";
            tabRequests.Padding = new Padding(3, 2, 3, 2);
            tabRequests.Size = new Size(604, 388);
            tabRequests.TabIndex = 0;
            tabRequests.Text = "Заявки";
            tabRequests.UseVisualStyleBackColor = true;
            // 
            // lbl_UpdateProposalsFail
            // 
            lbl_UpdateProposalsFail.AutoSize = true;
            lbl_UpdateProposalsFail.BackColor = Color.White;
            lbl_UpdateProposalsFail.Location = new Point(192, 152);
            lbl_UpdateProposalsFail.Name = "lbl_UpdateProposalsFail";
            lbl_UpdateProposalsFail.Size = new Size(205, 15);
            lbl_UpdateProposalsFail.TabIndex = 2;
            lbl_UpdateProposalsFail.Text = "Не удалось обновить список заявок";
            lbl_UpdateProposalsFail.Visible = false;
            // 
            // btn_UpdateProposals
            // 
            btn_UpdateProposals.Location = new Point(222, 339);
            btn_UpdateProposals.Name = "btn_UpdateProposals";
            btn_UpdateProposals.Size = new Size(126, 38);
            btn_UpdateProposals.TabIndex = 1;
            btn_UpdateProposals.Text = "Обновить список";
            btn_UpdateProposals.UseVisualStyleBackColor = true;
            btn_UpdateProposals.Click += btn_UpdateProposals_Click;
            // 
            // lst_Proposals
            // 
            lst_Proposals.Activation = ItemActivation.TwoClick;
            lst_Proposals.Columns.AddRange(new ColumnHeader[] { columnRequestId, columnDataTime, columnStatus });
            lst_Proposals.FullRowSelect = true;
            lst_Proposals.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lst_Proposals.Location = new Point(0, 0);
            lst_Proposals.Margin = new Padding(3, 2, 3, 2);
            lst_Proposals.MultiSelect = false;
            lst_Proposals.Name = "lst_Proposals";
            lst_Proposals.Size = new Size(604, 325);
            lst_Proposals.TabIndex = 0;
            lst_Proposals.UseCompatibleStateImageBehavior = false;
            lst_Proposals.View = View.Details;
            lst_Proposals.KeyPress += lst_Proposals_KeyPress;
            lst_Proposals.MouseDoubleClick += lst_Proposals_MouseDoubleClick;
            // 
            // columnRequestId
            // 
            columnRequestId.Text = "Номер заявки";
            columnRequestId.Width = 200;
            // 
            // columnDataTime
            // 
            columnDataTime.Text = "Время поступления";
            columnDataTime.Width = 170;
            // 
            // columnStatus
            // 
            columnStatus.Text = "Статус";
            columnStatus.Width = 230;
            // 
            // tabPage2
            // 
            tabPage2.BackgroundImageLayout = ImageLayout.None;
            tabPage2.Controls.Add(lst_Connections);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Margin = new Padding(3, 2, 3, 2);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3, 2, 3, 2);
            tabPage2.Size = new Size(604, 388);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Подключения";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // lst_Connections
            // 
            lst_Connections.Columns.AddRange(new ColumnHeader[] { colDeviceName, colIpAddress, colDeviceType });
            lst_Connections.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lst_Connections.Location = new Point(0, 0);
            lst_Connections.Margin = new Padding(3, 2, 3, 2);
            lst_Connections.MultiSelect = false;
            lst_Connections.Name = "lst_Connections";
            lst_Connections.Size = new Size(604, 373);
            lst_Connections.TabIndex = 0;
            lst_Connections.UseCompatibleStateImageBehavior = false;
            lst_Connections.View = View.Details;
            lst_Connections.MouseDoubleClick += lst_Connections_MouseDoubleClick;
            // 
            // colDeviceName
            // 
            colDeviceName.Text = "Устройство";
            colDeviceName.Width = 250;
            // 
            // colIpAddress
            // 
            colIpAddress.Text = "IP-адрес";
            colIpAddress.Width = 215;
            // 
            // colDeviceType
            // 
            colDeviceType.Text = "Тип";
            colDeviceType.Width = 130;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(642, 443);
            Controls.Add(tabControl1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            tabRequests.ResumeLayout(false);
            tabRequests.PerformLayout();
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabRequests;
        private TabPage tabPage2;
        private ListView lst_Proposals;
        private ColumnHeader columnRequestId;
        private ColumnHeader columnDataTime;
        private ColumnHeader columnStatus;
        private ListView lst_Connections;
        private ColumnHeader colDeviceName;
        private ColumnHeader colIpAddress;
        private ColumnHeader colDeviceType;
        private Button btn_UpdateProposals;
        private Label lbl_UpdateProposalsFail;
    }
}