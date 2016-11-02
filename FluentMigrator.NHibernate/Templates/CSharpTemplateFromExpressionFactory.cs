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
                {typeof(CreateTableExpression),(e)=>new CreateTableExpressionTemplate { Expression = (CreateTableExpression)e} },
                {typeof(CreateForeignKeyExpression), e=> new CreateForeignKeyExpressionTemplate {Expression = (CreateForeignKeyExpression)e} },
                {typeof(CreateIndexExpression),e=>new CreateIndexExpressionTemplate {Expression = (CreateIndexExpression)e } },
                {typeof(CreateSchemaExpression), e=>new CreateSchemaExpressionTemplate {Expression = (CreateSchemaExpression)e} },
                {typeof(CreateSequenceExpression),e=> new CreateSequenceExpressionTemplate {Expression = (CreateSequenceExpression)e} },
                {typeof(CreateConstraintExpression), e=>new CreateConstraintExpressionTemplate {Expression = (CreateConstraintExpression)e} },
                {typeof(DeleteTableExpression),(e)=>new DeleteTableExpressionTemplate { Expression = (DeleteTableExpression)e} },
                {typeof(DeleteForeignKeyExpression), e=> new DeleteForeignKeyExpressionTemplate {Expression = (DeleteForeignKeyExpression)e} },
                {typeof(DeleteIndexExpression),e=>new DeleteIndexExpressionTemplate {Expression = (DeleteIndexExpression)e } },
                {typeof(DeleteSchemaExpression), e=>new DeleteSchemaExpressionTemplate {Expression = (DeleteSchemaExpression)e} },
                {typeof(DeleteSequenceExpression),e=> new DeleteSequenceExpressionTemplate {Expression = (DeleteSequenceExpression)e} },
                {typeof(DeleteConstraintExpression), e=>new DeleteConstraintExpressionTemplate {Expression = (DeleteConstraintExpression)e} },
                {typeof(ExecuteSqlStatementExpression), e=>new ExecuteSqlStatementExpressionTemplate {Expression = (ExecuteSqlStatementExpression)e} },
                {typeof(AlterColumnExpression), e=> new AlterColumnExpressionTemplate {Expression = (AlterColumnExpression)e } }
            };
            
        }
        private static Func<MigrationExpressionBase, ITemplate> GetTemplateFactory (Type type)
        {
            var prop = type.GetProperty("Expression");
            //TODO Expresionize this;
            return (MigrationExpressionBase meb) =>
            {
                var instance = Activator.CreateInstance(type);
                prop.SetValue(instance, meb);
                return (ITemplate) instance;
            };
        }
        private static bool IsAnExpressionTemplate(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (ExpressionTemplate<>);
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
