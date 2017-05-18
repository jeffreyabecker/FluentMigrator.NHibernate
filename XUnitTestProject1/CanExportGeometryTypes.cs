using System;
using System.Linq;
using Coordinate.Domain.Db;
using FluentMigrator.NHibernate;
using GeoAPI.Geometries;
using Xunit;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Spatial.Dialect;

namespace XUnitTestProject1
{
    public class CanExportGeometryTypes
    {
        [Fact]
        public void Do()
        {
            SqlServerGeometryLoader.LoadNativeAssemblies(@"C:\code\FluentMigrator.NHibernate\XUnitTestProject1");
            var configuration = new ExampleConfiguration();
            configuration.BuildMappings();
            Dialect dialect = Dialect.GetDialect(configuration.Properties);
           var exprs= new ExpressionExporter(configuration, dialect).ToList();
        }

        private class ExampleConfiguration : NHibernate.Cfg.Configuration
        {
            public ExampleConfiguration()
            {
                Properties["dialect"] = typeof(MsSql2012GeometryDialect).AssemblyQualifiedName;
                Properties["format_sql"] = "true";
                //This will get overwritten by the configuration system
                Properties["connection.connection_string"] =
                    "Data Source=(local);Database=nhibernate;Integrated Security=True;";
                var mm = new ModelMapper();
                mm.AddMapping(typeof(ExampleMapping));
                AddDeserializedMapping(mm.CompileMappingForAllExplicitlyAddedEntities(),"example.hbm.xml");


            }
        }

        public class ExampleGeoEntity
        {
            public virtual int Id { get; set; }
            public virtual IGeometry Shape { get; set; }
        }

        public class ExampleMapping : ClassMapping<ExampleGeoEntity>
        {
            public ExampleMapping()
            {
                Id(x=>x.Id, m=>{m.Generator(Generators.Native);});
                Property(x=>x.Shape, p =>
                {
                    p.NotNullable(true);
                    p.Type(typeof(NHibernate.Spatial.Type.MsSql2008GeometryType), new { srid = 4326, subtype="GEOMETRY" });
                    p.Column(c =>
                    {
                        c.SqlType("geometry");
                    });
                });
            }
        }
    }
}
