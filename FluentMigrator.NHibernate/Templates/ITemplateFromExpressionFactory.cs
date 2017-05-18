using FluentMigrator.Expressions;

namespace FluentMigrator.NHibernate.Templates
{
    public interface ITemplateFromExpressionFactory
    {
        ITemplate GetTemplate(MigrationExpressionBase expr);
    }
}