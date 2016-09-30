using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class CreateTableExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.CreateTableExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nCreate.Table(\"");
tw.Write(Expression.TableName);
tw.Write("\")\r\n");

if(!String.IsNullOrEmpty(Expression.SchemaName)){tw.Write(".InSchema(\"");
tw.Write(Expression.SchemaName);
tw.Write("\")");
}
tw.WriteLine();
foreach(var c in Expression.Columns){
    tw.Write(".WithColumn(\"");
tw.Write(c.Name);
tw.Write("\")");

	new ColumnDefinitionTemplate { Expression = c }.WriteTo(tw);
	tw.WriteLine();
}
if(!String.IsNullOrEmpty(Expression.TableDescription)){
	tw.Write(".WithDescription(\"");
tw.Write(Expression.TableDescription);
tw.Write("\")");

}        }
    }
}

