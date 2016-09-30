using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class DeleteDefaultConstraintExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteDefaultConstraintExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
        }
    }
}

