using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Configuration;

namespace FTPClient
{
    public partial class MainForm : Form
    {
        private object mainInfo;
        private string uri = "http://github.com/sanmeng/ftp_client";

        //System.Configuration.Configuration config;

        public MainForm(object user)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            mainInfo = user;

            //config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //uri = config.AppSettings.Settings["Homepage"].Value;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MyFTPControl.LoadUser(mainInfo);
            string[] infoList = (string[])mainInfo;
            webBrowser1.Navigate(uri+"username="+infoList[1]+"&password="+infoList[2]);
            webBrowser1.AllowWebBrowserDrop = false;
            webBrowser1.WebBrowserShortcutsEnabled = false;
            webBrowser1.IsWebBrowserContextMenuEnabled = false;
        }

        private void toolStripButton_home_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(0);
            tBtn_back.Visible = true;
            tBtn_forward.Visible = true;
        }

        private void toolStripButton_ftp_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(1);
            tBtn_back.Visible = false;
            tBtn_forward.Visible = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            //将所有的链接的目标，指向本窗体 
            foreach (HtmlElement archor in this.webBrowser1.Document.Links)
            {
                archor.SetAttribute("target", "_self");
            }
            //将所有的FORM的提交目标，指向本窗体 
            foreach (HtmlElement form in this.webBrowser1.Document.Forms)
            {
                form.SetAttribute("target", "_self");
            }
        }

        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void tBtn_back_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void tBtn_forward_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }
    }
}
