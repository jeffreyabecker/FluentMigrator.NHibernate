namespace FluentMigrator.NHibernate
{
    public interface IMigrationConfigurationFactory
    {
        MigrationConfigurationBase CreateMigrationConfiguration();
    }
}