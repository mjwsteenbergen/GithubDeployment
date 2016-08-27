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
    class Updater
    {
        private string user;
        private string repository;
        private string path;

        public Package package;

        public string GetBasePath { get
        {
            return path ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        } }
        public string GetApplicationPath { get { return GetBasePath + Path.DirectorySeparatorChar + repository; } }
        public string GetCurrrentApplicationPath { get { return GetApplicationPath + Path.DirectorySeparatorChar + "current"; }  }
        public string GetOutdatedApplicationPath { get { return GetApplicationPath + Path.DirectorySeparatorChar + "old"; } }
        public string GetUpdateLocation { get { return GetApplicationPath + Path.DirectorySeparatorChar + "new";  } }
        public string GetPackageLocation { get { return GetApplicationPath + Path.DirectorySeparatorChar + "package.json";  } }

        public bool IsUnix = Environment.OSVersion.ToString().Contains("Unix");

        public Updater(string user, string repository, string path = null)
        {
            this.user = user;
            this.repository = repository;
            this.path = path;

            Directory.CreateDirectory(GetApplicationPath);

            if (!File.Exists(GetPackageLocation))
            {
                File.WriteAllText(GetPackageLocation, JsonConvert.SerializeObject(new Package()));
            }

            package = JsonConvert.DeserializeObject<Package>(File.ReadAllText(GetPackageLocation));
        }

        public void Install()
        {
            if (package.Install != "")
            {
                if (IsUnix)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    
                    psi.FileName = "sh";
					psi.Arguments = package.Install;
					psi.WorkingDirectory = GetApplicationPath;
                    psi.UseShellExecute = false;
                    psi.RedirectStandardOutput = true;

                    Process p = Process.Start(psi);
                    string strOutput = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    Console.WriteLine(strOutput);
                }
                else
                {
                    Process.Start("cmd", "/K \"cd " + GetApplicationPath + "\"" + " & start " + GetApplicationPath + Path.DirectorySeparatorChar + package.Install);
                }
            }

        }

        public void Apply()
        {
            if (Directory.Exists(GetUpdateLocation))
            {
                if (Directory.Exists(GetOutdatedApplicationPath))
                {
                    DirectoryInfo oldDirectoryInfo = new DirectoryInfo(GetOutdatedApplicationPath);
                    oldDirectoryInfo.Delete(true);
                }

                if (Directory.Exists(GetCurrrentApplicationPath))
                {
                    DirectoryInfo info = new DirectoryInfo(GetCurrrentApplicationPath);
                    info.MoveTo(GetOutdatedApplicationPath);
                }

                DirectoryInfo updateInfo =  new DirectoryInfo(GetUpdateLocation);
                updateInfo.MoveTo(GetCurrrentApplicationPath);
            }
        }

        public void PostInstall()
        {
            File.WriteAllText(GetPackageLocation, JsonConvert.SerializeObject(package));
        }
    }
}
