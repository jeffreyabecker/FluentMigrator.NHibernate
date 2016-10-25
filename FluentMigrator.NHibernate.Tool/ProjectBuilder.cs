using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.Cli.Utils;

namespace FluentMigrator.NHibernate.Tool
{
    public class ProjectBuilder
    {
        public string Project { get; set; }
        public string OutputDirectory { get; set; }

        public ProjectBuilder(string project, string outputDirectory)
        {
            Project = project;
            OutputDirectory = outputDirectory;
        }

        //public bool EnsureBuilt()
        //{
        //    var args = new List<string>
        //    {
        //        projectContext.ProjectFullPath,
        //        "--configuration", projectContext.Configuration,
        //        "--framework", projectContext.TargetFramework.GetShortFolderName()
        //    };

        //    if (projectContext.TargetDirectory != null)
        //    {
        //        args.Add("--output");
        //        args.Add(projectContext.TargetDirectory);
        //    }

        //    return Command.CreateDotNet(
        //        "build",
        //        args,
        //        projectContext.TargetFramework,
        //        projectContext.Configuration);
        //}
    }
}
