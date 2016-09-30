using System.IO;

namespace FluentMigrator.NHibernate.Templates
{
    internal abstract class ExpressionTemplate<T> : ITemplate //where T: Expressions.MigrationExpressionBase
    {
        public virtual T Expression { get; set; }
        public abstract void WriteTo(TextWriter tw);
    }
}