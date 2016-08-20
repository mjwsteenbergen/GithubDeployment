using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiLibs.GitHub;
using ApiLibs.General;

namespace GitHubDeployment
{
    class Downloader
    {

        public string user { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public string path { get; set; }
        public string type { get; set; }

        public Downloader(string user, string name, string version, string path, string type)
        {
            this.user = user;
            this.name = name;
            this.version = version;
            this.path = path ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            this.type = type ?? "zip";
        }

        public async Task Update()
        {
//            string applicationPath =
//                Environment.OSVersion.ToString().Contains("Unix") ?
//                @"~/.Laurentia/GithubDeployment" :
//                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Laurentia\GithubDeployment";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Release latestRelease = await GetRelease();

            string versionPath = path + Path.DirectorySeparatorChar + name + Path.DirectorySeparatorChar + "versions";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                Directory.CreateDirectory(versionPath);
            }

            string latestVersionPath = versionPath + Path.DirectorySeparatorChar + latestRelease.tag_name;

            if (!Directory.Exists(latestVersionPath))
            {
                Directory.CreateDirectory(latestVersionPath);
                path = latestVersionPath + Path.DirectorySeparatorChar;
                string zipFilename = await DownloadFile(latestRelease.zipball_url);

                ZipFile.ExtractToDirectory(zipFilename, latestVersionPath);
                Console.WriteLine("Extracting complete");


                string extractionDir = Directory.GetDirectories(latestVersionPath)[0];

                foreach (string dir in Directory.GetDirectories(extractionDir))
                {
                    DirectoryInfo info = new DirectoryInfo(dir);
                    info.MoveTo(latestVersionPath + Path.DirectorySeparatorChar + info.Name);
                }

                Console.WriteLine("Moving directories complete");

                foreach (string file in Directory.GetFiles(extractionDir))
                {
                    FileInfo info = new FileInfo(file);
                    info.MoveTo(latestVersionPath + Path.DirectorySeparatorChar + info.Name);
                }
                Console.WriteLine("Moving Files Complete");

                Directory.Delete(extractionDir);
                File.Delete(zipFilename);
                Console.WriteLine("Cleanup Complete");

            }
        }


        public async Task<Release> GetRelease()
        {

//            Passwords pass = Passwords.ReadPasswords(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Laurentia\");
            GitHubService ghs = new GitHubService();

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


        public async Task<string> DownloadFile(string url)
        {
            WebClient webClient = new WebClient();
            webClient.Headers["User-Agent"] = "myUserAgentString";
            webClient.DownloadFileCompleted += Completed;

            //Get the name of the file from the url
            string filename = Regex.Match(url, @"\/([^\/]+$)").Value.Remove(0, 1);
            try
            {
                await webClient.DownloadFileTaskAsync(new Uri(url), path + filename + "." + type);
                return path + filename + "." + type;
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
