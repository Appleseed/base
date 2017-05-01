namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Configuration section for a Metadata Tagging Service
    /// </summary>
    public class TagServiceSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(SitesToTagCollection), AddItemName = "site")]
        public SitesToTagCollection Sites
        {
            get { return (SitesToTagCollection)this[""]; }
        }
    }

    public class SitesToTagCollection : ConfigurationElementCollection, IEnumerable<SiteToTagElement>
    {
        private readonly List<SiteToTagElement> elements;

        public SitesToTagCollection()
        {
            this.elements = new List<SiteToTagElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new SiteToTagElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SiteToTagElement)element).Name;
        }

        public new IEnumerator<SiteToTagElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }

    /// <summary>
    /// Configuration element defining a site to be tagged by the Metadata Tagging Service
    /// </summary>
    public class SiteToTagElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
        }

        [ConfigurationProperty("portalId", IsRequired = true)]
        public int PortalId
        {
            get { return (int)this["portalId"]; }
        }
    }
}
