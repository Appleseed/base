namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;
    public class CrunchBaseApiCrawlerIndexServiceSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(CrunchBaseApiCrawlerToIndexCollection), AddItemName = "site")]
        public CrunchBaseApiCrawlerToIndexCollection Websites
        {
            get { return (CrunchBaseApiCrawlerToIndexCollection)this[""]; }
        }
    }

    public class CrunchBaseApiCrawlerToIndexCollection : ConfigurationElementCollection, IEnumerable<CrunchBaseApiCrawlerToIndexElement>
    {
        private readonly List<CrunchBaseApiCrawlerToIndexElement> elements;

        public CrunchBaseApiCrawlerToIndexCollection()
        {
            this.elements = new List<CrunchBaseApiCrawlerToIndexElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new CrunchBaseApiCrawlerToIndexElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CrunchBaseApiCrawlerToIndexElement)element).Name;
        }

        public new IEnumerator<CrunchBaseApiCrawlerToIndexElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }

    public class CrunchBaseApiCrawlerToIndexElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("apiUrl", IsRequired = true)]
        public string ApiUrl
        {
            get { return (string)this["apiUrl"]; }
        }

        [ConfigurationProperty("apiKey", IsRequired = true)]
        public string ApiKey
        {
            get { return (string)this["apiKey"]; }
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

        [ConfigurationProperty("itemsCollected", IsRequired = true)]
        public string itemsCollected
        {
            get { return (string)this["itemsCollected"]; }
        }
    }
}
