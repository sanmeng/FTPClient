using System;

namespace FTPControl
{
    public class TransferEventArgs : EventArgs
    {
        public string LocalPath { get; private set; }
        public string ServerPath { get; private set; }

        public long Size { get; private set; }

        public TransferEventArgs(string localPath, string serverPath, long size)
        {
            this.LocalPath = localPath;
            this.ServerPath = serverPath;
            this.Size = size;
        }

        public string Key
        {
            get 
            {
                return LocalPath + ServerPath;
            }
        }

        public string SizeString
        {
            get
            {
                if (Size < 1024) // < 1k
                    return string.Format("{0} B", Size);

                var m = 1024 * 1024;
                if (Size < m) // < 1M
                    return string.Format("{0:f2} KB", (Size * 1024.0 / m));

                var g = 1024 * 1024 * 1024;
                if (Size < 1024 * 1024 * 1024) // < 1G
                    return string.Format("{0:f2} MB", (Size * 1024.0 / g));

                return string.Format("{0:f2} GB", (Size * 1.0 / g));
            }
        }

    }
}