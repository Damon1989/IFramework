using System.Configuration;

namespace Autofac.Configuration.Elements
{
    public class ListItemElement : ConfigurationElement
    {
        private const string ValueAttributeName = "value";
        private const string KeyAttributeName = "key";

        [ConfigurationProperty("key", IsRequired = false)]
        public string Key => (string) base["key"];

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value => (string) base["value"];
    }
}