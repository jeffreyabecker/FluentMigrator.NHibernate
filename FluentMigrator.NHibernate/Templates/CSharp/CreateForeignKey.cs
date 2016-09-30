using System;
using System.IO;
using System.Linq;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class CreateForeignKeyExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.CreateForeignKeyExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nCreate.ForeignKey(");
if(!String.IsNullOrEmpty(Expression.ForeignKey.Name)){tw.Write("\"");
tw.Write(Expression.ForeignKey.Name);
tw.Write("\"");
}tw.Write(")\r\n      .FromTable(\"");
tw.Write(Expression.ForeignKey.ForeignTable);
tw.Write("\")");

			if(!String.IsNullOrEmpty(Expression.ForeignKey.ForeignTableSchema)){tw.Write(".InSchema(\"");
tw.Write(Expression.ForeignKey.ForeignTableSchema);
tw.Write("\")");
}tw.Write("\r\n\t  .ForeignColumns(");
tw.Write( String.Join(",", Expression.ForeignKey.ForeignColumns.Select(s=>"\""+s+"\"")));
tw.Write(")\r\n\t  .ToTable(\"");
tw.Write(Expression.ForeignKey.PrimaryTable);
tw.Write("\")");

			if(!String.IsNullOrEmpty(Expression.ForeignKey.PrimaryTableSchema)){tw.Write(".InSchema(\"");
tw.Write(Expression.ForeignKey.PrimaryTableSchema);
tw.Write("\")");
}tw.Write("\r\n\t  .PrimaryColumns(");
tw.Write( String.Join(",", Expression.ForeignKey.PrimaryColumns.Select(s=>"\""+s+"\"")));
tw.Write(")\r\n\t  .OnDelete(Rule.");
tw.Write(Expression.ForeignKey.OnDelete.ToString());
tw.Write(")\r\n\t  .OnUpdate(Rule.");
tw.Write(Expression.ForeignKey.OnUpdate.ToString());
tw.Write(")");
        }
    }
}

