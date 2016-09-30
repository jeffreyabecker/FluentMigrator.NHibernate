using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class CreateColumnExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.CreateColumnExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
        }
    }
}

