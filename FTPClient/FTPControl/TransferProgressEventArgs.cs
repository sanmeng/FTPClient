namespace FTPControl
{
    public class TransferProgressEventArgs : TransferEventArgs
    {
        public long BytesSent { get; private set; }

        public TransferProgressEventArgs(string localPath, string serverPath, long size, long bytesSent)
            :base(localPath, serverPath, size)
        {
            this.BytesSent = bytesSent;
        }

        public double Progress 
        {
            get { return (BytesSent * 1.0) / Size; } 
        }
    }
}