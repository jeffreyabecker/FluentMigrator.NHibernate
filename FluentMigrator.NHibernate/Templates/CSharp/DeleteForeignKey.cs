using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class DeleteForeignKeyExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteForeignKeyExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nDelete.ForeignKey(");
if(!String.IsNullOrEmpty(Expression.ForeignKey.Name)){tw.Write("\"");
tw.Write(Expression.ForeignKey.Name);
tw.Write("\"");
}tw.Write(")\r\n      .OnTable(\"");
tw.Write(Expression.ForeignKey.ForeignTable);
tw.Write("\")");

			if(!String.IsNullOrEmpty(Expression.ForeignKey.ForeignTableSchema)){tw.Write(".InSchema(\"");
tw.Write(Expression.ForeignKey.ForeignTableSchema);
tw.Write("\")");
}        }
    }
}

