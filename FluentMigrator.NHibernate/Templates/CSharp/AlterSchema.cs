using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class AlterSchemaExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.AlterSchemaExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
        }
    }
}

