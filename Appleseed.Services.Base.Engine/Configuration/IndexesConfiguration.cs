﻿
namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;


    public class IndexesSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(IndexesCollection), AddItemName = "index")]
        public IndexesCollection Indexes
        {
            get { return (IndexesCollection)this[""]; }
        }
    }

    public class IndexesCollection : ConfigurationElementCollection, IEnumerable<IndexesElement>
    {
        private readonly List<IndexesElement> elements;

        public IndexesCollection()
        {
            this.elements = new List<IndexesElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new IndexesElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IndexesElement)element).Name;
        }

        public new IEnumerator<IndexesElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }

    public class IndexesElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("location", IsRequired = true)]
        public string Location
        {
            get { return (string)this["location"]; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
        }

        [ConfigurationProperty("collectionName", IsRequired = true)]
        public string CollectionItem
        {
            get { return (string)this["collectionName"]; }
        }

    }
}

