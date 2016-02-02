using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPControl
{
    class UploadTask
    {
        public string RemoteDir { get; set; }
        public string FileName { get; private set; }
        public UploadTask(string fileName, string remoteDir)
        {
            this.FileName = fileName;
            this.RemoteDir = remoteDir;
        }
    }

    class DownloadTask
    {
        public string LocalDir { get; private set; }
        public string RemoteDir { get; private set; }
        public string FileName { get; private set; }

        public DownloadTask(string localDir, string remoteDir, string fileName)
        {
            this.LocalDir = localDir;
            this.RemoteDir = remoteDir;
            this.FileName = fileName;
        }
    }
}