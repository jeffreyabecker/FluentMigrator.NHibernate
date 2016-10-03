# FluentMigrator.NHibernate
A tool to generate FluentMigrator migrations from NHibernate. 

## Differences from EF Migrations
*  Entity Framework looks at its migration history table to find the most recently applied migrtation.  FluentMigrator.NHibernate uses reflection against the assembly containing the migrations. 
*  Entity Framework uses timestamps as migration numbers; FluentMigrator.NHibernate finds the last migration version in your assembly and increments.
*  You have a lot more control over the internals of how migrations are generated. FluentMigrator.NHiberate gives you an opportunity to override how the source & target expression lists are calcuated as well as an opportunity to filter expressions after the differences are calculated.  

## Usage
Right now there's no tooling for generating migrations akin to `dotnet ef migrations ...`. Right now the most effective way to use the migration generator is to inherit from `MigrationConfigurationBase`, make sure to set `MigrationNamespace` and `MigrationAssembly` in the constructor, and over ride GetConfiguration to return your NHibernate configuration. In a unit test you can then call `MyMigrationConfiguration.Generate("MigrationName","Path\To\MigrationsOutputFolder");`.

```
public class MyNHibernateConfig : NHibernate.Cfg.Configuration
{
	public MyNHibernateConfig() : this("Data Source=(local);Database=myapp;Integrated Security=True;")
	{
		
	}
	public MyNHibernateConfig(string connectionString)
	{
		this.DataBaseIntegration(db =>
		{
			db.ConnectionString = connectionString;
			db.Dialect<MsSql2012Dialect>();
		});
		var mapper = new ConventionModelMapper();
		var mapping = mapper.CompileMappingFor(FindEntities());
		AddMapping(mapping);
	}

	private IEnumerable<Type> FindEntities()
	{
		return Enumerable.Empty<Type>();
	}
}
public class MyMigrationsConfiguration : MigrationConfigurationBase
{
	public MyMigrationsConfiguration()
	{
		MigrationAssembly = typeof (MyNHibernateConfig).Assembly;
		MigrationNamespace = "MyMigrationsApp.Domain.Db.Migrations";
	}

	protected override Configuration GetConfiguration()
	{
		return new MyNHibernateConfig();
	}
}
```

