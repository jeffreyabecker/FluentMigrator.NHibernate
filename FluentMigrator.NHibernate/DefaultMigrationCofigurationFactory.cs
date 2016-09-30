namespace FluentMigrator.NHibernate
{
    public class DefaultMigrationCofigurationFactory<TMigrationConfiguration> : IMigrationConfigurationFactory
        where TMigrationConfiguration: MigrationConfigurationBase, new()
    {
        public MigrationConfigurationBase CreateMigrationConfiguration()
        {
            return (MigrationConfigurationBase)new TMigrationConfiguration();

        }
    }
}