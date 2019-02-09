using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;

namespace Updater
{
    /// <summary>
    /// This FTP update engine
    /// has been depreciated.
    /// </summary>
    public class FtpEngine : UpdateEngine
    {
        private const string FtpUrl = "ftp://208.113.130.91/files/Wendall/TimeApp/";
        private static readonly NetworkCredential FtpCredentials = new NetworkCredential("abcodeblocks", "Coding4HisGlory!");

        private readonly string _currentDirectory;

        public FtpEngine(string currentDirectory)
        {
            _currentDirectory = currentDirectory;
        }
        
        /// <summary>
        /// Returns list of Versions available on ftp update server.
        /// </summary>
        /// <returns></returns>
        public override List<DetailVersion> GetUpdateVersions()
        {
            var directories = new List<DetailVersion>();
            try
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest) WebRequest.Create(FtpUrl);
                ftpWebRequest.Credentials = FtpCredentials;
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse) ftpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream() ?? throw new Exception("Respose is null"));
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    directories.Add(new DetailVersion(new Version(line), ""));
                    line = streamReader.ReadLine();
                }

                streamReader.Close();
                return directories;
            }
            catch (Exception)
            {
                // do nothing
                return directories;
            }
        }

        protected virtual void DownloadUpdate(string url, string localPath)
        {
            FtpWebRequest listRequest = (FtpWebRequest) WebRequest.Create(url);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = FtpCredentials;
            var lines = new List<string>();
            using (FtpWebResponse listResponse = (FtpWebResponse) listRequest.GetResponse())
            using (Stream listStream = listResponse.GetResponseStream())
            using (StreamReader listReader = new StreamReader(listStream ?? throw new Exception("Value cannot be null!")))
            {
                while (!listReader.EndOfStream)
                {
                    lines.Add(listReader.ReadLine());
                }
            }

            foreach (string line in lines)
            {
                var tokens = line.Split(new[] {' '}, 9, StringSplitOptions.RemoveEmptyEntries);
                string name = tokens[8];
                string permissions = tokens[0];
                string localFilePath = Path.Combine(localPath, name);
                string fileUrl = url + name;
                if (permissions[0] == 'd') // this is a directory
                {
                    if (!Directory.Exists(localFilePath))
                    {
                        Directory.CreateDirectory(localFilePath);
                    }

                    foreach (string filename in Directory.GetFiles(localFilePath))
                    {
                        File.Delete(filename);
                    }

                    DownloadUpdate(fileUrl + "/", localFilePath);
                }
                else // this is a file
                {
                    FtpWebRequest downloadRequest = (FtpWebRequest) WebRequest.Create(fileUrl);
                    downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    downloadRequest.Credentials = FtpCredentials;
                    try
                    {
                        if (File.Exists(localFilePath))
                        {
                            if (Path.GetFileName(localFilePath) == "Updater.exe")
                            {
                                File.Move(Path.Combine(_currentDirectory, "Updater.exe"), Path.Combine(_currentDirectory, "Updater.exe_OLD"));       
                            }
                            else
                            {
                                File.Delete(localFilePath);    
                            }
                        }
                        using (FtpWebResponse downloadResponse = (FtpWebResponse) downloadRequest.GetResponse())
                        using (Stream sourceStream = downloadResponse.GetResponseStream())
                        using (Stream targetStream = File.Create(localFilePath))
                        {
                            byte[] buffer = new byte[10240];
                            int read;
                            while (sourceStream != null && (read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                targetStream.Write(buffer, 0, read);
                            }
                        }
                    }
                    catch (WebException e)
                    {
                        MessageBox.Show(fileUrl + " : " + ((FtpWebResponse) e.Response).StatusDescription);
                    }
                }
            }
        }

        public override void Update(Version version)
        {
            if (_currentDirectory == "" || !Directory.Exists(_currentDirectory))
            {
                throw new Exception($"Invalid directory!  {_currentDirectory}");
            }
            
            // Delete old ftp updater
            if(File.Exists(Path.Combine(_currentDirectory, "Updater.exe_OLD")))
                File.Delete(Path.Combine(_currentDirectory, "Updater.exe_OLD"));

            try
            {
                DownloadUpdate(FtpUrl + version + "/", _currentDirectory);
            }
            catch (Exception e)
            {
                throw new Exception($"Update failed! {e.Message}");
            }
        }    
    }
}