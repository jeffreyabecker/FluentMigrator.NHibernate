using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{
    internal class AlterColumnExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.AlterColumnExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nAlter.Column(\"");
tw.Write(Expression.Column.Name);
tw.Write("\")\r\n       .OnTable(\"");
tw.Write(Expression.TableName);
tw.Write("\")\r\n       ");
if(!String.IsNullOrEmpty(Expression.SchemaName)){tw.Write(".InSchema(\"");
tw.Write(Expression.SchemaName);
tw.Write("\")");
}
new ColumnDefinitionTemplate{Expression = Expression.Column}.WriteTo(tw);        }
    }
}

