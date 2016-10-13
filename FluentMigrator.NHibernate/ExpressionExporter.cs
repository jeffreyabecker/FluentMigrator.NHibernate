using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using FluentMigrator.Builders.Alter.Column;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.SqlTypes;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace FluentMigrator.NHibernate
{
    public class ExpressionExporter : IEnumerable<MigrationExpressionBase>
    {
        private readonly Configuration _cfg;
        private readonly Dialect _dialect;

        public ExpressionExporter(Configuration cfg, Dialect dialect)
        {
            _cfg = cfg;
            _dialect = dialect;
            _defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, _cfg.Properties, null);
            _defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, _cfg.Properties, null);
        }


        public IEnumerator<MigrationExpressionBase> GetEnumerator()
        {


            var mapping = _cfg.BuildMapping();
            var tables = _cfg.ClassMappings.SelectMany(m => m.TableClosureIterator)
                .GroupBy(t => new {t.Schema, t.Name})
                .Select(g => g.First())
                .Where(t => t.IsPhysicalTable)
                .ToList();
            var schemas = tables.Select(x => x.Schema).Where(s => !String.IsNullOrEmpty(s)).Distinct().ToList();
            foreach (var schema in schemas)
            {
                yield return new CreateSchemaExpression
                {
                    SchemaName = schema
                };
            }
            foreach (var table in tables)
            {
                yield return new CreateTableExpression
                {
                    SchemaName = table.Schema,
                    TableName = table.Name,
                    Columns = GetTableColumns(table, mapping),
                    TableDescription = table.Comment
                };
                foreach (var p in GetUniqueKeys(table)) yield return p;

            }
            foreach (var g in GetIdGenerators()) yield return g;
            foreach (var table in tables)
            {
                foreach (var p in GetIndexes(table)) yield return p;
                foreach (var p in GetFKs(table)) yield return p;
            }

            var aux = GetPrivateField<IList<IAuxiliaryDatabaseObject>, Configuration>(_cfg, "auxiliaryDatabaseObjects")
                .Cast<AbstractAuxiliaryDatabaseObject>()
                .Select(a=>new
                {
                    Drop = a.SqlDropString(_dialect, _defaultCatalog, _defaultSchema),
                    Create = a.SqlCreateString(_dialect, mapping, _defaultCatalog, _defaultSchema)
                });
            foreach (var a in aux)
            {
                yield return new ExecuteSqlStatementExpression
                {
                    SqlStatement = a.Drop
                };
                yield return new ExecuteSqlStatementExpression
                {
                    SqlStatement = a.Create
                };
            }
        }

        private IEnumerable<MigrationExpressionBase> GetIdGenerators()
        {
            var generators = GetPersistentIdentifierGenerators(_defaultCatalog, _defaultSchema);

            IEnumerable<MigrationExpressionBase> idGenerators = GetExpressionsFor(generators);
            return idGenerators;
        }

        private IEnumerable<MigrationExpressionBase> GetExpressionsFor(List<IPersistentIdentifierGenerator> generators)
        {
            foreach (var g in generators)
            {
                var table = g as TableGenerator;
                var sequence = g as SequenceGenerator;
                if (table != null)
                {
                    var tableName = GetPrivateField<string, TableGenerator>(table, "tableName");
                    var columnName = GetPrivateField<string, TableGenerator>(table, "columnName");
                    var sqlType = GetPrivateField<SqlType, TableGenerator>(table, "columnSqlType");
                    yield return new CreateTableExpression
                    {
                        TableName = tableName,
                        Columns = new List<ColumnDefinition>
                        {
                            new ColumnDefinition {Name = columnName, Type = sqlType.DbType}
                        }
                    };
                }
                else if (sequence != null)
                {
                    yield return new CreateSequenceExpression
                    {
                        Sequence = new SequenceDefinition
                        {
                            Name = sequence.SequenceName
                        }
                    };
                }
                else
                {
                    throw new NotImplementedException(String.Format("Havent implemented deconstruction for {0}", g.GetType().FullName));
                }
                
            }
        }

        private T GetPrivateField<T, TInstance>(object instance, string name)
        {
            var fields = typeof(TInstance).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).ToArray();
            var field = fields.FirstOrDefault(x=>x.Name == name);

            return (T) (field.GetValue(instance));
        }
        private List<IPersistentIdentifierGenerator> GetPersistentIdentifierGenerators(string defaultCatalog, string defaultSchema)
        {
            //TODO: Figure out how to determine if the mapping used 'native' and generate appropriate conditionals
            var classGens = _cfg.ClassMappings.Where(pc => !pc.IsInherited)
                .Select(pc => pc.Identifier.CreateIdentifierGenerator(_dialect, defaultCatalog, defaultSchema, (RootClass) pc))
                .Where(pc=> pc is IPersistentIdentifierGenerator)
                .Cast<IPersistentIdentifierGenerator>()
                .Where(x => x != null);

            var collectionGens = _cfg.CollectionMappings.Where(c => c.IsIdentified)
                .Cast<IdentifierCollection>()
                .Select(ig => ig.Identifier.CreateIdentifierGenerator(_dialect, defaultCatalog, defaultSchema, null))
                .Cast<IPersistentIdentifierGenerator>()
                .Where(x => x != null);
            var generators = classGens.Concat(collectionGens)
                .GroupBy(g => g.GeneratorKey())
                .Select(g => g.First())
                .ToList();
            return generators;
        }

        private static IEnumerable<MigrationExpressionBase> GetUniqueKeys(Table table)
        {
            foreach (var uk in table.UniqueKeyIterator)
            {
                var indexDefinition = new IndexDefinition();
                indexDefinition.SchemaName = table.Schema;
                indexDefinition.TableName = table.Name;
                indexDefinition.Name = uk.Name;
                indexDefinition.IsUnique = true;
                indexDefinition.IsClustered = false;
                indexDefinition.Columns = uk.Columns.Select(c => new IndexColumnDefinition
                {
                    Name = c.Name,
                    Direction = Direction.Ascending
                }).ToList();
                var createIndexExpression = new CreateIndexExpression
                {
                    Index = indexDefinition
                };
                yield return createIndexExpression;
            }
        }

        private static IEnumerable<MigrationExpressionBase> GetIndexes(Table table)
        {
            return table.IndexIterator.Select(idx => CreateIndexExpression(table, idx)).Cast<MigrationExpressionBase>();
        }

        private static CreateIndexExpression CreateIndexExpression(Table table, Index idx)
        {
            var indexDefinition = new IndexDefinition();
            indexDefinition.SchemaName = table.Schema;
            indexDefinition.TableName = table.Name;
            indexDefinition.Name = idx.Name;
            indexDefinition.IsUnique = false;
            indexDefinition.IsClustered = false;
            indexDefinition.Columns = idx.ColumnIterator.Select(c => new IndexColumnDefinition
            {
                Name = c.Name,
                Direction = Direction.Ascending
            }).ToList();
            return new CreateIndexExpression
            {
                Index = indexDefinition
            };
        }

        private static IEnumerable<MigrationExpressionBase> GetFKs(Table table)
        {
            return table.ForeignKeyIterator

                .Select(fk => GetCreateForeignKeyExpression(table, fk))
                .Where(x=>x!= null)
                .Cast<MigrationExpressionBase>();
        }

        private static CreateForeignKeyExpression GetCreateForeignKeyExpression(Table table, ForeignKey fk)
        {
            if (fk.ReferencedTable == null) return null;
            var foreignKeyDefinition = new ForeignKeyDefinition();
            foreignKeyDefinition.Name = fk.Name;
            foreignKeyDefinition.PrimaryTableSchema = fk.ReferencedTable.Schema;
            foreignKeyDefinition.PrimaryTable = fk.ReferencedTable.Name;
            foreignKeyDefinition.ForeignTableSchema = table.Schema;
            foreignKeyDefinition.ForeignTable = table.Name;
            foreignKeyDefinition.ForeignColumns = fk.Columns.Select(c => c.Name).ToList();
            foreignKeyDefinition.PrimaryColumns = fk.ReferencedColumns.Count > 0? fk.ReferencedColumns.Select(c => c.Name).ToList() : fk.ReferencedTable.PrimaryKey.ColumnIterator.Select(c=>c.Name).ToList();
            foreignKeyDefinition.OnDelete = fk.CascadeDeleteEnabled ? Rule.Cascade : Rule.None;
            foreignKeyDefinition.OnUpdate = Rule.None;
            return new CreateForeignKeyExpression
            {
                ForeignKey = foreignKeyDefinition
            };
        }

        private IList<ColumnDefinition> GetTableColumns(Table table, IMapping mapping)
        {
            
            return table.ColumnIterator.Select((c,i) => ColumnDefinition(table, mapping, c, i)).ToList();
        }
        private static readonly DbType[] TypesWithLength = new DbType[] { DbType.AnsiString, DbType.Binary, DbType.Xml, DbType.String, DbType.AnsiStringFixedLength, DbType.StringFixedLength };
        private string _defaultCatalog;
        private string _defaultSchema;

        private bool IsPrimaryKey(Table table, Column c) {
            return table.HasPrimaryKey && table.PrimaryKey.ColumnIterator.Any (x => x.Equals (c));
        }

        private ColumnDefinition ColumnDefinition(Table table, IMapping mapping, Column c, int i)
        {
            var columnDefinition = new ColumnDefinition();
            columnDefinition.TableName = table.Name;
            columnDefinition.ColumnDescription = c.Comment;
            columnDefinition.CustomType = c.SqlType;
            
            columnDefinition.Name = c.Name;
            columnDefinition.Type = GetSqlType(c, mapping);
            if (columnDefinition.Type.HasValue && TypesWithLength.Contains(columnDefinition.Type.Value))
            {
                columnDefinition.Size = c.Length;
            }
            if (columnDefinition.Type == DbType.Decimal || columnDefinition.Type == DbType.Currency)
            {
                if (!(c.Precision == 19 && c.Scale == 2))
                {
                    columnDefinition.Precision = c.Precision;
                    columnDefinition.Size = c.Scale;
                }
                else
                {
                    columnDefinition.Size = 0;
                }
            }

            columnDefinition.DefaultValue = c.DefaultValue;
            columnDefinition.IsPrimaryKey = IsPrimaryKey(table, c);
            columnDefinition.IsNullable = c.IsNullable;
            columnDefinition.IsUnique = c.IsUnique;
            columnDefinition.IsIdentity = i == 0 && table.IdentifierValue!= null && table.IdentifierValue.IsIdentityColumn(_dialect);
            columnDefinition.ModificationType = ColumnModificationType.Create;
            return columnDefinition;
        }

        private DbType? GetSqlType(Column column, IMapping mapping)
        {
            var code = column.GetSqlTypeCode(mapping);
            if(code != null)
            {
                return code.DbType;
            }
            return null;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
