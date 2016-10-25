using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                        Task.Run(async () =>
                        {
                            Console.WriteLine("Starting GitHubDeployment");
                            try
                            {

                                Directories.Repository = options.RepositoryName;

                                Directory.CreateDirectory(Directories.GetApplicationPath);

                                if (!File.Exists(Directories.GetPackageLocation))
                                {
                                    File.WriteAllText(Directories.GetPackageLocation, JsonConvert.SerializeObject(new Package()));
                                }

                                Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText(Directories.GetPackageLocation));

                                if (options.Apply)
                                {
                                    new Updater(package).Apply();
                                }
                                else
                                {
                                    await new Downloader(options.OwnerName, options.RepositoryName, options.Version, package).DownloadUpdate();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            Console.WriteLine("Exiting GitHubDeployment");

                        }).Wait();
                        
                        return 0;
                    },
                    errors =>
                    {
                        //LogHelper.Log(errors);
                        return 1;
                    });

            
             
        }
    }
}
