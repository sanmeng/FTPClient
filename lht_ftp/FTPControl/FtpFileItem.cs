using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTPControl
{
    class FtpFileItem
    {
        public DateTime CreateTime { get; set; }
        public Boolean IsDirectory { get; set; }
        public String Name { get; set; }
        public String Flags { get; set; }
        public String Owner { get; set; }
        public String Group { get; set; }

        public Int64 Size { get; set; }

        public int CompareTo(FtpFileItem f) 
        {
            var t1 = this.IsDirectory ? 1 : 0;
            var t2 = f.IsDirectory ? 1 : 0;
            if (t1 != t2)
            {
                return t2 - t1;
            }

            return this.Name.CompareTo(f.Name);
        }
    }

    enum FileListStyle
    {
        UnixStyle=0,
        WindowsStyle,
        Unknown
    }

    public enum Operation
    {
        DownLoad,
        UpLoad,
        Delete
    }
}
