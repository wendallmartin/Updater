using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;

namespace FTPUpdater
{
    public class FTPEngine
    {
        public static string FtpUrl = "ftp://208.113.130.91/files/Wendall/TimeApp/";
        public static NetworkCredential FtpCredentials = new NetworkCredential("abcodeblocks", "Coding4HisGlory!");

        /// <summary>
        /// Returns list of Versions available on ftp update server.
        /// </summary>
        /// <returns></returns>
        public static List<Version> GetUpdateVersions()
        {
            try
            {
                List<Version> directorys = new List<Version>();
                FtpWebRequest ftpWebRequest = (FtpWebRequest) WebRequest.Create(FtpUrl);
                ftpWebRequest.Credentials = FtpCredentials;
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse) ftpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream() ?? throw new Exception("Respose is null"));
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    directorys.Add(new Version(line));
                    line = streamReader.ReadLine();
                }

                streamReader.Close();
                return directorys;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void DownloadFtpDirectory(string url, string localPath)
        {
            FtpWebRequest listRequest = (FtpWebRequest) WebRequest.Create(url);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = FtpCredentials;
            List<string> lines = new List<string>();
            using (FtpWebResponse listResponse = (FtpWebResponse) listRequest.GetResponse())
            using (Stream listStream = listResponse.GetResponseStream())
            using (StreamReader listReader = new StreamReader(listStream ?? throw new Exception("Value cannnot be null!")))
            {
                while (!listReader.EndOfStream)
                {
                    lines.Add(listReader.ReadLine());
                }
            }

            foreach (string line in lines)
            {
                string[] tokens = line.Split(new[] {' '}, 9, StringSplitOptions.RemoveEmptyEntries);
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

                    DownloadFtpDirectory(fileUrl + "/", localFilePath);
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
                            if (Path.GetFileName(localFilePath) == "FTPUpdater.exe")
                            {
                                File.Move(Path.Combine(Program.CurrentDirectory, "FTPUpdater.exe"), Path.Combine(Program.CurrentDirectory, "FTPUpdater.exe_OLD"));       
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

        public static void Update(Version latest)
        {
            if (Program.CurrentDirectory == "" || !Directory.Exists(Program.CurrentDirectory))
            {
                MessageBox.Show("Invalid directory!");
                return;
            }
            
            // Delete old ftp updater
            if(File.Exists(Path.Combine(Program.CurrentDirectory, "FTPUpdater.exe_OLD")))
                File.Delete(Path.Combine(Program.CurrentDirectory, "FTPUpdater.exe_OLD"));

            try
            {
                DownloadFtpDirectory(FtpUrl + latest + "/", Program.CurrentDirectory);
                MessageBox.Show("Update successful!");
            }
            catch (Exception)
            {
                MessageBox.Show("Update failed!");
            }
        }
    }
}