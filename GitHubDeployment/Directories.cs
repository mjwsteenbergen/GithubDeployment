using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubDeployment
{
    class Directories
    {
        public static string Repository { get; set; }
        public static string GetBasePath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string GetApplicationPath { get { return GetBasePath + Path.DirectorySeparatorChar + Repository; } }
        public static string GetApplicationBinPath { get { return GetApplicationPath + Path.DirectorySeparatorChar + "Application"; } }
        public static string GetPackageLocation { get { return GetApplicationPath + Path.DirectorySeparatorChar + "package.json"; } }
    }
}
