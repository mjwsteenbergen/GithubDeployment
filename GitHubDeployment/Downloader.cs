using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
        private Package package;

        public Downloader(string user, string name, string version, Package package)
        {
            this.user = user;
            this.name = name;
            this.version = version;
            this.package = package;
        }

        public async Task DownloadUpdate()
        {
            Release latestRelease = await GetRelease();

            if (package.Version == latestRelease.tag_name)
            {
                Console.WriteLine("Latest version was already installed");
                return;
            }

            Console.WriteLine("Found new version: " + latestRelease.tag_name);

            if (!Directory.Exists(Directories.GetApplicationBinPath))
            {
                Directory.CreateDirectory(Directories.GetApplicationBinPath);
                string xOptions = " Application --recursive";
                ExecuteCommandLine("git", package.CloneUrl != "" ? package.CloneUrl + xOptions : "clone git@github.com:" + user + "/" + name + ".git" + xOptions, Directories.GetApplicationPath);
            }

            ExecuteCommandLine("git", "fetch");
            ExecuteCommandLine("git", "reset --hard "+ latestRelease.tag_name);

            
            package.Version = latestRelease.tag_name;
            package.WriteToFile();
        }

        public void ExecuteCommandLine(string command, string options, string appP = null)
        {
            Console.WriteLine(command + " " + options);
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = options,
                WorkingDirectory = appP ?? Directories.GetApplicationBinPath,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            Process p = Process.Start(psi);
            string strOutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            Console.WriteLine(strOutput);
        }


        public async Task<Release> GetRelease()
        {
            GitHubService ghs = package?.github_token == null ? new GitHubService() : new GitHubService(package.github_token);
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

            if (releases.Count == 0)
            {
                throw new KeyNotFoundException("This repository does not have any releases");
            }
            return releases[0];
        }
    }
}
