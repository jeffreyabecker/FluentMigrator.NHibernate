using System.Collections.Generic;
using System.Linq;
using FluentMigrator.NHibernate;
using GeoAPI.Geometries;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Spatial.Dialect;
using Xunit;

namespace XUnitTestProject1
{
    public class CanExportValueCollections
    {
        [Fact]
        public void Do()
        {
            var configuration = new ExampleConfiguration();
            configuration.BuildMappings();
            
            Dialect dialect = Dialect.GetDialect(configuration.Properties);

            var exprs = new ExpressionExporter(configuration, dialect).ToList();
        }
        private class ExampleConfiguration : NHibernate.Cfg.Configuration
        {
            public ExampleConfiguration()
            {
                Properties["dialect"] = typeof(MsSql2012Dialect).AssemblyQualifiedName;
                Properties["format_sql"] = "true";
                //This will get overwritten by the configuration system
                Properties["connection.connection_string"] =
                    "Data Source=(local);Database=nhibernate;Integrated Security=True;";
                var mm = new ModelMapper();
                mm.AddMapping(typeof(ExampleMapping));
                mm.AddMapping(typeof(ExampleChildMapping));
                var hbm = mm.CompileMappingForAllExplicitlyAddedEntities();
                var txt = hbm.AsString();
                AddDeserializedMapping(hbm, "example.hbm.xml");


            }
        }

        public class ExampleChild
        {
            public virtual int Id { get; set; }
            public virtual ExampleEntity Ent { get; set; }
        }
        public class ExampleEntity
        {
            public ExampleEntity()
            {
                Ints = new HashSet<int>();
            }
            public virtual int Id { get; set; }
            public virtual ISet<int> Ints { get; set; }
            public virtual ISet<ExampleChild> Children { get; set; }
        }

        public class ExampleChildMapping : ClassMapping<ExampleChild>
        {
            public ExampleChildMapping()
            {
                Table("ExampleChildMapping");
                Id(x=>x.Id, i=>i.Generator(Generators.Native));
                ManyToOne(x => x.Ent, m => { m.Column("ExampleId"); });
            }
        }
        public class ExampleMapping : ClassMapping<ExampleEntity>
        {
            public ExampleMapping()
            {
                Table("ExampleMapping");
                Id(x => x.Id, m => { m.Generator(Generators.Native); });
                Set(x => x.Ints, s =>
                {
                    s.Table("ExampleInts");

                    s.Key(k => { k.Column("ExampleId"); });
                }, s =>
                {
                    s.Element(e => { e.Column("`Value`"); e.Type(NHibernateUtil.Int32);});
                });
                Set(x=>x.Children, s =>
                {
                    s.Table("ExampleChildMapping");
                    s.Key(k=>{k.Column("ExampleId");});
                },s=>s.OneToMany(a=>a.Class(typeof(ExampleChild))));
            }
        }
    }
}