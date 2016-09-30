using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    public class DeleteSequenceExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.DeleteSequenceExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nDelete.Sequence(\"");
tw.Write(Expression.SequenceName);
tw.Write("\")");

	if(!String.IsNullOrEmpty(Expression.SchemaName)){tw.Write(".InSchema(\"");
tw.Write(Expression.SchemaName);
tw.Write("\")");
}        }
    }
}

