using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.ProjectModel;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Frameworks;

namespace dotnet_nh
{
    public class DotNetNhToolInvoker
    {
        private readonly CommandLineApplication _app;

        private CommandOption _outputOption;
        private CommandOption _buildBasePath;
        private CommandOption _frameworkOption;
        private CommandOption _runtimeOption;
        private CommandOption _configurationOption;
        private CommandOption _projectPath;
        private CommandArgument _command;

        private string Runtime { get; set; }

        public string Config { get; set; }

        private string BuildBasePath { get; set; }

        private string Output { get; set; }

        private string ProjectPath { get; set; }

        private NuGetFramework Framework { get; set; }

        public string Command { get; set; }

        public List<string> RemainingArguments { get; set; }

        private readonly Lazy<ICollection<ProjectContext>> _projectContexts;
        private readonly Lazy<ProjectDependenciesCommandFactory> _commandFactory;
        private CommandArgument _migrationTarget;
        public ICollection<ProjectContext> ProjectContexts => _projectContexts.Value;
        public ProjectDependenciesCommandFactory CommandFactory => _commandFactory.Value;
        public DotNetNhToolInvoker(string[] args)
        {
            //"dotnet-dependency-tool-invoker", "DotNet Dependency Tool Invoker", "Invokes tools declared as NuGet dependencies of a project"
            _app = new CommandLineApplication(false)
            {
                Name = "dotnet-nh",
                FullName = "DotNet NHibernate Tool",
                Description = "Invokes NHibernate subcommands"
            };
            _projectContexts = new Lazy<ICollection<ProjectContext>>(this.CreateProjectContexts);
            _commandFactory = new Lazy<ProjectDependenciesCommandFactory>(()=>this.CreateCommandFactory());
            AddDotnetBaseParameters();
            _app.OnExecute(() =>
            {
                // Locate the project and get the name and full path
                ProjectPath = _projectPath.Value();
                Output = _outputOption.Value();
                BuildBasePath = _buildBasePath.Value();
                Config = _configurationOption.Value() ?? Constants.DefaultConfiguration;
                Runtime = _runtimeOption.Value();
                if (_frameworkOption.HasValue())
                {
                    Framework = NuGetFramework.Parse(_frameworkOption.Value());
                }
                Command = _command.Value;
                RemainingArguments = _app.RemainingArguments;

                if (string.IsNullOrEmpty(ProjectPath))
                {
                    ProjectPath = Directory.GetCurrentDirectory();
                }

                return 0;
            });

            _app.Execute(args);
        }

        private ProjectDependenciesCommandFactory CreateCommandFactory()
        {
            return new ProjectDependenciesCommandFactory(
                Framework,
                Config,
                Output,
                BuildBasePath,
                ProjectContexts.First().ProjectDirectory);
        }


        protected void AddDotnetBaseParameters()
        {
            _app.HelpOption("-?|-h|--help");

            _configurationOption = _app.Option(
                "-c|--configuration <CONFIGURATION>",
                "Configuration under which to build",
                CommandOptionType.SingleValue);
            _outputOption = _app.Option(
                "-o|--output <OUTPUT_DIR>",
                "Directory in which to find the binaries to be run",
                CommandOptionType.SingleValue);
            _buildBasePath = _app.Option(
                "-b|--build-base-path <OUTPUT_DIR>",
                "Directory in which to find temporary outputs",
                CommandOptionType.SingleValue);
            _frameworkOption = _app.Option(
                "-f|--framework <FRAMEWORK>",
                "Looks for test binaries for a specific framework",
                CommandOptionType.SingleValue);
            _runtimeOption = _app.Option(
                "-r|--runtime <RUNTIME_IDENTIFIER>",
                "Look for test binaries for a for the specified runtime",
                CommandOptionType.SingleValue);
            _projectPath = _app.Option(
                "-p|--project-path <PROJECT_JSON_PATH>",
                "Path to Project.json that contains the tool dependency",
                CommandOptionType.SingleValue);
            _command = _app.Argument(
                "<COMMAND>",
                "The command to execute.");

            //migrations init -p C:\code\fmnh\ClassLibrary1
        }


        private  ICollection<ProjectContext> CreateProjectContexts()
        {
            var projectPath = ProjectPath;
            projectPath = projectPath ?? Directory.GetCurrentDirectory();

            if (!projectPath.EndsWith(Project.FileName))
            {
                projectPath = Path.Combine(projectPath, Project.FileName);
            }

            if (!File.Exists(projectPath))
            {
                throw new InvalidOperationException($"{projectPath} does not exist.");
            }

            return ProjectContext.CreateContextForEachFramework(projectPath).Where(p => Framework == null ||
                                Framework.GetShortFolderName()
                                    .Equals(p.TargetFramework.GetShortFolderName())).ToList();
        }
    }
}
