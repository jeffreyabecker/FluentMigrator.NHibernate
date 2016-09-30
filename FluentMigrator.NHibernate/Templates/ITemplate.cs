using System.IO;

namespace FluentMigrator.NHibernate.Templates
{
    internal interface ITemplate
    {
        void WriteTo(TextWriter tw);
    }
}