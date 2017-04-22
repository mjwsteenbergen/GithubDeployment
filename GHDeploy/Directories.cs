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
        private static string GetBasePath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        private static string _applicationPath;
        public static string GetApplicationPath {
            get
            {
                return _applicationPath ?? GetBasePath + Path.DirectorySeparatorChar + Repository;
            }
            set
            {
                _applicationPath = value;
            }
        }
        public static string GetApplicationBinPath { get { return GetApplicationPath + Path.DirectorySeparatorChar + "Application"; } }
        public static string GetPackageLocation { get { return GetApplicationPath + Path.DirectorySeparatorChar + "package.json"; } }
    }
}
