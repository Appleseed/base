namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;

    public class GraphDBsSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(GraphDBsCollection), AddItemName = "graphDB")]
        public GraphDBsCollection graphDBs
        {
            get { return (GraphDBsCollection)this[""]; }
        }
    }

    public class GraphDBsCollection : ConfigurationElementCollection, IEnumerable<GraphDBsElement>
    {
        private readonly List<GraphDBsElement> elements;

        public GraphDBsCollection()
        {
            this.elements = new List<GraphDBsElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new GraphDBsElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((GraphDBsElement)element).Name;
        }

        public new IEnumerator<GraphDBsElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }


    public class GraphDBsElement : ConfigurationElement
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

        [ConfigurationProperty("location", IsRequired = true)]
        public string Location
        {
            get { return (string)this["location"]; }
        }

        [ConfigurationProperty("uri", IsRequired = true)]
        public string Uri
        {
            get { return (string)this["uri"]; }
        }

    }


}
