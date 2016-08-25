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

        [Option('f', "format", HelpText = "Which file to take from a github release", Required = false)]
        public string Format { get; set; }

        [Option('v', "version", HelpText = "Which version to get", Required = false)]
        public string Version { get; set; }

        [Option('l', "location", HelpText = "Where to put it", Required = false)]
        public string DowloadLocation { get; set; }

        [Option('t', "type", HelpText = "The type of the file", Required = false)]
        public string Type { get; set; }

        [Option('a', "apply", HelpText = "Apply the update", Required = false)]
        public bool Apply { get; set; }

    }
}
