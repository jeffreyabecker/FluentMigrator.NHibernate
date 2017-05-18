using System;
using System.Collections.Generic;
using System.IO;
using FluentMigrator.Expressions;
using FluentMigrator.NHibernate.Templates.CSharp;

namespace FluentMigrator.NHibernate.Templates
{
    internal class CSharpTemplateFromExpressionFactory : ITemplateFromExpressionFactory
    {
        private Dictionary<Type, Func<MigrationExpressionBase, ITemplate>> _templateLookup = InitTemplates();

        private static Dictionary<Type, Func<MigrationExpressionBase, ITemplate>> InitTemplates()
        {
            return new Dictionary<Type, Func<MigrationExpressionBase, ITemplate>>
            {
                {typeof(FluentMigrator.Expressions.AlterColumnExpression),e=> new AlterColumnExpressionTemplate { Expression = (FluentMigrator.Expressions.AlterColumnExpression)e} },
                {typeof(FluentMigrator.Expressions.AlterDefaultConstraintExpression),e=> new AlterDefaultConstraint { Expression = (FluentMigrator.Expressions.AlterDefaultConstraintExpression)e} },
                {typeof(FluentMigrator.Expressions.AlterSchemaExpression),e=> new AlterSchemaExpressionTemplate { Expression = (FluentMigrator.Expressions.AlterSchemaExpression)e} },
                {typeof(FluentMigrator.Expressions.AlterTableExpression),e=> new AlterTableExpressionTemplate { Expression = (FluentMigrator.Expressions.AlterTableExpression)e} },
                {typeof(FluentMigrator.Expressions.CreateColumnExpression),e=> new CreateColumnExpressionTemplate { Expression = (FluentMigrator.Expressions.CreateColumnExpression)e} },
                {typeof(FluentMigrator.Expressions.CreateConstraintExpression),e=> new CreateConstraintExpressionTemplate { Expression = (FluentMigrator.Expressions.CreateConstraintExpression)e} },
                {typeof(FluentMigrator.Expressions.CreateForeignKeyExpression),e=> new CreateForeignKeyExpressionTemplate { Expression = (FluentMigrator.Expressions.CreateForeignKeyExpression)e} },
                {typeof(FluentMigrator.Expressions.CreateIndexExpression),e=> new CreateIndexExpressionTemplate { Expression = (FluentMigrator.Expressions.CreateIndexExpression)e} },
                {typeof(FluentMigrator.Expressions.CreateSchemaExpression),e=> new CreateSchemaExpressionTemplate { Expression = (FluentMigrator.Expressions.CreateSchemaExpression)e} },
                {typeof(FluentMigrator.Expressions.CreateSequenceExpression),e=> new CreateSequenceExpressionTemplate { Expression = (FluentMigrator.Expressions.CreateSequenceExpression)e} },
                {typeof(FluentMigrator.Expressions.CreateTableExpression),e=> new CreateTableExpressionTemplate { Expression = (FluentMigrator.Expressions.CreateTableExpression)e} },
                {typeof(FluentMigrator.Expressions.DeleteColumnExpression),e=> new DeleteColumnExpressionTemplate { Expression = (FluentMigrator.Expressions.DeleteColumnExpression)e} },
                {typeof(FluentMigrator.Expressions.DeleteConstraintExpression),e=> new DeleteConstraintExpressionTemplate { Expression = (FluentMigrator.Expressions.DeleteConstraintExpression)e} },
                {typeof(FluentMigrator.Expressions.DeleteDefaultConstraintExpression),e=> new DeleteDefaultConstraintExpressionTemplate { Expression = (FluentMigrator.Expressions.DeleteDefaultConstraintExpression)e} },
                {typeof(FluentMigrator.Expressions.DeleteForeignKeyExpression),e=> new DeleteForeignKeyExpressionTemplate { Expression = (FluentMigrator.Expressions.DeleteForeignKeyExpression)e} },
                {typeof(FluentMigrator.Expressions.DeleteIndexExpression),e=> new DeleteIndexExpressionTemplate { Expression = (FluentMigrator.Expressions.DeleteIndexExpression)e} },
                {typeof(FluentMigrator.Expressions.DeleteSchemaExpression),e=> new DeleteSchemaExpressionTemplate { Expression = (FluentMigrator.Expressions.DeleteSchemaExpression)e} },
                {typeof(FluentMigrator.Expressions.DeleteSequenceExpression),e=> new DeleteSequenceExpressionTemplate { Expression = (FluentMigrator.Expressions.DeleteSequenceExpression)e} },
                {typeof(FluentMigrator.Expressions.DeleteTableExpression),e=> new DeleteTableExpressionTemplate { Expression = (FluentMigrator.Expressions.DeleteTableExpression)e} },
                {typeof(FluentMigrator.Expressions.ExecuteSqlStatementExpression),e=> new ExecuteSqlStatementExpressionTemplate { Expression = (FluentMigrator.Expressions.ExecuteSqlStatementExpression)e} },
            };
            
        }


        public ITemplate GetTemplate(MigrationExpressionBase expr)
        {
            var expressionType = expr.GetType();
            if (_templateLookup.ContainsKey(expressionType))
            {
                return _templateLookup[expressionType](expr);
            }
            return new BadExpressionTemplate(expressionType);
        }

        private class BadExpressionTemplate : ITemplate
        {
            private readonly Type _t;

            public BadExpressionTemplate(Type t)
            {
                _t = t;
            }

            public void WriteTo(TextWriter tw)
            {
                tw.Write("throw new NotImplementedException(\"No template implemented for {0}\");", _t.FullName);
            }
        }
    }
}
