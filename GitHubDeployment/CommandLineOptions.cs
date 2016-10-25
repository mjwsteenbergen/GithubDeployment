using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace GitHubDeployment
{
    class CommandLineOptions
    {
        [Option('u', "user", HelpText = "Where to get the files from", Required = true)]
        public string OwnerName { get; set; }

        [Option('r',"repository", HelpText = "Where to get the files from", Required = true)]
        public string RepositoryName { get; set; }

        [Option('v', "version", HelpText = "Which version to get", Required = false)]
        public string Version { get; set; }

        [Option('a', "apply", HelpText = "Apply the update", Required = false)]
        public bool Apply { get; set; }

    }
}
