using System;
using System.Collections.Generic;

namespace Updater
{
    public abstract class UpdateEngine
    {
        public enum InstanceType
        {
            Update,
            Install
        }
        
        public InstanceType Type;
        
        public delegate void UpdateChangedDel(double received, double total);
        public UpdateChangedDel UpdateChangedEvent;
        protected string Url = "http://www.wrmcodeblocks.com/TheTimeApp/Downloads";

        public string DownloadDirectory { get;  set; }
        
        public Version UpdateVersion { get; protected set; }

        /// <summary>
        /// Returns list of Versions available on ftp update server.
        /// </summary>
        /// <returns></returns>
        public abstract List<DetailVersion> GetUpdateVersions();

        public abstract void Update(Version version);
    }
}