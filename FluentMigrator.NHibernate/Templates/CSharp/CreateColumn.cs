using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class CreateColumnExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.CreateColumnExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
            tw.Write("Create.Column(");
            tw.Write("\"");
            tw.Write(Expression.Column.Name);
            tw.Write("\")");
            tw.Write(".OnTable(\"");
            tw.Write(Expression.TableName);
            tw.Write("\")");
            if (!String.IsNullOrWhiteSpace(Expression.SchemaName))
            {
                tw.Write(".InSchema(\"");
                tw.Write(Expression.SchemaName);
                tw.Write("\")");
            }
            ColumnDefinitionTemplate.WriteColumnValue(tw, Expression.Column);
            //Create.Column("").OnTable("").InSchema("").AsDate().NotNullable().WithDefaultValue("");
        }
    }
}

