using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Login;
using System.Configuration;
namespace lht_ftp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            System.Net.ServicePointManager.DefaultConnectionLimit = 200;
            System.Threading.ThreadPool.SetMaxThreads(5, 5);

            var server = ConfigurationManager.AppSettings["FtpServer"];

            using (LoginForm lf = new LoginForm())
            {
                if (lf.ShowDialog() == DialogResult.OK)
                {
                    string[] info = new string[3]{lf.ftpHost,lf.ftpUserName,lf.ftpPassword};
                    Application.Run(new MainForm(info));
                }
                else
                {
                    return;
                }
            }
        }
    }
}
