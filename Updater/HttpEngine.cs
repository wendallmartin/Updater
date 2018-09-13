using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

namespace Downloader
{
    public class HttpEngine : UpdateEngine
    {
        private List<DetailVersion> _versions;
        private ManualResetEvent _downloadWait = new ManualResetEvent(false);
        
        public HttpEngine(){}

        public HttpEngine(string downloadDirectory, Version currentVersion, string url)
        {
            DownloadDirectory = downloadDirectory;
            CurrentVersion = currentVersion;
            Url = url;
        }
        
        /// <summary>
        /// Returns list of Versions available on ftp update server.
        /// </summary>
        /// <returns></returns>
        public override List<DetailVersion> GetUpdateVersions()
        {
            if (_versions == null)
            {
                _versions = new List<DetailVersion>();
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(Url + "/versions.xml");
                    foreach (XmlNode n in xmlDoc)
                    {
                        if (n.Name == "versions")
                        {
                            foreach (XmlNode m in n.ChildNodes)
                            {
                                try
                                {
                                    var parsedString = m.InnerText.Split('-');
                                    if (parsedString.Length > 1)
                                    {
                                        _versions.Add(new DetailVersion(new Version(parsedString[0]), parsedString[1]));
                                    }
                                }
                                catch (Exception e)
                                {
                                    // hungry catch eats exceptions
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }

            return _versions;
        }

        public override void DownloadUpdate(string url, string localPath)
        {
            if (File.Exists(localPath)) File.Delete(localPath);
            File.Create(localPath).Close();
            File.SetAttributes(localPath, FileAttributes.Normal);
            using (WebClient client = new WebClient())
            {
                client.DownloadFileCompleted += OnDownloadComplete;
                client.DownloadProgressChanged += OnDownloadChanged;
                client.DownloadFileAsync(new Uri(url), localPath);
            }
        }

        private void OnDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            _downloadWait.Set();
        }

        private void OnDownloadChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            UpdateChangedEvent?.Invoke(Math.Round(bytesIn / 1000000, 1), Math.Round(totalBytes / 1000000, 1));
        }

        public override void Update(Version version)
        {
            if (DownloadDirectory == "")
            {
                throw new Exception($"Invalid directory!  {DownloadDirectory}");
            }
            
            UpdateVersion = version;

            if (!Directory.Exists(DownloadDirectory))
            {
                Directory.CreateDirectory(DownloadDirectory);
            }

            string downloadFile = DownloadDirectory + $"/{version}.exe";
            try
            {
                DownloadUpdate($"{Url}/{version}.exe", downloadFile);
                _downloadWait.Reset();
                _downloadWait.WaitOne();
            }
            catch (Exception e)
            {
                throw new Exception("Update Failed!  " + e.Message);
            }
        }
    }
}