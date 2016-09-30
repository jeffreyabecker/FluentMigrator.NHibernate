using FluentMigrator.Expressions;

namespace FluentMigrator.NHibernate.Templates
{
    internal interface ITemplateFromExpressionFactory
    {
        ITemplate GetTemplate(MigrationExpressionBase expr);
    }
}