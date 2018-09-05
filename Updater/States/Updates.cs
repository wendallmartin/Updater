using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace Updater.States
{
    public class Updates : UpdateState
    {
        public delegate void ProgressChangedDel();
        public static ProgressChangedDel ProgressChangedEvent;
        
        public static UpdateState State { get; } = new Updates();
        
        public override void Previous(Installer installer)
        {
            
        }

        public override void Next(Installer installer)
        {
            installer.Download.Visibility = Visibility.Visible;
            installer.DownloadVersion(installer.UpdateComboBox.Text);
        }
    }
}