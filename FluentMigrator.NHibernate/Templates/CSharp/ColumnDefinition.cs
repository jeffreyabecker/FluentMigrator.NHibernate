using System;
using System.Data;
using System.IO;
using FluentMigrator.Model;

namespace FluentMigrator.NHibernate.Templates.CSharp
{
    internal class ColumnDefinitionTemplate
        : ExpressionTemplate<ColumnDefinition>
    {
        public override void WriteTo(TextWriter tw)
        {
            tw.Write("\r\n");

            var col = Expression;
            var colType = Expression.Type;

            if ( !String.IsNullOrWhiteSpace(col.CustomType))
            {
                tw.Write(".AsCustom(\"");
                tw.Write(Expression.CustomType);
                tw.Write("\")");
            }
            else if (colType == DbType.AnsiString || colType == DbType.Binary || colType == DbType.Xml ||
                     colType == DbType.String)
            {
                tw.Write(".As");
                tw.Write(colType.ToString());
                tw.Write("(");
                if (col.Size == Int32.MaxValue) { tw.Write("Int32.MaxValue");}
                else { tw.Write(col.Size);}
                tw.Write(")");
            }
            else if (colType == DbType.Decimal && col.Size == 0)
            {
                tw.Write(".AsDecimal()");
            }
            else if (colType == DbType.Decimal && col.Size != 0)
            {
                tw.Write(".AsDecimal(");
                tw.Write(col.Size);
                tw.Write(",");
                tw.Write(col.Precision);
                tw.Write(")");
            } 
            else if (colType == DbType.Single)
            {
                tw.Write(".AsFloat()");
            }
            else if (colType == DbType.AnsiStringFixedLength)
            {
                tw.Write(".AsFixedLengthAnsiString(");
                tw.Write(col.Size);
                tw.Write(")");
            }
            else if (colType == DbType.StringFixedLength)
            {
                tw.Write(".AsFixedLengthString(");
                tw.Write(col.Size);
                tw.Write(")");
            }
            else
            {
                tw.Write(".As");
                tw.Write(colType.ToString());
                tw.Write("()");
            }
            if (col.IsNullable ?? true)
            {
                tw.Write(".Nullable()");
            }
            else
            {
                tw.Write(".NotNullable()");
            }

            if (col.IsIdentity)
            {
                tw.Write(".Identity()");
            }
            if (col.IsUnique)
            {
                tw.Write(".Unique()");
            }
            if (col.IsPrimaryKey)
            {
                tw.Write(".PrimaryKey()");
            }

            if (!(col.DefaultValue is ColumnDefinition.UndefinedDefaultValue) && col.DefaultValue != null)
            {
                tw.Write(".WithDefaultValue(\"");
                tw.Write(col.DefaultValue);
                tw.Write("\")");
            }
            if (!string.IsNullOrEmpty(col.ColumnDescription))
            {
                tw.Write(".WithColumnDescription(\"");
                tw.Write(col.ColumnDescription);
                tw.Write("\")");
            }
        }
    }
}