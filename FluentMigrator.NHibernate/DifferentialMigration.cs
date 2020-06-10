using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FluentMigrator.Builders.Alter;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace FluentMigrator.NHibernate
{
    public class DifferentialMigration :  IEnumerable<MigrationExpressionBase>
    {
        private readonly List<MigrationExpressionBase> _fromSchema;
        private readonly List<MigrationExpressionBase> _toSchema;


        public DifferentialMigration(List<MigrationExpressionBase> fromSchema, List<MigrationExpressionBase> toSchema)
        {
            _fromSchema = fromSchema;
            _toSchema = toSchema;
        }

        private static bool DefaultFilter(List<MigrationExpressionBase> a, List<MigrationExpressionBase> b,
            MigrationExpressionBase c)
        {
            return true;
        }
        public IEnumerator<MigrationExpressionBase> GetEnumerator()
        {
            if (_fromSchema == null || _fromSchema.Count == 0) return _toSchema.GetEnumerator();
            return GetNewSchemas()
                .Concat(GetRemovedIndexes())
                .Concat(GetRemovedForeignKeys())
                .Concat(GetNewTables())
                .Concat(GetNewSequences())
                .Concat(GetUpdatedTables())
                .Concat(GetRemovedTables())
                .Concat(GetRemovedSequences())
                .Concat(GetNewIndexes())
                .Concat(GetNewForeignKeys())
				.Concat(GetRemovedSchemas())
                .Concat(GetAuxObjects())

                .GetEnumerator();
        }

        private IEnumerable<MigrationExpressionBase> GetAuxObjects()
        {
            return _fromSchema.OfType<ExecuteSqlStatementExpression>()
                .Concat(_toSchema.OfType<ExecuteSqlStatementExpression>())
                .Cast<MigrationExpressionBase>();
        }


        private IEnumerable<MigrationExpressionBase> GetNewSchemas()
        {
            var fromSchemata = _fromSchema.OfType<CreateSchemaExpression>().ToList();
            var toSchemata = _toSchema.OfType<CreateSchemaExpression>().ToList();
            return toSchemata.Where(ts => !fromSchemata.Any(fs => fs.SchemaName == ts.SchemaName))
                .Cast<MigrationExpressionBase>();
        }
        private IEnumerable<MigrationExpressionBase> GetRemovedIndexes()
        {
            var fromIxs = _fromSchema.OfType<CreateIndexExpression>().ToList();
            
            var toIxs = _toSchema.OfType<CreateIndexExpression>().ToList();

            return fromIxs.Where(f => !toIxs.Any(t => AreSameIndexName(f.Index, t.Index)))
                .Select(f => f.Reverse())
                .Cast<MigrationExpressionBase>();
        }



        private IEnumerable<MigrationExpressionBase> GetNewTables()
        {
            var fromTables = _fromSchema.OfType<CreateTableExpression>().ToList();
            var toTables = _toSchema.OfType<CreateTableExpression>().ToList();
            var tablesDelta = toTables.Where(t => !fromTables.Any(f =>AreSameTableName(f, t)))
                .Cast<MigrationExpressionBase>();
            var fromPks = _fromSchema.OfType<CreateConstraintExpression>()
                .Where(c => c.Constraint.IsPrimaryKeyConstraint).ToList();
            var toPks = _toSchema.OfType<CreateConstraintExpression>()
                .Where(c => c.Constraint.IsPrimaryKeyConstraint).ToList();

            var pksDelta = toPks.Where(t => !fromPks.Any(f => AreSameTableName(f, t)))
                .Cast<MigrationExpressionBase>();
            return tablesDelta.Concat(pksDelta);
            
        }

        private bool AreSameTableName(CreateConstraintExpression f, CreateConstraintExpression t)
        {
            //return fromTable.SchemaName == toTable.SchemaName && fromTable.TableName == toTable.TableName;
            return f.Constraint.SchemaName == t.Constraint.SchemaName && f.Constraint.TableName == t.Constraint.TableName && f.Constraint.ConstraintName == t.Constraint.ConstraintName;
        }

        private IEnumerable<MigrationExpressionBase> GetUpdatedTables()
        {
            var fromTables = _fromSchema.OfType<CreateTableExpression>().ToList();
            var toTables = _toSchema.OfType<CreateTableExpression>().ToList();
            var alteredTables = toTables.Select(t => new
            {
                To = t,
                From = fromTables.FirstOrDefault(f=>AreSameTableName(f,t))
            })
                .Where(x=>!AreSameTableDef(x.From,x.To))
                .SelectMany(x=>GetAlters(x.From, x.To))
                .Cast<MigrationExpressionBase>();

            var fromPks = _fromSchema.OfType<CreateConstraintExpression>()
                .Where(c => c.Constraint.IsPrimaryKeyConstraint).ToList();
            var toPks = _toSchema.OfType<CreateConstraintExpression>()
                .Where(c => c.Constraint.IsPrimaryKeyConstraint).ToList();

            var alteredPks = toPks.Select(t => new
            {
                To = t,
                From = fromPks.FirstOrDefault(f => AreSameTableName(f, t))
            })
                .Where(x => x.From != null && !AreSameKeyDef(x.From, x.To))
                .SelectMany(x=>GetAlters(x.From,x.To))
                .Cast<MigrationExpressionBase>();

            return alteredTables.Concat(alteredPks);
            ;
   
        }

        private IEnumerable<MigrationExpressionBase> GetAlters(CreateConstraintExpression from, CreateConstraintExpression to)
        {
            return new MigrationExpressionBase[]
            {
                new DeleteConstraintExpression(ConstraintType.PrimaryKey)
                {
                    Constraint = from.Constraint
                },
                to
            };
        }

        private bool AreSameKeyDef(CreateConstraintExpression @from, CreateConstraintExpression to)
        {
            return AreSameTableName(@from, to) &&
            MatchStrings(from.Constraint.Columns, to.Constraint.Columns);
        }


        private IEnumerable<MigrationExpressionBase> GetRemovedTables()
        {
            var fromTables = _fromSchema.OfType<CreateTableExpression>().ToList();
            var toTables = _toSchema.OfType<CreateTableExpression>().ToList();
            return fromTables.Where(f => !toTables.Any(t => AreSameTableName(f, t)))
                .Select(t=>t.Reverse())
               .Cast<MigrationExpressionBase>();
        }



        private IEnumerable<MigrationExpressionBase> GetRemovedForeignKeys()
        {
            var fromFks = _fromSchema.OfType<CreateForeignKeyExpression>().ToList();
            var toFks = _toSchema.OfType<CreateForeignKeyExpression>();
            return fromFks.Where(ff => !toFks.Any(tf => AreSameFk(ff.ForeignKey, tf.ForeignKey)))
                .Select(ff => ff.Reverse())
                .Cast<MigrationExpressionBase>();
        }

        private IEnumerable<MigrationExpressionBase> GetNewIndexes()
        {
            var fromIndexes = _fromSchema.OfType<CreateIndexExpression>().ToList();
            var toIndexes = _toSchema.OfType<CreateIndexExpression>().ToList();
            return toIndexes.Where(t => !fromIndexes.Any(f => AreSameIndexName(f.Index, t.Index)))
                .Cast<MigrationExpressionBase>();
        }

		private IEnumerable<MigrationExpressionBase> GetNewForeignKeys()
		{
			var fromFks = _fromSchema.OfType<CreateForeignKeyExpression>().ToList();
			var toFks = _toSchema.OfType<CreateForeignKeyExpression>().ToList();
			return toFks.Where(tf => !fromFks.Any(ff => AreSameFk(ff.ForeignKey, tf.ForeignKey)))
				.Cast<MigrationExpressionBase>();
		}

		private IEnumerable<MigrationExpressionBase> GetRemovedSchemas()
        {
            var fromSchemata = _fromSchema.OfType<CreateSchemaExpression>().ToList();
            var toSchemata = _toSchema.OfType<CreateSchemaExpression>().ToList();
            return fromSchemata.Where(fs => !toSchemata.Any(ts => ts.SchemaName == fs.SchemaName))
                .Select(fs => fs.Reverse())
                .Cast<MigrationExpressionBase>();

        }
        private IEnumerable<MigrationExpressionBase> GetNewSequences()
        {
            var toSequences = _toSchema.OfType<CreateSequenceExpression>();
            var fromSequences = _fromSchema.OfType<CreateSequenceExpression>();
            return toSequences.Where(t => !fromSequences.Any(f => HaveSameSequenceName(f, t)));
        }

        private static bool HaveSameSequenceName(CreateSequenceExpression f, CreateSequenceExpression t)
        {
            return f.Sequence.SchemaName == t.Sequence.SchemaName && f.Sequence.Name == t.Sequence.Name;
        }

        private IEnumerable<MigrationExpressionBase> GetRemovedSequences()
        {
            var toSequences = _toSchema.OfType<CreateSequenceExpression>();
            var fromSequences = _fromSchema.OfType<CreateSequenceExpression>();
            return fromSequences.Where(f => !toSequences.Any(t => HaveSameSequenceName(f, t)))
                .Select(x => x.Reverse())
                .Cast<MigrationExpressionBase>();

        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private bool AreSameTableName(CreateTableExpression fromTable, CreateTableExpression toTable)
        {
            return fromTable.SchemaName == toTable.SchemaName && fromTable.TableName == toTable.TableName;
        }
        private bool MatchIndexColumns(ICollection<IndexColumnDefinition> fromIx, ICollection<IndexColumnDefinition> toIx)
        {
            return MatchCollection(fromIx, toIx, (a, b) => a.Name == b.Name && a.Direction == b.Direction);
        }
        private bool MatchStrings(ICollection<string> from, ICollection<string> to)
        {
            return MatchCollection(from, to, (a, b) => a == b);
        }

        private bool MatchCollection<T>(ICollection<T> from, ICollection<T> to, Func<T, T, bool> comparitor)
        {
            if (from.Count != to.Count) return false;
            var toList = to.ToList();
            return from.Select((f, i) => comparitor(f, toList[i])).All(x => x);
        }
        private bool AreSameFk(ForeignKeyDefinition fromFk, ForeignKeyDefinition toFk)
        {
            return fromFk.Name == toFk.Name
                   && fromFk.ForeignTableSchema == toFk.ForeignTableSchema
                   && fromFk.ForeignTable == toFk.ForeignTable
                   && fromFk.PrimaryTable == toFk.PrimaryTable
                   && fromFk.PrimaryTableSchema == toFk.PrimaryTableSchema
                   && fromFk.OnDelete == toFk.OnDelete
                   && fromFk.OnUpdate == toFk.OnUpdate
                   && MatchStrings(fromFk.ForeignColumns, toFk.ForeignColumns)
                   && MatchStrings(fromFk.PrimaryColumns, toFk.PrimaryColumns);

        }

        //private bool MatchIncludes(ICollection<IndexIncludeDefinition> fromIx, ICollection<IndexIncludeDefinition> toIx)
        //{
        //    return MatchCollection(fromIx, toIx, (a, b) => a.Name == b.Name);
        //}

        private bool AreSameIndexName(IndexDefinition fromIx, IndexDefinition toIx)
        {
            return fromIx.SchemaName == toIx.SchemaName && fromIx.TableName == toIx.TableName
                   && fromIx.IsClustered == toIx.IsClustered && fromIx.IsUnique == toIx.IsUnique
                   && MatchIndexColumns(fromIx.Columns, toIx.Columns) /*&& MatchIncludes(fromIx.Includes, toIx.Includes)*/;
        }
        private IEnumerable<MigrationExpressionBase> GetAlters(CreateTableExpression from, CreateTableExpression to)
        {
            if (from == null || to == null) return Enumerable.Empty<MigrationExpressionBase>();

            var removedCols = from.Columns.Where(f => !to.Columns.Any(t => AreSameColumnName(f, t)))
                .Select(c => new CreateColumnExpression
                {
                    TableName = to.TableName,
                    SchemaName = to.SchemaName,
                    Column = c
                }.Reverse())
                .Cast<MigrationExpressionBase>();
            var addedCols = to.Columns.Where(t => !from.Columns.Any(f => AreSameColumnName(f, t)))
                .Select(c => new CreateColumnExpression
                {
                    TableName = to.TableName,
                    SchemaName = to.SchemaName,
                    Column = c
                }).Cast<MigrationExpressionBase>();
            var matches = from.Columns
                .Select(f => new
                {
                    From = f,
                    To = to.Columns.FirstOrDefault(t => AreSameColumnName(f, t))
                }).Where(x => !AreSameColumnDef(x.From, x.To)).ToList();
            var updatedCols = matches
                .Select(x => new AlterColumnExpression
                {
                    SchemaName = to.SchemaName,
                    TableName = to.TableName,
                    Column = x.To
                })
                .Cast<MigrationExpressionBase>();

            return addedCols.Concat(updatedCols).Concat(removedCols);

        }


        private bool AreSameColumnName(ColumnDefinition from, ColumnDefinition to)
        {
            return from.Name == to.Name;
        }

        private bool AreSameTableDef(CreateTableExpression from, CreateTableExpression to)
        {
            if (from == null || to == null || !AreSameTableName(from, to)) return false;
            return MatchCollection(from.Columns, to.Columns, AreSameColumnDef);
        }

        private bool AreSameColumnDef(ColumnDefinition a, ColumnDefinition b)
        {
            var sameName = a.Name == b.Name;
            var sameType = a.Type == b.Type;
            var sameSize = a.Size == b.Size;
            var samePrecision = a.Precision == b.Precision;
            var sameCustomType = a.CustomType == b.CustomType;
            var sameDefaultValue = a.DefaultValue == b.DefaultValue || (a.DefaultValue is FluentMigrator.Model.ColumnDefinition.UndefinedDefaultValue) && (b.DefaultValue is FluentMigrator.Model.ColumnDefinition.UndefinedDefaultValue);
            var sameIsForeignKey = a.IsForeignKey == b.IsForeignKey;
            var sameIsIdentity = a.IsIdentity == b.IsIdentity;
            var sameIsIndexed = a.IsIndexed == b.IsIndexed;
            var sameIsPrimaryKey = a.IsPrimaryKey == b.IsPrimaryKey;
            var samePrimaryKeyName = a.PrimaryKeyName == b.PrimaryKeyName;
            var sameIsNullable = a.IsNullable == b.IsNullable;
            var sameIsUnique = a.IsUnique == b.IsUnique;
            var sameTableName = a.TableName == b.TableName;
            var sameModificationType = a.ModificationType == b.ModificationType;
            var sameColumnDescription = a.ColumnDescription == b.ColumnDescription;
            var sameCollationName = a.CollationName == b.CollationName;
            return sameName && sameType && sameSize && samePrecision && sameCustomType && sameDefaultValue && sameIsForeignKey &&
            sameIsIdentity && sameIsIndexed && sameIsPrimaryKey && samePrimaryKeyName && sameIsNullable && sameIsUnique &&
            sameTableName && sameModificationType && sameColumnDescription && sameCollationName;

        }



    }
}