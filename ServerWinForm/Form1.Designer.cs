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
            btnRefreshAll = new Button();
            lstProposals = new ListView();
            columnRequestId = new ColumnHeader();
            columnDataTime = new ColumnHeader();
            columnStatus = new ColumnHeader();
            tabPage2 = new TabPage();
            lstConnections = new ListView();
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
            tabControl1.Size = new Size(408, 401);
            tabControl1.TabIndex = 0;
            // 
            // tabRequests
            // 
            tabRequests.Controls.Add(btnRefreshAll);
            tabRequests.Controls.Add(lstProposals);
            tabRequests.Location = new Point(4, 24);
            tabRequests.Margin = new Padding(3, 2, 3, 2);
            tabRequests.Name = "tabRequests";
            tabRequests.Padding = new Padding(3, 2, 3, 2);
            tabRequests.Size = new Size(400, 373);
            tabRequests.TabIndex = 0;
            tabRequests.Text = "Заявки";
            tabRequests.UseVisualStyleBackColor = true;
            // 
            // btnRefreshAll
            // 
            btnRefreshAll.Location = new Point(31, 341);
            btnRefreshAll.Name = "btnRefreshAll";
            btnRefreshAll.Size = new Size(148, 27);
            btnRefreshAll.TabIndex = 1;
            btnRefreshAll.Text = "Очистить всё";
            btnRefreshAll.UseVisualStyleBackColor = true;
            btnRefreshAll.Click += btnTest_Click;
            // 
            // lstProposals
            // 
            lstProposals.Activation = ItemActivation.TwoClick;
            lstProposals.Columns.AddRange(new ColumnHeader[] { columnRequestId, columnDataTime, columnStatus });
            lstProposals.FullRowSelect = true;
            lstProposals.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lstProposals.Location = new Point(0, 0);
            lstProposals.Margin = new Padding(3, 2, 3, 2);
            lstProposals.MultiSelect = false;
            lstProposals.Name = "lstProposals";
            lstProposals.Size = new Size(400, 308);
            lstProposals.TabIndex = 0;
            lstProposals.UseCompatibleStateImageBehavior = false;
            lstProposals.View = View.Details;
            lstProposals.KeyPress += lstProposals_KeyPress;
            lstProposals.MouseDoubleClick += lstProposals_MouseDoubleClick;
            // 
            // columnRequestId
            // 
            columnRequestId.Text = "Номер заявки";
            columnRequestId.Width = 120;
            // 
            // columnDataTime
            // 
            columnDataTime.Text = "Время поступления";
            columnDataTime.Width = 150;
            // 
            // columnStatus
            // 
            columnStatus.Text = "Статус";
            columnStatus.Width = 120;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(lstConnections);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Margin = new Padding(3, 2, 3, 2);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3, 2, 3, 2);
            tabPage2.Size = new Size(400, 373);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Подключения";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // lstConnections
            // 
            lstConnections.Columns.AddRange(new ColumnHeader[] { colDeviceName, colIpAddress, colDeviceType });
            lstConnections.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lstConnections.Location = new Point(0, 0);
            lstConnections.Margin = new Padding(3, 2, 3, 2);
            lstConnections.MultiSelect = false;
            lstConnections.Name = "lstConnections";
            lstConnections.Size = new Size(400, 308);
            lstConnections.TabIndex = 0;
            lstConnections.UseCompatibleStateImageBehavior = false;
            lstConnections.View = View.Details;
            // 
            // colDeviceName
            // 
            colDeviceName.Text = "Устройство";
            colDeviceName.Width = 170;
            // 
            // colIpAddress
            // 
            colIpAddress.Text = "IP-адрес";
            colIpAddress.Width = 120;
            // 
            // colDeviceType
            // 
            colDeviceType.Text = "Тип";
            colDeviceType.Width = 105;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(436, 443);
            Controls.Add(tabControl1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            tabRequests.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabRequests;
        private TabPage tabPage2;
        private ListView lstProposals;
        private ColumnHeader columnRequestId;
        private ColumnHeader columnDataTime;
        private ColumnHeader columnStatus;
        private ListView lstConnections;
        private ColumnHeader colDeviceName;
        private ColumnHeader colIpAddress;
        private ColumnHeader colDeviceType;
        private Button btnRefreshAll;
    }
}
