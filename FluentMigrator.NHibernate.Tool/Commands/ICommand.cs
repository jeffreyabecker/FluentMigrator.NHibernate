//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;

//namespace FluentMigrator.NHibernate.Tool.Commands
//{
//    public interface ICommand
//    {
//        int Execute(ILoggerFactory loggerFactory);
//    }

//    public class AddMigration : ICommand
//    {
//        public string MigraitonsAssembly { get; set; }
//        public string MigrationsDirectory { get; set; }
//        public string MigrationConfigurationClass { get; set; }
//        public string Name { get; set; }

//        public int Execute(ILoggerFactory loggerFactory)
//        {
//            var logger = loggerFactory.CreateLogger<AddMigration>();


//        }
//    }

//    public interface ICommandFactory
//    {
        
//    }
//}
