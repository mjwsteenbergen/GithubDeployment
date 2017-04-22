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
        [Value(0, HelpText = "", Required = false)]
        public string RepositoryName { get; set; }

        [Option('v', "version", HelpText = "Which version to get", Required = false)]
        public string Version { get; set; }

        [Option('b', "branch", HelpText = "Switch to new branch", Required = false)]
        public string Branch { get; set; }

        [Option("no-apply", HelpText = "Do not immediately apply the update", Required = false)]
        public bool NotApply { get; set; }

        [Option("install", HelpText = "Only runs the install script")]
        public bool Install { get; set; }

        [Option("init", HelpText = "Only runs the install script")]
        public bool Init { get; set; }
    }
}
