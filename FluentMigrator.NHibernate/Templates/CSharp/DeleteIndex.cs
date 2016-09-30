using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class DeleteIndexExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteIndexExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nDelete.Index(");
if(!String.IsNullOrEmpty(Expression.Index.Name)){tw.Write("\"");
tw.Write(Expression.Index.Name);
tw.Write("\"");
}tw.Write(")\r\n      .OnTable(\"");
tw.Write(Expression.Index.TableName);
tw.Write("\")");

			if(!String.IsNullOrEmpty(Expression.Index.SchemaName)){tw.Write(".InSchema(\"");
tw.Write(Expression.Index.SchemaName);
tw.Write("\")");
}        }
    }
}

