using FluentMigrator.Expressions;

namespace FluentMigrator.NHibernate
{
    public interface ITemplateFromExpressionFactory
    {
        ITemplate GetTemplate(MigrationExpressionBase expr);
    }
}