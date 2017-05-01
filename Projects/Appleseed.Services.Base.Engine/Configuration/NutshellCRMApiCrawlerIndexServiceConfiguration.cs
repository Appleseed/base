namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;
    public class NutshellCRMApiCrawlerIndexServiceSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(NutshellCRMApiCrawlerToIndexCollection), AddItemName = "site")]
        public NutshellCRMApiCrawlerToIndexCollection Websites
        {
            get { return (NutshellCRMApiCrawlerToIndexCollection)this[""]; }
        }
    }

    public class NutshellCRMApiCrawlerToIndexCollection : ConfigurationElementCollection, IEnumerable<NutshellCRMApiCrawlerToIndexElement>
    {
        private readonly List<NutshellCRMApiCrawlerToIndexElement> elements;

        public NutshellCRMApiCrawlerToIndexCollection()
        {
            this.elements = new List<NutshellCRMApiCrawlerToIndexElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new NutshellCRMApiCrawlerToIndexElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NutshellCRMApiCrawlerToIndexElement)element).Name;
        }

        public new IEnumerator<NutshellCRMApiCrawlerToIndexElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }

    public class NutshellCRMApiCrawlerToIndexElement : ConfigurationElement
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

        [ConfigurationProperty("apiUserEmail", IsRequired = true)]
        public string ApiUserEmail
        {
            get { return (string)this["apiUserEmail"]; }
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
