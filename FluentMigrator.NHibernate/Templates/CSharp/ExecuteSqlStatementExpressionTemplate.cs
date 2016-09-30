using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FluentMigrator.NHibernate.Templates.CSharp
{
    public class ExecuteSqlStatementExpressionTemplate : ExpressionTemplate<FluentMigrator.Expressions.ExecuteSqlStatementExpression>
    {
        public override void WriteTo(TextWriter tw)
        {
            if (!String.IsNullOrWhiteSpace(Expression.SqlStatement))
            {
                var escaped = EscapeForCode(Expression.SqlStatement);
                tw.Write("\r\nExecute.Sql(\"");
                tw.Write(escaped);
                tw.Write("\");");
            }
        }

        private static readonly Dictionary<char, string> escapeMapping = new Dictionary<char, string>
        {
            {'\"', "\\\""},
            {'\\', "\\\\"},
            {'\a', @"\a"},
            {'\b', @"\b"},
            {'\f', @"\f"},
            {'\n', @"\n"},
            {'\r', @"\r"},
            {'\t', @"\t"},
            {'\v', @"\v"},
            {'\0', @"\0"}
        };

        public static string EscapeForCode(string s)
        {
            var sb = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                if (escapeMapping.ContainsKey(c))
                {
                    sb.Append(escapeMapping[c]);
                }
                else if (c > 31 && c < 127)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append("\\x").AppendFormat("{0:X4}", (int)c);
                }
            }
            return sb.ToString();
        }
    }
}