using System.IO;

namespace FluentMigrator.NHibernate
{
    public interface ITemplate
    {
        void WriteTo(TextWriter tw);
    }
}