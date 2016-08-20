using System;
using System.Collections.Generic;
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
                            try
                            {
                                Console.WriteLine("Starting");

                                Downloader download = new Downloader(options.OwnerName, options.RepositoryName,
                                    options.Version,
                                    options.DowloadLocation,
                                    options.Type);

                                if (options.AutoUpdate)
                                {
                                    await download.Update();
                                }
                                else
                                {
                                    Release r = await download.GetRelease();
                                    await download.DownloadFile(r.zipball_url);
                                }

                                
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

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
