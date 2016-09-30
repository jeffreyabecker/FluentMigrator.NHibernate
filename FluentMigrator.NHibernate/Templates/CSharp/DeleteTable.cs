using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class DeleteTableExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteTableExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nDelete.Table(\"");
tw.Write(Expression.TableName);
tw.Write("\")");
if(!String.IsNullOrEmpty(Expression.SchemaName)){tw.Write(".InSchema(\"");
tw.Write(Expression.SchemaName);
tw.Write("\")");
}        }
    }
}

