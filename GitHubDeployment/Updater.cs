using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GitHubDeployment
{
    class Updater {

        private string path;
        public Package package;

        public bool IsUnix = Environment.OSVersion.ToString().Contains("Unix");

        public void Install()
        {
            if (package.Install != "")
            {
                if (IsUnix)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "sh",
                        Arguments = package.Install,
                        WorkingDirectory = Directories.GetApplicationBinPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };

                    Process p = Process.Start(psi);
                    string strOutput = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    Console.WriteLine(strOutput);
                }
                else
                {
                    Process.Start("cmd", "/K \"cd " + Directories.GetApplicationPath + "\"" + " & start " + Directories.GetApplicationPath + Path.DirectorySeparatorChar + package.Install);
                }
            }

        }

        public void PostInstall()
        {
            
        }

        public void Apply()
        {
            Install();
            PostInstall();
        }
    }
}
