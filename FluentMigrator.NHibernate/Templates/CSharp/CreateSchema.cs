using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class CreateSchemaExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.CreateSchemaExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nCreate.Schema(\"");
tw.Write(Expression.SchemaName);
tw.Write("\")");
        }
    }
}

