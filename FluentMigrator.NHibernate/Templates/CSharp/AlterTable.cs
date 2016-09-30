using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class AlterTableExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.AlterTableExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
        }
    }
}

