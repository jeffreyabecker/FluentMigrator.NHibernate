using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using FluentMigrator.Expressions;
using FluentMigrator.NHibernate.Templates;
using Newtonsoft.Json;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace FluentMigrator.NHibernate
{
    public abstract class MigrationConfigurationBase
    {
        protected string MigrationNamespace { get; set; }
        protected Assembly MigrationAssembly { get; set; }

        protected virtual string ProjectRelativeMigrationsDirectory { get { return "Db\\Migrations\\"; } }

        public virtual string GetTargetMigrationsFolder(string projectRoot)
        {
            return Path.Combine(projectRoot, (ProjectRelativeMigrationsDirectory ?? "").TrimStart('\\', '/'));
        }

        protected virtual bool FilterExpressions(List<MigrationExpressionBase> @from, List<MigrationExpressionBase> @to,
            MigrationExpressionBase expression)
        {

            return true;
        }

        public virtual GeneratedMigration Generate(string name)
        {
            var @from = GetFromExpressions();
            var @to = GetToExpressions();
            var diff = new DifferentialMigration(@from, @to)
                .Where(exp=>FilterExpressions(@from,@to,exp))
                .ToList();
            var tf = new CSharpTemplateFromExpressionFactory();
            var serializedConfiguration = SerializeConfiguration(@to);
            var version = GetVersion();
            var m = new Templates.CSharp.Migration
            {
                Expressions = diff,
                Name = name,
                Namespace = MigrationNamespace,
                SerializedConfiguration = serializedConfiguration,
                TemplateFactory = tf,
                Version = version
            };
            using (var sw = new StringWriter())
            {
                m.WriteTo(sw);
                sw.Flush();
                var result = new GeneratedMigration
                {
                    Code = sw.GetStringBuilder().ToString(),
                    Name = name,
                    Version = version,
                };
                return result;
            }

        }

        protected virtual long GetVersion()
        {
            return FindNextVersion(MigrationAssembly);
        }

        protected virtual List<MigrationExpressionBase> GetToExpressions()
        {
            var configuration = GetConfiguration();
            configuration.BuildMappings();
            Dialect dialect = Dialect.GetDialect(configuration.Properties);
            return new ExpressionExporter(configuration, dialect).ToList();
        }

        protected abstract Configuration GetConfiguration();

        protected virtual List<MigrationExpressionBase> GetFromExpressions()
        {
            return GetFromExpressionList(MigrationAssembly);
        }

        public static string SerializeConfiguration(List<MigrationExpressionBase> expressions)
        {
            var ms = new MemoryStream();
            using (var gs = new GZipStream(ms, CompressionLevel.Optimal, true))
            using (var sw = new StreamWriter(gs))
            {
                sw.Write(JsonConvert.SerializeObject(expressions, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                }));
                sw.Flush();
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public static List<MigrationExpressionBase> DeserializeConfiguration(string source)
        {
            var bytes = Convert.FromBase64String(source);
            var ms = new MemoryStream(bytes);
            using (var gs = new GZipStream(ms, CompressionMode.Decompress))
            using (var sr = new StreamReader(gs))
            {
                var src = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<List<MigrationExpressionBase>>(src, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                });
            }
        }

        public static List<MigrationExpressionBase> GetFromExpressionList(Assembly migrationsAssembly)
        {
            var lastMigration = migrationsAssembly.ExportedTypes.Where(t => t.BaseType == typeof (Migration))
                .Where(s => HasConfigurationData(s))
                .OrderBy(t => GetVersion(t))
                .LastOrDefault();

            if (lastMigration == null)
            {
                return new List<MigrationExpressionBase>();
            }
            var f = lastMigration.GetField("ConfigurationData", BindingFlags.Public | BindingFlags.Static);
            var data = (string) f.GetValue(null);
            return DeserializeConfiguration(data);
        }

        private static long GetVersion(Type type)
        {
            return type.GetCustomAttributes(false)
                .OfType<MigrationAttribute>()
                .Select(x => x.Version)
                .FirstOrDefault();
        }

        private static bool HasConfigurationData(Type type)
        {
            return type.GetField("ConfigurationData", BindingFlags.Public | BindingFlags.Static) != null;
        }

        public static long FindNextVersion(Assembly migrationsAssembly)
        {
            var lastMigration = migrationsAssembly.ExportedTypes.Where(t => t.BaseType == typeof (Migration))
                .Where(s => HasConfigurationData(s))
                .Select(t => GetVersion(t))
                .OrderBy(x => x)
                .LastOrDefault();
            return lastMigration + 1;
        }
    }
}