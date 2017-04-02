using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiLibs.GitHub;
using CommandLine;
using Newtonsoft.Json;

namespace GitHubDeployment
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args).MapResult(
                    options =>
                    {

                        int returnCode = 0;

                        Task.Run(async () =>
                        {
                            Console.WriteLine("Starting GitHubDeployment");
                            try
                            {
                                if (options.Init)
                                {
                                    FirstRun(options);
                                    return;
                                }

                                if (Equals(options.RepositoryName, null))
                                {
                                    Directories.GetApplicationPath = Environment.CurrentDirectory;

                                    if (!File.Exists(Directories.GetPackageLocation))
                                    {
                                        Console.WriteLine("You did not supply an owner/repository and you are not in a folder which has a package.json");
                                        returnCode = 1;
                                        return;
                                    }

                                }
                                else
                                {
                                    Directories.Repository = options.RepositoryName;

                                    if (!File.Exists(Directories.GetPackageLocation))
                                    {
                                        Console.WriteLine("We could not find the repository you are looking for");
                                        returnCode = 1;
                                        return;
                                    }
                                }

                                Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText(Directories.GetPackageLocation));

                                Updater updater = new Updater(options.RepositoryName, options.Version, package);

                                if (options.Install)
                                {
                                    updater.Install();
                                    return;
                                }

                                updater.Fetch();
                                bool needsInstall = false;

                                if (!Equals(options.Branch, null))
                                {
                                    updater.ExecuteCommandLine("git", " checkout " + options.Branch);
                                    updater.ExecuteCommandLine("git", "submodule update --recursive");

                                    needsInstall = true;
                                }

                                needsInstall = needsInstall || await updater.DownloadUpdate();

                                if (!options.NotApply && needsInstall)
                                {
                                    updater.Install();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message + "\n" + e.StackTrace);
                            }

                            Console.WriteLine("Exiting GitHubDeployment");

                        }).Wait();
                        
                        return returnCode;
                    },
                    errors =>
                    {
                        //LogHelper.Log(errors);
                        return 1;
                    });

            
             
        }

        private static void FirstRun(CommandLineOptions options)
        {
            var package = new Package();


            var match = Regex.Match(options.RepositoryName, "(.+)/(.+)");

            Directories.Repository = match.Groups[2].Value;
            package.Username = match.Groups[1].Value;

            Directory.CreateDirectory(Directories.GetApplicationPath);
            Directory.CreateDirectory(Directories.GetApplicationBinPath);

            Updater updater = new Updater(Directories.Repository, options.Version, package);

            updater.FirstDownload();
            package.WriteToFile();
        }
    }
}
