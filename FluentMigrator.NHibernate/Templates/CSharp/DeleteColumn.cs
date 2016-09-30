using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class DeleteColumnExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteColumnExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
        }
    }
}

