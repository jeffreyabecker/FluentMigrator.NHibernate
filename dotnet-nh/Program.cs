using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.ProjectModel;

namespace dotnet_nh
{
    public class Program
    {
        public static int Main(string[] args)
        {
      

            var dotnetParams = new DotNetNhToolInvoker(args);

    

            if (string.IsNullOrEmpty(dotnetParams.Command))
            {
                Console.WriteLine("A command name must be provided");

                return 1;
            }

            var prj = dotnetParams.ProjectContexts.First();
            var assemblyPath = Path.Combine(prj.ProjectDirectory, "bin", dotnetParams.Config,
                prj.TargetFramework.GetShortFolderName());
            

            foreach (var projectContext in dotnetParams.ProjectContexts)
            {
                Console.WriteLine($"Invoking '{dotnetParams.Command}' for '{projectContext.TargetFramework}'.");

                try
                {
                    var exitCode = dotnetParams.CommandFactory.Create(
                            $"dotnet-{dotnetParams.Command}",
                            dotnetParams.RemainingArguments,
                            projectContext.TargetFramework,
                            dotnetParams.Config)
                        .ForwardStdErr()
                        .ForwardStdOut()
                        .Execute()
                        .ExitCode;

                    Console.WriteLine($"Command returned {exitCode}");
                }
                catch (CommandUnknownException)
                {
                    Console.WriteLine($"Command not found");
                    return 1;
                }
            }
            return 0;
        }


    }
}
