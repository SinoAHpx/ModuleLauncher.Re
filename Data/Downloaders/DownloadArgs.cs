using System;

namespace AHpx.ModuleLauncher.Data.Downloaders
{
    public class DownloadArgs
    {
        public class StartedArgs
        {
            public string FileName { get; set; }
            public long FileSize { get; set; }
        }

        public class CompletedArgs
        {
            public Exception Exception { get; set; }
            public bool Cancelled { get; set; }
        }

        public class ProgressArgs
        {
            public string ProgressID { get; set; }
            public double Percentage { get; set; }
            public byte[] ReceivedBytes { get; set; }
            public long ProgressedBytesSize { get; set; }
            public long ReceivedBytesSize { get; set; }
            public long TotalBytesSize { get; set; }
            public double BytesPerSecondSpeed { get; set; }
            public double AverageBytesPerSecondSpeed { get; set; }
        }
    }
}