using System.Configuration;

namespace Autofac.Configuration.Elements
{
    public class FileElement : ConfigurationElement
    {
        private const string FilenameAttributeName = "name";
        private const string SectionAttributeName = "section";
        internal const string Key = "name";

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => (string) base["name"];

        [ConfigurationProperty("section", IsRequired = false)]
        public string Section => (string) base["section"];
    }
}