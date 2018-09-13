using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace Downloader.States
{
    public class Updates : UpdateState
    {
        public delegate void ProgressChangedDel();
        public static ProgressChangedDel ProgressChangedEvent;
        
        public static UpdateState State { get; } = new Updates();
        
        public override void Previous(Downloader downloader)
        {
            
        }

        public override void Next(Downloader downloader)
        {
            downloader.Download.Visibility = Visibility.Visible;
            downloader.DownloadVersion(downloader.UpdateComboBox.Text);
        }
    }
}