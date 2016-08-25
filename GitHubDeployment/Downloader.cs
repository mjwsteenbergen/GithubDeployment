using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiLibs.GitHub;
using ApiLibs.General;
using Newtonsoft.Json;

namespace GitHubDeployment
{
    class Downloader
    {

        public string user { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        private Updater updater;

        public Downloader(string user, string name, string version, string path, string type)
        {
            this.user = user;
            this.name = name;
            this.version = version;
            this.type = type;
            updater = new Updater(user, name, path);

        }

        public async Task Update()
        {
            Directory.CreateDirectory(updater.GetApplicationPath);

            Release latestRelease = await GetRelease();

            

            if (updater.package.Version == latestRelease.tag_name)
            {
                return;
            }

            Directory.CreateDirectory(updater.GetUpdateLocation);

            string downloadedFilename = "";
            if (type == null)
            {
                downloadedFilename = await DownloadFile(latestRelease.zipball_url, "zip");
            }
            else
            {
                foreach (var asset in latestRelease.assets)
                {
                    if (asset.name.Contains(type))
                    {
                        downloadedFilename = await DownloadFile(asset.browser_download_url);
                        break;
                    }
                }
            }

            if ((new FileInfo(downloadedFilename)).Extension == ".zip")
            {
                ExtractFile(downloadedFilename);
            }

            updater.package.Version = latestRelease.tag_name;

            updater.Install();
            updater.PostInstall();

        }

        public void ExtractFile(string downloadedFilename)
        {

            ZipFile.ExtractToDirectory(downloadedFilename, updater.GetUpdateLocation);
            Console.WriteLine("Extracting complete");


            string extractionDir = Directory.GetDirectories(updater.GetUpdateLocation)[0];

            foreach (string dir in Directory.GetDirectories(extractionDir))
            {
                DirectoryInfo info = new DirectoryInfo(dir);
                info.MoveTo(updater.GetUpdateLocation + Path.DirectorySeparatorChar + info.Name);
            }

            Console.WriteLine("Moving directories complete");

            foreach (string file in Directory.GetFiles(extractionDir))
            {
                FileInfo info = new FileInfo(file);
                info.MoveTo(updater.GetUpdateLocation + Path.DirectorySeparatorChar + info.Name);
            }
            Console.WriteLine("Moving Files Complete");

            Directory.Delete(extractionDir);
            File.Delete(downloadedFilename);
            Console.WriteLine("Cleanup Complete");
        }


        public async Task<Release> GetRelease()
        {

//            Passwords pass = Passwords.ReadPasswords(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Laurentia\");
            GitHubService ghs = new GitHubService(updater.package.github_token);
//            var releaseasa = await ghs.GetRelease(user, name, "105.2.3");
//            var list = await ghs.GetMyRepositories();
            List<Release> releases = await ghs.GetReleases(user, name);

            Release correctRelease;

            if (version != null)
            {
                foreach (Release release in releases)
                {
                    if (release.name == version)
                    {
                        return release;
                    }
                }

                throw new KeyNotFoundException("The specified version was not found");
            }
            else
            {
                if (releases.Count == 0)
                {
                    throw new KeyNotFoundException("This repository does not have any releases");
                }
                return releases[0];
            }

        }


        public async Task<string> DownloadFile(string url, string type = null)
        {
            WebClient webClient = new WebClient();
            webClient.Headers["User-Agent"] = "myUserAgentString";
            webClient.DownloadFileCompleted += Completed;

            //Get the name of the file from the url
            string filename = Regex.Match(url, @"\/([^\/]+$)").Value.Remove(0, 1);
            try
            {
                string fileType = type == null ? "" : "." + type;
                string password = updater.package.github_token != null ? "?access_token=" + updater.package.github_token : "";
                await webClient.DownloadFileTaskAsync(new Uri(url + password), updater.GetApplicationPath + Path.DirectorySeparatorChar + filename + fileType);
                return updater.GetApplicationPath + Path.DirectorySeparatorChar + filename + "." + type;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            Console.WriteLine("Download Complete");
        }
    }
}
