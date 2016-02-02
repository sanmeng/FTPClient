using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Collections;

using System.IO.Compression;

using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Runtime.Serialization;
using Microsoft.Win32;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FTPControl
{
    class FtpClient
    {
        public class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon
            [DllImport("shell32.dll", EntryPoint = "ExtractIcon")]
            public static extern int ExtractIcon(IntPtr hInst, string lpFileName, int nIndex);
            [DllImport("shell32.dll", EntryPoint = "SHGetFileInfo")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttribute, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint Flags);
            [DllImport("User32.dll", EntryPoint = "DestroyIcon")]
            public static extern int DestroyIcon(IntPtr hIcon);
            [DllImport("shell32.dll")]
            public static extern uint ExtractIconEx(string lpszFile, int nIconIndex, int[] phiconLarge, int[] phiconSmall, uint nIcons);
            [StructLayout(LayoutKind.Sequential)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public IntPtr iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            }
        }

        #region  获取服务器图标
        /// 给出文件扩展名（.*），返回相应图标
        /// 若不以"."开头则返回文件夹的图标。
        public Icon GetIconByFileType(string fileType,bool isLarge)
        {
            if(fileType == null || fileType.Equals(string.Empty)) return null;
            RegistryKey regVersion = null;
            string regFileType = null;
            string regIconString = null;
            string systemDirectory = Environment.SystemDirectory + "\\";
            if(fileType[0] == '.')
            {
                //读系统注册表中文件类型信息
                regVersion = Registry.ClassesRoot.OpenSubKey(fileType, true);
                if(regVersion != null)
                {
                    regFileType = regVersion.GetValue("") as string;
                    regVersion.Close();
                    regVersion = Registry.ClassesRoot.OpenSubKey(regFileType + @"\DefaultIcon" , true);
                    if(regVersion != null)
                    {
                        regIconString = regVersion.GetValue("") as string;
                        regVersion.Close();
                    }
                }
                if(regIconString == null)
                {
                    //没有读取到文件类型注册信息，指定为未知文件类型的图标
                    regIconString = systemDirectory +"shell32.dll,0";
                }
            }
            else if (fileType[0] == '@')
            {
                regIconString = systemDirectory + "shell32.dll,0";
            }
            else
            {
                //直接指定为文件夹图标
                regIconString = systemDirectory +"shell32.dll,3";
            }
            string[] fileIcon = regIconString.Split(new char[]{','});
            if(fileIcon.Length != 2)
            {
                //系统注册表中注册的标图不能直接提取，则返回可执行文件的通用图标
                fileIcon = new string[]{systemDirectory +"shell32.dll","2"};
            }
            Icon resultIcon = null;
            try
            {
                //调用API方法读取图标
                int[] phiconLarge = new int[1];
                int[] phiconSmall = new int[1];
                uint count = Win32.ExtractIconEx(fileIcon[0],Int32.Parse(fileIcon[1]),phiconLarge,phiconSmall,1);
                IntPtr IconHnd = new IntPtr(isLarge?phiconLarge[0]:phiconSmall[0]);
                resultIcon = Icon.FromHandle(IconHnd);
            }

            catch
            {
                fileIcon = new string[] { systemDirectory + "shell32.dll", "2" };
                                //调用API方法读取图标
                int[] phiconLarge = new int[1];
                int[] phiconSmall = new int[1];
                uint count = Win32.ExtractIconEx(fileIcon[0],Int32.Parse(fileIcon[1]),phiconLarge,phiconSmall,1);
                IntPtr IconHnd = new IntPtr(isLarge?phiconLarge[0]:phiconSmall[0]);
                resultIcon = Icon.FromHandle(IconHnd);
            }
            return resultIcon;
        }
        #endregion

        #region  文件夹的复制
        /// <summary>
        /// 文件夹的复制
        /// </summary>
        /// <param Ddir="string">要复制的目的路径</param>
        /// <param Sdir="string">要复制的原路径</param>
        public void Files_Copy(string Ddir, string Sdir)
        {
            DirectoryInfo dir = new DirectoryInfo(Sdir);
            string SbuDir = Ddir;
            try
            {
                if (!dir.Exists)//判断所指的文件或文件夹是否存在
                {
                    return;
                }
                DirectoryInfo dirD = dir as DirectoryInfo;//如果给定参数不是文件夹则退出
                string UpDir = UpAndDown_Dir(Ddir);
                if (dirD == null)//判断文件夹是否为空
                {
                    Directory.CreateDirectory(UpDir + "\\" + dirD.Name);//如果为空，创建文件夹并退出
                    return;
                }
                else
                {
                    Directory.CreateDirectory(UpDir + "\\" + dirD.Name);
                }
                SbuDir = UpDir + "\\" + dirD.Name + "\\";
                FileSystemInfo[] files = dirD.GetFileSystemInfos();//获取文件夹中所有文件和文件夹
                //对单个FileSystemInfo进行判断,如果是文件夹则进行递归操作
                foreach (FileSystemInfo FSys in files)
                {
                    FileInfo file = FSys as FileInfo;
                    if (file != null)//如果是文件的话，进行文件的复制操作
                    {
                        FileInfo SFInfo = new FileInfo(file.DirectoryName + "\\" + file.Name);//获取文件所在的原始路径
                        SFInfo.CopyTo(SbuDir + "\\" + file.Name, true);//将文件复制到指定的路径中
                    }
                    else
                    {
                        string pp = FSys.Name;//获取当前搜索到的文件夹名称
                        Files_Copy(SbuDir + FSys.ToString(), Sdir + "\\" + FSys.ToString());//如果是文件，则进行递归调用
                    }
                }
            }
            catch
            {
                MessageBox.Show("文件制复失败。");
                return;
            }
        }
        #endregion

        #region  返回上一级目录
        /// <summary>
        /// 返回上一级目录
        /// </summary>
        /// <param dir="string">目录</param>
        /// <returns>返回String对象</returns>
        public string UpAndDown_Dir(string dir)
        {
            string Change_dir = "";
            Change_dir = Directory.GetParent(dir).FullName;
            return Change_dir;
        }
        #endregion


        public void GetFtpServerIco(ImageList il,string ftpip,string  user,string  pwd,ListView lv,string path)//获取服务器的图标
        {
            try
            {
                string[] a;
                lv.Items.Clear();
                il.Images.Clear();
                if(path.Length==0)
                    a = GetFileList(ftpip, user, pwd);
                else
                    a= GetFileList(ftpip + "/" + path.Remove(path.LastIndexOf("/")), user, pwd);
                if (a != null)
                {
                    var fileInfoList = GetList(a);
                    var items = new List<ListViewItem>();
                    for (int i = 0; i < fileInfoList.Length; i++)
                    {
                        FtpFileItem f = fileInfoList[i];
                        string filetype = "";
                        if (f.IsDirectory)
                        {
                            filetype = f.Name;
                        }
                        else
                        {
                            if (f.Name.IndexOf('.') != -1)
                                filetype = f.Name.Substring(f.Name.LastIndexOf("."), f.Name.Length - f.Name.LastIndexOf("."));
                            else
                                filetype = "@";
                        }
                        il.Images.Add(GetIconByFileType(filetype, true));
                        string[] info = new string[4];
                        try
                        {
                            FileInfo fi = new FileInfo(f.Name);
                            info[0] = fi.Name;
                            //info[1] = GetFileSize(f.Name, ftpip, user, pwd, path).ToString();
                            if (f.IsDirectory)
                            {
                                info[2] = "";
                                info[1] = "文件夹";
                            }
                            else
                            {
                                info[2] = (f.Size / 1024 + 1).ToString() + " KB";
                                //info[2] = (GetFileSize(f.Name, ftpip, user, pwd, path) / 1024 + 1).ToString() + " KB";
                                //info[2] = " -- KB";
                                info[1] = fi.Extension.ToString();
                            }
                            ListViewItem item = new ListViewItem(info, i);
                            //lv.Items.Add(item);
                            items.Add(item);
                        }
                        catch (ArgumentException ex)
                        { 
                        }
                    }

                    lv.Items.AddRange(items.ToArray());
                }
            }
            catch(Exception e)
            {
            }
        }

        public void ListFolders(ToolStripComboBox tscb)//获取本地磁盘目录
        {
            string[] logicdrives = System.IO.Directory.GetLogicalDrives();
            for (int i = 0; i < logicdrives.Length; i++)
            {
                tscb.Items.Add(logicdrives[i]);
                tscb.SelectedIndex = 0;
            }
        }

        int k = 0;
        public void GoBack(ListView lv,ImageList il,string path)
        {

            if (AllPath.Length != 3)
            {
                string NewPath = AllPath.Remove(AllPath.LastIndexOf("\\")).Remove(AllPath.Remove(AllPath.LastIndexOf("\\")).LastIndexOf("\\")) + "\\";
                lv.Items.Clear();
                GetListViewItem(NewPath, il, lv);
                AllPath = NewPath;
            }
            else
            {
                if (k == 0)
                {
                    lv.Items.Clear();
                    GetListViewItem(path, il, lv);
                    k++;
                }
            }
        }
        public string Mpath()
        {
            string path=AllPath;
            return path;
        }

        public static string AllPath = "";//---------
        public void GetPath(string path, ImageList imglist, ListView lv,int ppath)//-------
        {
                string pp = "";
                string uu = "";
                if (ppath == 0)
                {
                    if (AllPath != path)
                    {
                        lv.Items.Clear();
                        AllPath = path;
                        GetListViewItem(AllPath, imglist, lv);
                    }
                }
                else
                {
                    uu = AllPath + path;
                    if (Directory.Exists(uu))
                    {
                        AllPath = AllPath + path + "\\";
                        pp = AllPath.Substring(0, AllPath.Length - 1);
                        lv.Items.Clear();
                        GetListViewItem(pp, imglist, lv);
                    }
                    else
                    {
                        uu = AllPath + path;
                        System.Diagnostics.Process.Start(uu);
                    }
                }
         }

        public void GetListViewItem(string path, ImageList imglist, ListView lv)//获取指定路径下所有文件及其图标
        {
            lv.Items.Clear();
            Win32.SHFILEINFO shfi = new Win32.SHFILEINFO();
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                for (int i = 0; i < dirs.Length; i++)
                {
                    string[] info = new string[4];
                    DirectoryInfo dir = new DirectoryInfo(dirs[i]);
                    if (dir.Name == "RECYCLER" || dir.Name == "RECYCLED" || dir.Name == "Recycled" || dir.Name == "System Volume Information")
                    { }
                    else
                    {
                        //获得图标
                        Win32.SHGetFileInfo(dirs[i],
                                            (uint)0x80,
                                            ref shfi,
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                                            (uint)(0x100 | 0x400)); //取得Icon和TypeName
                        //添加图标
                        imglist.Images.Add(dir.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = dir.Name;
                        info[1] = "";
                        info[2] = "文件夹";
                        info[3] = dir.LastWriteTime.ToString();
                        ListViewItem item = new ListViewItem(info, dir.Name);
                        lv.Items.Add(item);
                        //销毁图标
                        Win32.DestroyIcon(shfi.hIcon);
                    }
                }
                for (int i = 0; i < files.Length; i++)
                {
                    string[] info = new string[4];
                    FileInfo fi = new FileInfo(files[i]);
                    string Filetype = fi.Name.Substring(fi.Name.LastIndexOf(".")+1,fi.Name.Length- fi.Name.LastIndexOf(".") -1);
                    string newtype=Filetype.ToLower();
                    if (newtype == "sys" || newtype == "ini" || newtype == "bin" || newtype == "log" || newtype == "com" || newtype == "bat" || newtype == "db")
                    { }
                    else
                    {


                        //获得图标
                        Win32.SHGetFileInfo(files[i],
                                            (uint)0x80,
                                            ref shfi,
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                                            (uint)(0x100 | 0x400)); //取得Icon和TypeName
                        //添加图标
                        imglist.Images.Add(fi.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = fi.Name;
                        info[1] = (fi.Length/1024+1).ToString()+" KB";
                        info[2] = fi.Extension.ToString();
                        info[3] = fi.LastWriteTime.ToString();
                        ListViewItem item = new ListViewItem(info, fi.Name);
                        lv.Items.Add(item);
                        //销毁图标
                        Win32.DestroyIcon(shfi.hIcon);
                    }
                }
            }
            catch
            {
            }
        }

        FtpWebRequest reqFTP;
        private string ftpHost;
        private string ftpUserName;
        private string ftpPassword;

        public FtpClient(string ftpHost, string ftpUserName, string ftpPassword)
        {
            // TODO: Complete member initialization
            this.ftpHost = ftpHost;
            this.ftpUserName = ftpUserName;
            this.ftpPassword = ftpPassword;
        }

        public bool CheckFtp(string DomainName, string FtpUserName, string FtpUserPwd)//验证登录用户是否合法
        {
            bool ResultValue = true;
            try
            {
                FtpWebRequest ftprequest = (FtpWebRequest)WebRequest.Create("ftp://" + DomainName);//创建FtpWebRequest对象
                ftprequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);//设置FTP登陆信息
                ftprequest.Method = WebRequestMethods.Ftp.ListDirectory;//发送一个请求命令
                FtpWebResponse ftpResponse = (FtpWebResponse)ftprequest.GetResponse();//响应一个请求
                ftpResponse.Close();//关闭请求
            }
            catch
            {
                ResultValue = false;
            }
            return ResultValue;
        }

        public long GetFileSize(string filename, string ftpserver,string ftpUserID, string ftpPassword,string path)
        {
            long filesize = 0;
            try
            {
                FileInfo fi = new FileInfo(filename);
                string uri;
                if(path.Length==0)
                    uri = "ftp://" + ftpserver + "/" + fi.Name;
                else
                    uri = "ftp://" + ftpserver + "/" +path+ fi.Name;
                Connect(uri, ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.Proxy = null;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                filesize = response.ContentLength;
                return filesize;
            }
            catch
            {
                return 0;
            }
        }

        public FtpFileItem[] List(string path)
        {
            var list = GetFTPList(ftpHost, ftpUserName, ftpPassword, path);
            if(list == null)
                return new FtpFileItem[0];

            return GetList(list);
        }

        /// <summary>
        /// 获取FTP文件列表
        /// </summary>
        /// <param name="ftpServerIP"></param>
        /// <param name="ftpUserID"></param>
        /// <param name="ftpPassword"></param>
        /// <returns></returns>
        public string[] GetFileList(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                var resulsts = new List<String>();
                string line = reader.ReadLine();
                while (line != null)
                {
                    resulsts.Add(line);
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                return resulsts.ToArray();
            }
            catch
            {
                downloadFiles = null;
                return downloadFiles;
            }
        }

        /// <summary>
        /// 获取FTP指定文件夹的文件列表
        /// </summary>
        /// <param name="ftpServerIP"></param>
        /// <param name="ftpUserID"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <returns>文件信息列表</returns>
        public string[] GetFileListAll(string ftpServerIP, string ftpUserID, string ftpPassword,string filename,string path)
        {
            if (path == null)
                path = "";
            if (path.Length == 0)
            {
                string[] downloadFiles;
                StringBuilder result = new StringBuilder();
                FtpWebRequest reqFTP;
                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + filename));
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                    WebResponse response = reqFTP.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Append(line);
                        result.Append("\n");
                        line = reader.ReadLine();
                    }
                    result.Remove(result.ToString().LastIndexOf('\n'), 1);
                    reader.Close();
                    response.Close();
                    return result.ToString().Split('\n');
                }
                catch
                {
                    downloadFiles = null;
                    return downloadFiles;
                }
            }
            else
            {
                string[] downloadFiles;
                StringBuilder result = new StringBuilder();
                FtpWebRequest reqFTP;
                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" +path+ filename));
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                    WebResponse response = reqFTP.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Append(line);
                        result.Append("\n");
                        line = reader.ReadLine();
                    }
                    result.Remove(result.ToString().LastIndexOf('\n'), 1);
                    reader.Close();
                    response.Close();
                    return result.ToString().Split('\n');
                }
                catch
                {
                    downloadFiles = null;
                    return downloadFiles;
                }
            }
        }

        /// <去空格>
        /// 去除字符串中的空格
        /// </去空格>
        /// <param name="str">源字符串</param>
        /// <returns>新的字符串</returns>
        private string QCKG(string str)
        {
            string a = "";
            CharEnumerator CEnumerator = str.GetEnumerator();
            while (CEnumerator.MoveNext())
            {
                byte[] array = new byte[1];
                array = System.Text.Encoding.ASCII.GetBytes(CEnumerator.Current.ToString());
                int asciicode = (short)(array[0]);
                if (asciicode != 32)
                {
                    a += CEnumerator.Current.ToString();
                }
            }
            return a;
        }

        public event EventHandler<TransferEventArgs> FileUploadBegin;
        public event EventHandler<TransferProgressEventArgs> FileUploadPorgress;
        public event EventHandler<TransferEventArgs> FileUploadFinished;
        public event EventHandler<TransferEventArgs> FileUploadFailed;

        private void UploadThread(Object stateInfo)
        {
            // No state object was passed to QueueUserWorkItem, so 
            // stateInfo is null.            
            var task = (UploadTask)stateInfo;


            if (task.RemoteDir == null)
                task.RemoteDir = "";
            FileInfo fileInf = new FileInfo(task.FileName);

            var transferEventArgs = new TransferEventArgs(task.FileName, task.RemoteDir, fileInf.Length);
            if (FileUploadBegin != null)
            {
                FileUploadBegin(this, transferEventArgs);
            }

            int allbye = (int)(fileInf.Length / 1024 + 1);
            string newFileName;
            if (fileInf.Name.IndexOf("#") == -1)
            {
                newFileName = QCKG(fileInf.Name);
            }
            else
            {
                newFileName = fileInf.Name.Replace("#", "＃");
                newFileName = QCKG(newFileName);
            }
            string uri;
            if (task.RemoteDir.Length == 0)
                uri = "ftp://" + ftpHost + "/" + newFileName;
            else
                uri = "ftp://" + ftpHost + "/" + task.RemoteDir + "/" + newFileName;

            FtpWebRequest reqFTP;
            // 根据uri创建FtpWebRequest对象 
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            reqFTP.Proxy = null;
            reqFTP.KeepAlive = true;
            // ftp用户名和密码 
            reqFTP.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
            // 默认为true，连接不会被关闭 
            // 在一个命令之后被执行 
            reqFTP.KeepAlive = false;
            // 指定执行什么命令 
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 指定数据传输类型 
            reqFTP.UseBinary = true;
            // 上传文件时通知服务器文件的大小 
            reqFTP.ContentLength = fileInf.Length;
            //int buffLength = 2048;// 缓冲大小设置为2kb 
            int buffLength = 1024 * 1024; 

            byte[] buff = new byte[buffLength];
            // 打开一个文件流 (System.IO.FileStream) 去读上传的文件 
            FileStream fs = fileInf.OpenRead();
            try
            {
                long startbye = 0;
                // 把上传的文件写入流 
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的2kb 
                int contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束 
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入 upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    startbye += contentLen;
                    //pb.Value = (int)(startbye / 1024 + 1);
                    if (FileUploadPorgress != null)
                    {
                        FileUploadPorgress(this, new TransferProgressEventArgs(task.FileName, task.RemoteDir, fileInf.Length, startbye));
                    }
                }
                // 关闭两个流 
                strm.Close();
                fs.Close();

                if(FileUploadFinished !=null)
                {
                    FileUploadFinished(this, transferEventArgs);
                }
            }
            catch(Exception e)
            {
                if(FileUploadFailed !=null)
                {
                    FileUploadFailed(this, transferEventArgs);
                }
            }
        }

        public void Upload(string filename, string path)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(UploadThread), new UploadTask(filename, path));
        }

        /// <上传>
        /// 上传
        /// </上传>
        /// <param name="filename">上传的文件</param>
        /// <param name="ftpServerIP"></param>
        /// <param name="ftpUserID"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="pb"></param>
        /// <param name="path"></param>
        /// <returns>上传成功返回True</returns>
        public bool Upload(string filename, string ftpServerIP, string ftpUserID, string ftpPassword, ToolStripProgressBar pb,string path)
        {
            if (path == null)
                path = "";
            bool success = true;
            FileInfo fileInf = new FileInfo(filename);
            int allbye = (int)(fileInf.Length/1024 + 1);
            long startbye = 0;
            pb.Maximum = allbye;
            pb.Minimum = 0;
            string newFileName;
            if (fileInf.Name.IndexOf("#") == -1)
            {
                newFileName =QCKG(fileInf.Name);
            }
            else
            {
                newFileName = fileInf.Name.Replace("#", "＃");
                newFileName = QCKG(newFileName);
            }
            string uri;
            if (path.Length == 0)
                uri = "ftp://" + ftpServerIP + "/" + newFileName;
            else
                uri = "ftp://" + ftpServerIP + "/" + path + newFileName;
            FtpWebRequest reqFTP;
            // 根据uri创建FtpWebRequest对象 
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            // ftp用户名和密码 
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            // 默认为true，连接不会被关闭 
            // 在一个命令之后被执行 
            reqFTP.KeepAlive = false;
            // 指定执行什么命令 
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 指定数据传输类型 
            reqFTP.UseBinary = true;
            // 上传文件时通知服务器文件的大小 
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;// 缓冲大小设置为2kb 
            byte[] buff = new byte[buffLength];
            // 打开一个文件流 (System.IO.FileStream) 去读上传的文件 
            FileStream fs= fileInf.OpenRead();
            try
            {
                // 把上传的文件写入流 
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的2kb 
                int contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束 
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入 upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    startbye += contentLen;
                    pb.Value = (int)(startbye/1024 + 1);
                }
                // 关闭两个流 
                strm.Close();
                fs.Close();
             }
             catch
             {
                 success = false;
             }
             return success;
        }

        /// <连接FTP>
        /// 连接FTP
        /// </连接FTP>
        /// <param name="path">ftp uri</param>
        /// <param name="ftpUserID">用户</param>
        /// <param name="ftpPassword">密码</param>
        public void Connect(String path, string ftpUserID, string ftpPassword)
        {
            try
            {
                // 根据uri创建FtpWebRequest对象
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
                // 指定数据传输类型
                reqFTP.UseBinary = true;
                // ftp用户名和密码
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            }
            catch (System.NotSupportedException notSupportedException)
            {
                throw notSupportedException;
            }
            catch (System.ArgumentNullException argumentNullException)
            {
                throw argumentNullException;
            }
            catch (System.Security.SecurityException securityException)
            {
                throw securityException;
            }
        }

        /// <删除文件>
        /// 删除文件
        /// </删除文件>
        /// <param name="fileName">文件名</param>
        /// <param name="ftpServerIP">主机</param>
        /// <param name="ftpUserID">用户</param>
        /// <param name="ftpPassword">妈妈</param>
        /// <param name="path">FTP源目录</param>
        public void DeleteFileName(string fileName, string ftpServerIP, string ftpUserID, string ftpPassword,string path)
        {
           try
           {
               string uri;
               if(path.Length==0)
                   uri="ftp://" + ftpServerIP + "/" + fileName;
               else
                   uri = "ftp://" + ftpServerIP + "/" + path + fileName;
               // 根据uri创建FtpWebRequest对象
               reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
               // 指定数据传输类型
               reqFTP.UseBinary = true;
               // ftp用户名和密码
               reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
               // 默认为true，连接不会被关闭
               // 在一个命令之后被执行
               reqFTP.KeepAlive = false;
               // 指定执行什么命令
               reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
               FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
               response.Close();
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message, "删除错误");
           }
        }

        public event EventHandler<TransferEventArgs> FileDownloadBegin;
        public event EventHandler<TransferProgressEventArgs> FileDownloadPorgress;
        public event EventHandler<TransferEventArgs> FileDownloadFinished;
        public event EventHandler<TransferEventArgs> FileDownloadFailed;

        private void DownloadThread(Object stateInfo)
        {
            var task = (DownloadTask)stateInfo;

            FtpWebRequest reqFTP;

            string uri;
            if (task.RemoteDir.Length == 0)
                uri = "ftp://" + ftpHost + "/" + task.FileName;
            else
                uri = "ftp://" + ftpHost + "/" + task.RemoteDir + task.FileName;

            long size = GetFileSize(task.FileName, ftpHost, ftpUserName, ftpPassword, task.RemoteDir);
            long startbye = 0;

            var localPath = task.LocalDir + "\\" + task.FileName;
            var remotePath = task.RemoteDir + "\\" + task.FileName;
            var transferEventArgs = new TransferEventArgs(localPath, remotePath, size);

            try
            {
                if (FileDownloadBegin != null)
                {
                    FileDownloadBegin(this, transferEventArgs);
                }

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                int bufferSize = 1024 * 1024;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                FileStream outputStream = new FileStream(task.LocalDir + "\\" + task.FileName, FileMode.Create);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    startbye += readCount;
                    //pb.Value = (int)(startbye / 1024 + 1);

                    if (FileDownloadPorgress != null)
                    {
                        FileDownloadPorgress(this, new TransferProgressEventArgs(localPath, remotePath, size, startbye));
                    }
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();

                if (FileDownloadFinished != null)
                {
                    FileDownloadFinished(this, transferEventArgs);
                }
            }
            catch (System.NotSupportedException)
            {
                if (FileDownloadFailed != null)
                {
                    FileDownloadFailed(this, transferEventArgs);
                }
            }
            catch (Exception)
            {
                if (FileDownloadFailed != null)
                {
                    FileDownloadFailed(this, transferEventArgs);
                }
            }
        }

        public void Download(string localDir, string remoteDir, string fileName)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadThread), new DownloadTask(localDir, remoteDir, fileName));
        }
        /// <创建目录>
        /// 创建目录
        /// </创建目录>
       /// <param name="dirName">目录名</param>
       /// <param name="ftpServerIP">主机</param>
       /// <param name="ftpUserID">用户</param>
       /// <param name="ftpPassword">密码</param>
        public bool MakeDir(string dirName, string ftpServerIP,string ftpUserID, string ftpPassword)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP +"/"+ dirName;
                Connect(uri, ftpUserID, ftpPassword);//连接       
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.Proxy = null;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool MakeDir(string dirName)
        {
            return MakeDir(dirName, ftpHost, ftpUserName, ftpPassword);
        }


        /// <删除目录>
        /// 删除目录
        /// </删除目录>
        /// <param name="dirName">需要删除的目录</param>
        /// <param name="ftpServerIP">Host</param>
        /// <param name="ftpUserID">用户名</param>
        /// <param name="ftpPassword">密码</param>
        public void delDir(string dirName, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
             try
             {
                 string uri = "ftp://" + ftpServerIP + "/" + dirName;
                 Connect(uri, ftpUserID, ftpPassword);//连接      
                 reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;//向服务器发送删除文件夹的命令
                 FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                 response.Close();
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message);
             }
        }

        /// <获取FTP文件列表>
        /// 获取FTP文件列表
        /// </获取FTP文件列表>
        /// <param name="ftpServerIP">ftp host</param>
        /// <param name="ftpUserID">登录用户名</param>
        /// <param name="ftpPassword">登陆密码</param>
        /// <param name="path">ftp 文件夹</param>
        /// <returns>指定文件夹下的文件列表</returns>
        public string[] GetFTPList(string ftpServerIP, string ftpUserID, string ftpPassword, string path)//指定路径的文件列表
        {
            if (path == null)
            path = "";
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + path.Remove(path.LastIndexOf("/"))));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = true;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch(Exception)
            {
                downloadFiles = null;
                return downloadFiles;
            }
        }

        /// <windows解析文件结构>
        /// 从Windows格式中返回文件信息
        /// </windows解析文件结构>
        /// <param name="Record">文件信息</param>
        private FtpFileItem ParseFileStructFromWindowsStyleRecord(string Record)
        {
            FtpFileItem f = new FtpFileItem();
            string processstr = Record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            myDTFI.ShortTimePattern = "t";
            f.CreateTime = DateTime.Parse(dateStr + " " + timeStr, myDTFI);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
            }
            else
            {
                string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);   // true);
                processstr = strs[1];
                f.IsDirectory = false;
            }
            f.Name = processstr;
            return f;
        }

        /// <Unix解析文件结构>
        /// 从Unix格式中返回文件信息
        /// </Unix解析文件结构>
        /// <param name="Record">文件信息</param>
        private FtpFileItem ParseFileStructFromUnixStyleRecord(string Record)
        {
            FtpFileItem f = new FtpFileItem();
            string processstr = Record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Size = Int64.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 0));
            //_cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            string yearOrTime = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":") >= 0)  //time
            {
                processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
            }
            f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            f.Name = processstr;   //最后就是名称
            return f;
        }

        /// <剪取字符串>
        /// 按照一定的规则进行字符串截取
        /// </剪取字符串>
        /// <param name="s">截取的字符串</param>
        /// <param name="c">查找的字符</param>
        /// <param name="startIndex">查找的位置</param>
        private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }

        /// <判断文件列表的方式>
        /// 判断文件列表的方式Window方式还是Unix方式
        /// </判断文件列表的方式>
        /// <param name="recordList">文件信息列表</param>
        private FileListStyle GuessFileListStyle(string[] recordList)
        {
            foreach (string s in recordList)
            {
                if (s.Length > 10
                 && Regex.IsMatch(s.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                {
                    return FileListStyle.UnixStyle;
                }
                else if (s.Length > 8
                 && Regex.IsMatch(s.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                {
                    return FileListStyle.WindowsStyle;
                }
            }
            return FileListStyle.Unknown;
        }

        /// <summary>
        /// 获得文件和目录列表
        /// </summary>
        /// <param name="datastring">FTP返回的列表字符信息</param>
        private FtpFileItem[] GetList(string[] dataRecords)
        {
            List<FtpFileItem> myListArray = new List<FtpFileItem>();
            FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
            foreach (string s in dataRecords)
            {
                if (_directoryListStyle != FileListStyle.Unknown && s != "")
                {
                    FtpFileItem f = new FtpFileItem();
                    f.Name = "..";
                    switch (_directoryListStyle)
                    {
                        case FileListStyle.UnixStyle:
                            f = ParseFileStructFromUnixStyleRecord(s);
                            break;
                        case FileListStyle.WindowsStyle:
                            f = ParseFileStructFromWindowsStyleRecord(s);
                            break;
                    }
                    if (!(f.Name == "." || f.Name == ".."))
                    {
                        myListArray.Add(f);
                    }
                }
            }
            myListArray.Sort(delegate(FtpFileItem x, FtpFileItem y)
            {
                return x.CompareTo(y);
            });

            return myListArray.ToArray();
        }
    }
}
