namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;

    public class WebCrawlIndexServiceSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(WebCrawlToIndexCollection), AddItemName = "site")]
        public WebCrawlToIndexCollection Websites
        {
            get { return (WebCrawlToIndexCollection)this[""]; }
        }
    }

    public class WebCrawlToIndexCollection : ConfigurationElementCollection, IEnumerable<WebCrawlToIndexElement>
    {
        private readonly List<WebCrawlToIndexElement> elements;

        public WebCrawlToIndexCollection()
        {
            this.elements = new List<WebCrawlToIndexElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new WebCrawlToIndexElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WebCrawlToIndexElement)element).Name;
        }

        public new IEnumerator<WebCrawlToIndexElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }

        /// <summary>
        /// Configuration element defining a website to be indexed by the WebCrawl Indexing Service
        /// </summary>
        public class WebCrawlToIndexElement : ConfigurationElement
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

            [ConfigurationProperty("crawlDepth", IsRequired = true)]
            public int crawlDepth
            {
                get { return (int)this["crawlDepth"]; }
            }

            [ConfigurationProperty("indexPath", IsRequired = true)]
            public string IndexPath
            {
                get { return (string)this["indexPath"]; }
            }

            [ConfigurationProperty("connectionString", IsRequired = true)]
            public string ConnectionString
            {
                get { return (string)this["connectionString"]; }
            }

            [ConfigurationProperty("tableName", IsRequired = true)]
            public string TableName
            {
                get { return (string)this["tableName"]; }
            }
        }
    
}
