# FluentMigrator.NHibernate
A tool to generate FluentMigrator migrations from NHibernate. 

## Differences from EF Migrations
*  Entity Framework looks at its migration history table to find the most recently applied migrtation.  FluentMigrator.NHibernate uses reflection against the assembly containing the migrations. 
*  Entity Framework uses timestamps as migration numbers; FluentMigrator.NHibernate finds the last migration version in your assembly and increments.

## Usage
Right now there's no tooling for generating migrations akin to `dotnet ef migrations ...`. Right now the most effective way to use the migration generator is to inherit from `MigrationConfigurationBase`, make sure to set `MigrationNamespace` and `MigrationAssembly` in the constructor, and over ride GetConfiguration to return your NHibernate configuration. In a unit test you can then call `MyMigrationConfiguration.Generate("MigrationName","Path\To\MigrationsOutputFolder");`.


