using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ApiLibs.General;
using GitHubDeployment;

namespace ghdeploy
{
    class CommandLine
    {
        public static string Run(string command, string options = "", string appP = null, bool showInConsole = true)
        {
            if (showInConsole)
            {
                Console.WriteLine(command + " " + options);
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = options,
                WorkingDirectory = appP ?? Directories.GetApplicationBinPath,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            Process p = Process.Start(psi);
            string strOutput = "";
            if (showInConsole)
            {
                while (!p.StandardOutput.EndOfStream)
                {
                    string line = p.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                    strOutput += line;
                }
            }
            else
            {
                strOutput = p.StandardOutput.ReadToEnd();
            }
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                throw new Exception("Script returned an error");
            }
            return strOutput;
        }
    }
}
