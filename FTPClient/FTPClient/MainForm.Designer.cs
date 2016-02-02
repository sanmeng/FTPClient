namespace FTPClient
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tb_web = new System.Windows.Forms.TabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.tb_ftp = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_home = new System.Windows.Forms.ToolStripButton();
            this.tBtn_back = new System.Windows.Forms.ToolStripButton();
            this.tBtn_forward = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_ftp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.MyFTPControl = new FTPControl.FTPControl();
            this.tabControl.SuspendLayout();
            this.tb_web.SuspendLayout();
            this.tb_ftp.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tb_web);
            this.tabControl.Controls.Add(this.tb_ftp);
            this.tabControl.Location = new System.Drawing.Point(0, 23);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(943, 507);
            this.tabControl.TabIndex = 0;
            // 
            // tb_web
            // 
            this.tb_web.Controls.Add(this.pictureBox1);
            this.tb_web.Controls.Add(this.webBrowser1);
            this.tb_web.Location = new System.Drawing.Point(4, 22);
            this.tb_web.Name = "tb_web";
            this.tb_web.Padding = new System.Windows.Forms.Padding(3);
            this.tb_web.Size = new System.Drawing.Size(935, 481);
            this.tb_web.TabIndex = 0;
            this.tb_web.Text = "私有云";
            this.tb_web.UseVisualStyleBackColor = true;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(-4, 3);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(943, 474);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Url = new System.Uri("http://192.168.2.34", System.UriKind.Absolute);
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            this.webBrowser1.NewWindow += new System.ComponentModel.CancelEventHandler(this.webBrowser1_NewWindow);
            // 
            // tb_ftp
            // 
            this.tb_ftp.Controls.Add(this.MyFTPControl);
            this.tb_ftp.Location = new System.Drawing.Point(4, 22);
            this.tb_ftp.Name = "tb_ftp";
            this.tb_ftp.Padding = new System.Windows.Forms.Padding(3);
            this.tb_ftp.Size = new System.Drawing.Size(935, 481);
            this.tb_ftp.TabIndex = 1;
            this.tb_ftp.Text = "FTP";
            this.tb_ftp.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_home,
            this.tBtn_back,
            this.tBtn_forward,
            this.toolStripSeparator1,
            this.toolStripButton_ftp,
            this.toolStripSeparator2,
            this.toolStripButton4});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(943, 48);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 48);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 48);
            // 
            // toolStripButton_home
            // 
            this.toolStripButton_home.AutoSize = false;
            this.toolStripButton_home.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_home.Image = global::lht_ftp.Properties.Resources.shouy;
            this.toolStripButton_home.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_home.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButton_home.Name = "toolStripButton_home";
            this.toolStripButton_home.Size = new System.Drawing.Size(40, 40);
            this.toolStripButton_home.Text = "主页";
            this.toolStripButton_home.Click += new System.EventHandler(this.toolStripButton_home_Click);
            // 
            // tBtn_back
            // 
            this.tBtn_back.AutoSize = false;
            this.tBtn_back.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tBtn_back.Image = global::lht_ftp.Properties.Resources.hout;
            this.tBtn_back.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tBtn_back.ImageTransparentColor = System.Drawing.Color.White;
            this.tBtn_back.Name = "tBtn_back";
            this.tBtn_back.Size = new System.Drawing.Size(40, 40);
            this.tBtn_back.Text = "后退";
            this.tBtn_back.Click += new System.EventHandler(this.tBtn_back_Click);
            // 
            // tBtn_forward
            // 
            this.tBtn_forward.AutoSize = false;
            this.tBtn_forward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tBtn_forward.Image = global::lht_ftp.Properties.Resources.qianj;
            this.tBtn_forward.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tBtn_forward.ImageTransparentColor = System.Drawing.Color.White;
            this.tBtn_forward.Name = "tBtn_forward";
            this.tBtn_forward.Size = new System.Drawing.Size(40, 40);
            this.tBtn_forward.Text = "前进";
            this.tBtn_forward.Click += new System.EventHandler(this.tBtn_forward_Click);
            // 
            // toolStripButton_ftp
            // 
            this.toolStripButton_ftp.AutoSize = false;
            this.toolStripButton_ftp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_ftp.Image = global::lht_ftp.Properties.Resources.ftp1;
            this.toolStripButton_ftp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_ftp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_ftp.Name = "toolStripButton_ftp";
            this.toolStripButton_ftp.Size = new System.Drawing.Size(40, 40);
            this.toolStripButton_ftp.Text = "FTP";
            this.toolStripButton_ftp.Click += new System.EventHandler(this.toolStripButton_ftp_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.AutoSize = false;
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = global::lht_ftp.Properties.Resources.tuic;
            this.toolStripButton4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(40, 40);
            this.toolStripButton4.Text = "退出";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImage = global::lht_ftp.Properties.Resources.web;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(172, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(591, 410);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // MyFTPControl
            // 
            this.MyFTPControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MyFTPControl.Location = new System.Drawing.Point(-4, 3);
            this.MyFTPControl.Name = "MyFTPControl";
            this.MyFTPControl.Size = new System.Drawing.Size(939, 478);
            this.MyFTPControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 534);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "蓝海创意云 - 私有云";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tb_web.ResumeLayout(false);
            this.tb_ftp.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tb_web;
        private System.Windows.Forms.TabPage tb_ftp;
        private FTPControl.FTPControl MyFTPControl;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_home;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton_ftp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripButton tBtn_back;
        private System.Windows.Forms.ToolStripButton tBtn_forward;
    }
}

