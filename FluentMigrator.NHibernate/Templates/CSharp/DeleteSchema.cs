using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class DeleteSchemaExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteSchemaExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nDelete.Schema(\"");
tw.Write(Expression.SchemaName);
tw.Write("\")");
        }
    }
}

