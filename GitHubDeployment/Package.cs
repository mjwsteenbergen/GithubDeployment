using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GitHubDeployment
{
    class Package
    {
        public string Version { get; set; }
        public string Install { get; set; }
        public string github_token { get; set; }

        [JsonProperty(PropertyName = "clone_url")]
        public string CloneUrl { get; set; }

        public Package()
        {
            Version = "";
            Install = "";
            CloneUrl = "";
        }

        public void WriteToFile()
        {
            File.WriteAllText(Directories.GetPackageLocation, JsonConvert.SerializeObject(this));
        }
    }
}
