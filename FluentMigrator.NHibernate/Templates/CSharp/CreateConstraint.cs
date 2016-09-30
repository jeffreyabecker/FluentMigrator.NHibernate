using System;
using System.IO;
using System.Linq;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class CreateConstraintExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.CreateConstraintExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\n");
if(Expression.Constraint.IsPrimaryKeyConstraint){tw.Write("\r\n\tCreate.PrimaryKey(");
if(!String.IsNullOrEmpty(Expression.Constraint.ConstraintName)){tw.Write("\"");
tw.Write(Expression.Constraint.ConstraintName);
tw.Write("\"");
}tw.Write(")\r\n\t      .OnTable(\"");
tw.Write(Expression.Constraint.TableName);
tw.Write("\")\r\n\t\t  ");
if(!String.IsNullOrEmpty(Expression.Constraint.SchemaName)){tw.Write(".WithSchema(\"");
tw.Write(Expression.Constraint.SchemaName);
tw.Write("\")");
}tw.Write("\r\n");
}else{tw.Write("\r\n\tCreate.UniqueConstraint(");
if(!String.IsNullOrEmpty(Expression.Constraint.ConstraintName)){tw.Write("\"");
tw.Write(Expression.Constraint.ConstraintName);
tw.Write("\"");
}tw.Write(")\r\n\t      .OnTable(\"");
tw.Write(Expression.Constraint.TableName);
tw.Write("\")\r\n\t\t  ");
if(!String.IsNullOrEmpty(Expression.Constraint.SchemaName)){tw.Write(".WithSchema(\"");
tw.Write(Expression.Constraint.SchemaName);
tw.Write("\")");
}tw.Write("\r\n");
}tw.Write("\r\n.Columns(");
tw.Write( String.Join(", ", Expression.Constraint.Columns.Select(c=>"\""+c+"\"")));
tw.Write(")");
        }
    }
}

