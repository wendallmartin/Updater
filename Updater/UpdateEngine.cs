using System;
using System.Collections.Generic;

namespace FTPUpdater
{
    public abstract class UpdateEngine
    {
        public delegate void UpdateChangedDel(double recieved, double total);
        public UpdateChangedDel UpdateChangedEvent;
        
        /// <summary>
        /// Returns list of Versions available on ftp update server.
        /// </summary>
        /// <returns></returns>
        public abstract List<DetailVersion> GetUpdateVersions();
        
        public abstract void DownloadUpdate(string url, string localPath);
        public abstract void Update(Version version);
    }
}