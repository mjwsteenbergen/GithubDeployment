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
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "install")]
        public string Install { get; set; }

        [JsonProperty(PropertyName = "update_method")]
        public string UpdateMethod { get; set; }

        [JsonProperty(PropertyName = "github_token")]
        public string GithubToken { get; set; }

        [JsonProperty(PropertyName = "clone_url")]
        public string CloneUrl { get; set; }

        public Package()
        {
            Version = "";
            Install = "";
            CloneUrl = "";
            UpdateMethod = "tag";
            GithubToken = "";
        }

        public void WriteToFile()
        {
            File.WriteAllText(Directories.GetPackageLocation, JsonConvert.SerializeObject(this));
        }
    }
}
