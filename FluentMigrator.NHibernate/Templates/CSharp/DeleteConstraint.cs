using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class DeleteConstraintExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteConstraintExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\n");
if(Expression.Constraint.IsPrimaryKeyConstraint){tw.Write("\r\n\tDelete.PrimaryKey(\"");
tw.Write(Expression.Constraint.ConstraintName);
tw.Write("\")\r\n\t      .FromTable(\"");
tw.Write(Expression.Constraint.TableName);
tw.Write("\")\r\n\t\t  ");
if(!String.IsNullOrEmpty(Expression.Constraint.SchemaName)){tw.Write(".InSchema(\"");
tw.Write(Expression.Constraint.SchemaName);
tw.Write("\")");
}tw.Write("\r\n");
}else{tw.Write("\r\n\tCreate.UniqueConstraint(\"");
tw.Write(Expression.Constraint.ConstraintName);
tw.Write("\")\r\n\t      .FromTable(\"");
tw.Write(Expression.Constraint.TableName);
tw.Write("\")\r\n\t\t  ");
if(!String.IsNullOrEmpty(Expression.Constraint.SchemaName)){tw.Write(".InSchema(\"");
tw.Write(Expression.Constraint.SchemaName);
tw.Write("\")");
}tw.Write("\r\n");
}        }
    }
}

