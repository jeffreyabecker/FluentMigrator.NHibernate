using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class DeleteDefaultConstraintExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteDefaultConstraintExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
        }
    }
}

