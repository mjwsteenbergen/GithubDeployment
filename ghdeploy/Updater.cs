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
    class Updater
    {
//        public string user { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        private Package package;
        public bool IsUnix = Environment.OSVersion.ToString().Contains("Unix");

        public Updater(string name, string version, Package package)
        {
//            this.user = user;
            this.name = name;
            this.version = version;
            this.package = package;
        }

        public void FirstDownload()
        {
            string xOptions = " Application --recursive";
            ExecuteCommandLine("git", "clone git@github.com:" + package.Username + "/" + name + ".git" + xOptions, Directories.GetApplicationPath);
            package.WriteToFile();
        }

        public void Fetch()
        {
            if (!Directory.Exists(Directories.GetApplicationBinPath))
            {
                Console.WriteLine("Application sub folder does not exist. Something must have gone wrong. Exiting");
                throw new Exception();
            }

            ExecuteCommandLine("git", "fetch");
        }

        public async Task<bool> DownloadUpdate()
        {
            if (package.UpdateMethod == "tag")
            {
                Release latestRelease = await GetRelease();

                if (package.Version == latestRelease.tag_name)
                {
                    Console.WriteLine("Latest version was already installed");
                    return false;
                }

                Console.WriteLine("Found new version: " + latestRelease.tag_name);

                ExecuteCommandLine("git", "reset --hard " + latestRelease.tag_name);
                package.Version = latestRelease.tag_name;
            }
            else if (package.UpdateMethod == "pull")
            {
                if (ExecuteCommandLine("git", "pull").Contains("Already up-to-date."))
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentException("Update Method " + package.UpdateMethod + " does not exist");
            }
            
            ExecuteCommandLine("git", "submodule update --recursive");
            
            package.WriteToFile();

            return true;
        }

        public void Install()
        {
            if (package.Install != "")
            {
                if (IsUnix)
                {
                    var commanditems = package.Install.Split(' ');
                    string command = commanditems[0];
                    var arguments = command.Remove(0);

                    ExecuteCommandLine(command, arguments);
                }
                else
                {
                    Process.Start("cmd", "/K \"cd " + Directories.GetApplicationPath + "\"" + " & start " + Directories.GetApplicationPath + Path.DirectorySeparatorChar + package.Install);
                }
            }

        }

        public string ExecuteCommandLine(string command, string options, string appP = null)
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
            if (p.ExitCode != 0)
            {
                throw new Exception("Script returned an error");
            }
            return strOutput;
        }


        public async Task<Release> GetRelease()
        {
            GitHubService ghs = package?.GithubToken == null ? new GitHubService() : new GitHubService(package.GithubToken);
            List<Release> releases = await ghs.GetReleases(package?.Username, name);

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
