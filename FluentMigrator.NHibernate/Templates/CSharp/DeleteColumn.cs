using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class DeleteColumnExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteColumnExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
        }
    }
}

