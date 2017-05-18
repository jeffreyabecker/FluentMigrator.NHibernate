using System.IO;

namespace FluentMigrator.NHibernate.Templates
{
    public interface ITemplate
    {
        void WriteTo(TextWriter tw);
    }
}