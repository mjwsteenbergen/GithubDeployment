using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiLibs.GitHub;
using ghdeploy;
using Mono.Options;
using Newtonsoft.Json;

namespace GitHubDeployment
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string RepositoryName = "";
            string Version = "";
            string Branch = "";
            bool NotApply = false;
            bool Install = false;
            bool Init = false;
            bool ShowHelp = false;

            // thses are the available options, not that they set the variables
            var options = new OptionSet {
                { "v|version=", "Which version to get", v => Version = v },
                { "b|branch=", "Switch to new branch", b => Branch = b },
                { "no-apply", "Do not immediately apply the update", n => NotApply = n != null },
                { "i|install", "Only runs the install script", i => Install = i != null },
                { "init", "Initialised github project in the form of user/repo", i => Init = i != null },
                { "h|help", "show this message and exit", h => ShowHelp = h != null },
            };

            List<string> extra;
            try
            {
                // parse the command line
                extra = options.Parse(args);
                RepositoryName = string.Join(' ', extra);
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write("deploy: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `deploy --help' for more information.");
                return;
            }

            if (ShowHelp)
            {
                Program.ShowHelp(options);
                return;
            }

            Console.WriteLine("Starting GitHubDeployment");
            try
            {
                if (Init)
                {
                    FirstRun(RepositoryName, Version);
                    return;
                }

                if (Equals(RepositoryName, null))
                {
                    Directories.GetApplicationPath = Environment.CurrentDirectory;

                    if (!File.Exists(Directories.GetPackageLocation))
                    {
                        Console.WriteLine("You did not supply an owner/repository and you are not in a folder which has a package.json");
                        throw new Exception();
                    }

                }
                else
                {
                    Directories.Repository = RepositoryName;

                    if (!File.Exists(Directories.GetPackageLocation))
                    {
                        Console.WriteLine("We could not find the repository you are looking for");
                        throw new Exception();
                    }
                }

                Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText(Directories.GetPackageLocation));

                Updater updater = new Updater(RepositoryName, Version, package);

                if (Install)
                {
                    updater.Install();
                    return;
                }

                updater.Fetch();
                bool needsInstall = false;

                if (!Equals(Branch, null))
                {
                    CommandLine.Run("git", " checkout " + Branch);
                    CommandLine.Run("git", "submodule update --recursive");

                    needsInstall = true;
                }

                needsInstall = needsInstall || await updater.DownloadUpdate();

                if (!NotApply && needsInstall)
                {
                    updater.Install();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

            Console.WriteLine("Exiting GitHubDeployment");

        }

        private static void FirstRun(string repositoryName, string version)
        {
            var package = new Package();


            var match = Regex.Match(repositoryName, "(.+)/(.+)");

            Directories.Repository = match.Groups[2].Value;
            package.Username = match.Groups[1].Value;

            Directory.CreateDirectory(Directories.GetApplicationPath);
            Directory.CreateDirectory(Directories.GetApplicationBinPath);

            Updater updater = new Updater(Directories.Repository, version, package);

            updater.FirstDownload();
            package.WriteToFile();
        }

        private static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("deploy: Deploying all your work easily");
            Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }
    }
}
