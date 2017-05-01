namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;
    public class DropBoxCloudFileIndexServiceSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(DropBoxCloudFileToIndexCollection), AddItemName = "site")]
        public DropBoxCloudFileToIndexCollection Websites
        {
            get { return (DropBoxCloudFileToIndexCollection)this[""]; }
        }
    }

    public class DropBoxCloudFileToIndexCollection : ConfigurationElementCollection, IEnumerable<DropBoxCloudFileToIndexElement>
    {
        private readonly List<DropBoxCloudFileToIndexElement> elements;

        public DropBoxCloudFileToIndexCollection()
        {
            this.elements = new List<DropBoxCloudFileToIndexElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new DropBoxCloudFileToIndexElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DropBoxCloudFileToIndexElement)element).Name;
        }

        public new IEnumerator<DropBoxCloudFileToIndexElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }

    public class DropBoxCloudFileToIndexElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("appKey", IsKey = true, IsRequired = true)]
        public string appKey
        {
            get { return (string)this["appKey"]; }

        }

        [ConfigurationProperty("appSecret", IsKey = true, IsRequired = true)]
        public string appSecret
        {
            get { return (string)this["appSecret"]; }
        }

        [ConfigurationProperty("token", IsKey = true, IsRequired = true)]
        public string token
        {
            get { return (string)this["token"]; }
        }

        [ConfigurationProperty("tokenSecret", IsKey = true, IsRequired = true)]
        public string tokenSecret
        {
            get { return (string)this["tokenSecret"]; }
        }

        
        //[ConfigurationProperty("indexPath", IsRequired = true)]
        //public string IndexPath
        //{
        //    get { return (string)this["indexPath"]; }
        //}

        [ConfigurationProperty("filesPath", IsRequired = true)]
        public string FilesPath
        {
            get { return (string)this["filesPath"]; }
        }

        [ConfigurationProperty("connectionString", IsRequired = false)]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
        }

        [ConfigurationProperty("queueName", IsRequired = true)]
        public string QueueName
        {
            get { return (string)this["queueName"]; }
        }

        [ConfigurationProperty("tableName", IsRequired = false)]
        public string TableName
        {
            get { return (string)this["tableName"]; }
        }

        [ConfigurationProperty("userAgent", IsRequired = true)]
        public string UserAgent
        {
            get { return (string)this["userAgent"]; }
        }


        

    }
}
