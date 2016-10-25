using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Frameworks;

namespace FluentMigrator.NHibernate.Tool
{
    public class Program
    {
        public Program(string[] args)
        {
            var app = new CommandLineApplication(false)
            {
                Name = "dotnet nh",
                FullName = "NHibernate dotnet core cli commands"
            };
            Configure(app, this);
            ResultCode = app.Execute(args);
        }

        public bool IsShowingInformation { get; set; }

        public string Configuration { get; set; }

        public string TargetProject { get; set; }
        public int ResultCode { get; set; }

        public static int Main(string[] args)
        {
            var p = new Program(args);
            return p.ResultCode;
        }

        public int Run()
        {
            if (IsShowingInformation) return 2;

            return 0;
        }

        private static void Configure(CommandLineApplication app, Program options)
        {
            var project = app.Option(
                "-p|--project <project>",
                "The target project, defaults to current directory.", CommandOptionType.SingleValue);

            var configuration = app.Option("-c|--configuration",
                "The MigrationConfigurationBase or IMigrataionConfiguration instance to load.",
                CommandOptionType.SingleValue);
            var framework = app.Option(
                "-f|--framework <framework>",
                $"Target framework to load from the startup project (defaults to the framework most compatible with {FrameworkConstants.CommonFrameworks.NetCoreApp10}).",
                CommandOptionType.SingleValue);
            var output = app.Option(
                "-o|--output <output_dir>",
                "Directory in which to find outputs", CommandOptionType.SingleValue);
            var noBuild = app.Option("--no-build", "Do not build before executing.", CommandOptionType.NoValue);
            var verbose = app.Option("--verbose", "Show verbose output", CommandOptionType.NoValue);
            app.OnExecute(() =>
            {
                options.TargetProject = project.Value();
                options.IsShowingInformation = app.IsShowingInformation;
                options.Configuration = configuration.Value();
                options.Framework = framework.HasValue()
                                        ? NuGetFramework.Parse(framework.Value())
                                        : null;
                options.BuildOutputPath = output.Value();
                options.NoBuild = noBuild.HasValue();
                options.IsVerbose = verbose.HasValue();
                options.RemainingArguments = app.RemainingArguments;

                return options.Run();
            });
        }

        public List<string> RemainingArguments { get; set; }

        public bool IsVerbose { get; set; }

        public bool NoBuild { get; set; }

        public string BuildOutputPath { get; set; }

        public NuGetFramework Framework { get; set; }
    }
}