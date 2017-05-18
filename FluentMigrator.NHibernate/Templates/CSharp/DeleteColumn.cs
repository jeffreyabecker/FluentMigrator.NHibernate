using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class DeleteColumnExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteColumnExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
            foreach (var cn  in Expression.ColumnNames)
            {
                tw.Write("Delete.Column(");
                tw.Write("\"");
                tw.Write(cn);
                tw.Write("\")");
                tw.Write(".OnTable(\"");
                tw.Write(Expression.TableName);
                tw.Write("\")");
                if (!String.IsNullOrWhiteSpace(Expression.SchemaName))
                {
                    tw.Write(".InSchema(\"");
                    tw.Write(Expression.TableName);
                    tw.Write("\")");
                }
            }
        }
    }
}

