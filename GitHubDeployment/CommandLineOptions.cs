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
        [Value(0, HelpText = "User to get the files from", Required = false)]
        public string OwnerName { get; set; }

        [Value(1, HelpText = "Repository to get the files from", Required = false)]
        public string RepositoryName { get; set; }

        [Option('v', "version", HelpText = "Which version to get", Required = false)]
        public string Version { get; set; }

        [Option("no-apply", HelpText = "Do not immediately apply the update", Required = false)]
        public bool NotApply { get; set; }
    }
}
