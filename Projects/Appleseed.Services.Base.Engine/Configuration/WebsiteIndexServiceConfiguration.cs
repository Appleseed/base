namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Configuration section for a Website Indexing Service
    /// </summary>
    public class WebsiteIndexServiceSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(WebsitesToIndexCollection), AddItemName = "site")]
        public WebsitesToIndexCollection Websites
        {
            get { return (WebsitesToIndexCollection)this[""]; }
        }
    }

    public class WebsitesToIndexCollection : ConfigurationElementCollection, IEnumerable<WebsiteToIndexElement>
    {
        private readonly List<WebsiteToIndexElement> elements;

        public WebsitesToIndexCollection()
        {
            this.elements = new List<WebsiteToIndexElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new WebsiteToIndexElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WebsiteToIndexElement)element).Name;
        }

        public new IEnumerator<WebsiteToIndexElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }

    /// <summary>
    /// Configuration element defining a website to be indexed by the Website Indexing Service
    /// </summary>
    public class WebsiteToIndexElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("siteMapUrl", IsRequired = true)]
        public string SiteMapUrl
        {
            get { return (string)this["siteMapUrl"]; }
        }


        [ConfigurationProperty("indexPath", IsRequired = true)]
        public string IndexPath
        {
            get { return (string)this["indexPath"]; }
        }
    }
}
