using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class CreateIndexExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.CreateIndexExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nCreate.Index(");
if(!String.IsNullOrEmpty(Expression.Index.Name)){tw.Write("\"");
tw.Write(Expression.Index.Name);
tw.Write("\"");
}tw.Write(")\r\n      .OnTable(\"");
tw.Write(Expression.Index.TableName);
tw.Write("\")");
if(!String.IsNullOrEmpty(Expression.Index.SchemaName)){tw.Write(".InSchema(\"");
tw.Write(Expression.Index.SchemaName);
tw.Write("\")");
}tw.Write("\r\n\t  .WithOptions()");

		if(Expression.Index.IsClustered)
		{
			tw.Write(".Clustered()");

		}
		else
		{
			tw.Write(".NonClustered()");

		}
		
		if(Expression.Index.IsUnique)
		{
			tw.Write(".Unique()");

		}
		
		foreach(var c in Expression.Index.Columns)
		{tw.Write("\r\n\t  .OnColumn(\"");
tw.Write(c.Name);
tw.Write("\").");
tw.Write(c.Direction.ToString());
tw.Write("()");

		}
		tw.Write("\r\n\t\t");
        }
    }
}

