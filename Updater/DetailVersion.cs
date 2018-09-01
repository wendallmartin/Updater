using System;

namespace FTPUpdater
{
    public class DetailVersion
    {
        private Version _version;
        private string _details;
        
        public DetailVersion(Version version, string details)
        {
            _version = version;
            _details = details;
        }

        public Version Version => _version;

        public string Details => _details;
    }
}