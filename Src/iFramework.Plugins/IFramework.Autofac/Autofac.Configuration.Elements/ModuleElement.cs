using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Autofac.Configuration.Elements
{
    public class ModuleElement : ConfigurationElement
    {
        private const string TypeAttributeName = "type";
        private const string ParametersElementName = "parameters";
        private const string PropertiesElementName = "properties";

        [ConfigurationProperty("type", IsRequired = true)]
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public string Type => (string) base["type"];

        [ConfigurationProperty("parameters", IsRequired = false)]
        public ParameterElementCollection Parameters => (ParameterElementCollection) base["parameters"];

        [ConfigurationProperty("properties", IsRequired = false)]
        public new PropertyElementCollection Properties => (PropertyElementCollection) base["properties"];
    }
}