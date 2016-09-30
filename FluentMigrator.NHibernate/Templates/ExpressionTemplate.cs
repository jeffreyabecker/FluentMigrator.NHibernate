using System.IO;

namespace FluentMigrator.NHibernate.Templates
{
    public abstract class ExpressionTemplate<T> : ITemplate //where T: Expressions.MigrationExpressionBase
    {
        public virtual T Expression { get; set; }
        public abstract void WriteTo(TextWriter tw);
    }
}