namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;


    public class IlluminatorsSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(IlluminatorsCollection), AddItemName = "illuminator")]
        public IlluminatorsCollection illuminators
        {
            get { return (IlluminatorsCollection)this[""]; }
        }
    }

    public class IlluminatorsCollection : ConfigurationElementCollection, IEnumerable<IlluminatorsElement>
    {
        private readonly List<IlluminatorsElement> elements;

        public IlluminatorsCollection()
        {
            this.elements = new List<IlluminatorsElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new IlluminatorsElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IlluminatorsElement)element).Name;
        }

        public new IEnumerator<IlluminatorsElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }


    public class IlluminatorsElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
        }

        [ConfigurationProperty("apiKey", IsRequired = true)]
        public string ApiKey
        {
            get { return (string)this["apiKey"]; }
        }

    }


}
