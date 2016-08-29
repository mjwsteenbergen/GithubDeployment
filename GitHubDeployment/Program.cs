using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.GitHub;
using CommandLine;

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
                            Trace.WriteLine("Starting GitHubDeployment");
                            try
                            {
//                                Updater u = new Updater("restsharp", "RestSharp");
//                                u.Install();
                                Downloader download = new Downloader(options.OwnerName, options.RepositoryName,
                                    options.Version,
                                    options.DowloadLocation,
                                    options.Type);

                                if (options.Apply)
                                {
                                    new Updater(options.OwnerName, options.RepositoryName).Apply();
                                }
                                else
                                {
                                    await download.Update();
                                }
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e.Message);
                            }

                            Trace.WriteLine("Exiting GitHubDeployment");

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
