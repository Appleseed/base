namespace Appleseed.Services.Base.Engine.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Configuration section for a File Indexing Service
    /// </summary>
    public class DbFileIndexServiceSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(DbFilesToIndexCollection), AddItemName = "file")]
        public DbFilesToIndexCollection Files
        {
            get { return (DbFilesToIndexCollection)this[""]; }
        }
    }

    public class DbFilesToIndexCollection : ConfigurationElementCollection, IEnumerable<DbFileToIndexElement>
    {
        private readonly List<DbFileToIndexElement> elements;

        public DbFilesToIndexCollection()
        {
            this.elements = new List<DbFileToIndexElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new DbFileToIndexElement();
            this.elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DbFileToIndexElement)element).Name;
        }

        public new IEnumerator<DbFileToIndexElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }

    /// <summary>
    /// Configuration element defining a directory to be indexed by the File Indexing Service
    /// </summary>
    public class DbFileToIndexElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
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
