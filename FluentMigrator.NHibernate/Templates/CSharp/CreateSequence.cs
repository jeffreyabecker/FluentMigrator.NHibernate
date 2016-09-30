using System;
using System.IO;
using FluentMigrator.Model;
namespace FluentMigrator.NHibernate.Templates.CSharp
{    internal class CreateSequenceExpressionTemplate
        : ExpressionTemplate<FluentMigrator.Expressions.CreateSequenceExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
tw.Write("\r\nCreate.Sequence(\"");
tw.Write(Expression.Sequence.Name);
tw.Write("\")");

	if(!String.IsNullOrEmpty(Expression.Sequence.SchemaName)){tw.Write(".InSchema(\"");
tw.Write(Expression.Sequence.SchemaName);
tw.Write("\")");
}tw.Write("\r\n\t");
if(Expression.Sequence.Increment.HasValue){tw.Write(".Increment(");
tw.Write(Expression.Sequence.Increment.ToString());
tw.Write(")");
}tw.Write("\r\n\t");
if(Expression.Sequence.MinValue.HasValue){tw.Write(".MinValue(");
tw.Write(Expression.Sequence.MinValue.ToString());
tw.Write(")");
}tw.Write("\r\n\t");
if(Expression.Sequence.MaxValue.HasValue){tw.Write(".MaxValue(");
tw.Write(Expression.Sequence.MaxValue.ToString());
tw.Write(")");
}tw.Write("\r\n\t");
if(Expression.Sequence.StartWith.HasValue){tw.Write(".StartWith(");
tw.Write(Expression.Sequence.StartWith.ToString());
tw.Write(")");
}tw.Write("\r\n\t");
if(Expression.Sequence.Cache.HasValue){tw.Write(".Cache(");
tw.Write(Expression.Sequence.Cache.ToString());
tw.Write(")");
}tw.Write("\r\n\t");
if(Expression.Sequence.Cycle){tw.Write(".Cycle()");
}tw.Write("\r\n\t;");
        }
    }
}

