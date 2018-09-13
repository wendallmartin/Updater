using System;
using System.Collections.Generic;

namespace Downloader
{
    public abstract class UpdateEngine
    {
        public enum InstanceType
        {
            Update,
            Install
        }
        
        public InstanceType Type;
        
        public delegate void UpdateChangedDel(double recieved, double total);
        public UpdateChangedDel UpdateChangedEvent;
        public string Url = "http://www.wrmcodeblocks.com/TheTimeApp/Downloads";

        public string DownloadDirectory { get;  set; }
        public Version CurrentVersion { get;  set; }
        public Version UpdateVersion { get;  set; }

        /// <summary>
        /// Returns list of Versions available on ftp update server.
        /// </summary>
        /// <returns></returns>
        public abstract List<DetailVersion> GetUpdateVersions();
        
        public abstract void DownloadUpdate(string url, string localPath);
        public abstract void Update(Version version);
    }
}