using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubDeployment
{
    class Package
    {
        public string Version { get; set; }
        public string Install { get; set; }
        public string github_token { get; set; }

        public Package()
        {
            Version = "";
            Install = "";
        }
    }
}
