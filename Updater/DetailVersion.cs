using System;

namespace Updater
{
    public class DetailVersion
    {
        public DetailVersion(Version version, string details)
        {
            Version = version;
            Details = details;
        }

        public Version Version { get; }

        public string Details { get; }
    }
}