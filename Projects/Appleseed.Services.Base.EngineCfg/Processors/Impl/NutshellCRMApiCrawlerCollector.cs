namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.XPath;
    using System.IO;

    using Common.Logging;

    using Appleseed.Services.Core.Extractors;
    using Appleseed.Services.Base.Model;
    using Newtonsoft.Json.Linq;
    using Appleseed.Services.Core.Helpers;
    using System.Data.SqlClient;
    public class NutshellCRMApiCrawlerCollector : ICollectThings
    {
        private ILog Logger { get; set; }

        private string connectionString { get; set; }

        private string tableName { get; set; }

        private List<Model.BaseItem> CollectedItems { get; set; }

        public NutshellCRMApiCrawlerCollector(ILog logger, string connectionString, string tableName)
        {
          
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("databaseConnectionString");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("databaseTableName");
            }

            this.tableName = tableName;
            this.Logger = logger;
            this.connectionString = connectionString;
        }

        public List<BaseItem> CollectItems()
        {
            this.CollectedItems = new List<Model.BaseItem>();
            this.CollectItems(this.CollectedItems);
            return this.CollectedItems;
        }

        public void CollectItems(List<BaseItem> items)
        {
            this.NutshellCollectionItems(items);
        }

        private const string NutshellSql = "SELECT * FROM {0} WHERE ItemKey IS NOT NULL AND ItemType IN ('{1}','{2}')"; //AND DATEDIFF(minute, ItemCreatedDate, GETDATE()) > 0
        private void NutshellCollectionItems(List<BaseItem> collectionPages)
        {
            var sql = string.Format(NutshellSql, this.tableName, CollectionItemType.NS_ACC, CollectionItemType.NS_CON);

            this.Logger.Info("NuthshellCollector: ConnectionString: " + this.connectionString);
            this.Logger.Info("NuthshellCollector: Table: " + this.tableName);

            using (var reader = SqlClientHelper.GetReader(this.connectionString, sql)) 
            {
                if (!reader.HasRows)
                {
                    this.Logger.Error("No data found in the database.");
                }

                // Get ordinals for each column
                var itemKeyIndex = reader.GetOrdinal("ItemKey");
                var itemPathIndex = reader.GetOrdinal("ItemPath");
                var itemContentIndex = reader.GetOrdinal("ItemContent");
                var itemFilePathIndex = reader.GetOrdinal("ItemFilePath");
                var itemFileNameIndex = reader.GetOrdinal("ItemFileName");
                var itemViewRolesIndex = reader.GetOrdinal("ItemViewRoles");
                var itemTypeIndex = reader.GetOrdinal("ItemType");
                

                this.Logger.Info("Items that need to be indexed");

                //TODO
                //enumerate 
                //display queried records till last entry returns false as bool for record within reader
                while (reader.Read())
                {
                    //read name column of SQL server table data
                    string viewRoles;

                    //SECURE WITH ROLES 
                    switch (GetDbStringValue(reader, itemViewRolesIndex))
                    {
                        case CollectionItemSecurityType.PUBLIC:
                            viewRoles = CollectionItemSecurityType.PUBLIC;
                            break;
                        case CollectionItemSecurityType.ADMIN:
                            viewRoles = CollectionItemSecurityType.ADMIN;
                            break;
                        case CollectionItemSecurityType.AUTHORIZED:
                            viewRoles = CollectionItemSecurityType.AUTHORIZED;
                            break;
                        default:
                            viewRoles = CollectionItemSecurityType.PUBLIC;
                            break;
                    }

                    //DETERMINE WHAT THE PATH IS

                    var itemPath = "";
                    var itemType = "";
                    switch (GetDbStringValue(reader, itemTypeIndex))
                    {
                        case CollectionItemType.NS_ACC:
                            //itemPath = GetDbStringValue(reader, itemFilePathIndex) + "\\" + GetDbStringValue(reader, itemFileNameIndex);
                            itemPath = GetDbStringValue(reader, itemPathIndex);
                            itemType = CollectionItemType.NS_ACC;
                            break;

                        case CollectionItemType.NS_CON:
                            itemPath = GetDbStringValue(reader, itemPathIndex);
                            itemType = CollectionItemType.NS_CON;
                            break;

                        // Not sure the default type
                        default:
                            itemPath = GetDbStringValue(reader, itemPathIndex);
                            itemType = CollectionItemType.NS_CON;
                            break;
                    }

                    var publishedDate = new DateTime();
                    try
                    {
                        var itemCreatedDateIndex = reader.GetOrdinal("ItemCreatedDate");
                        string dbStringValue = GetDbStringValue(reader, itemCreatedDateIndex);
                        publishedDate = string.IsNullOrEmpty(dbStringValue) ? DateTime.Now : Convert.ToDateTime(dbStringValue);
                    }
                    catch (Exception)
                    {
                    }

                    // Add items into collection
                    collectionPages.Add(
                        new AppleseedModuleItem()
                        {
                            Key = getKeyNumber(GetDbStringValue(reader, itemKeyIndex)),
                            Path = itemPath,
                            ModuleID = 0,
                            PageID = 0,
                            PortalID = 0,
                            ViewRoles = viewRoles,
                            Type = itemType,
                            PublishedDate = publishedDate,
                            Name = GetDbStringValue(reader, itemFileNameIndex),
                        });

                    this.Logger.Info("Collected Item:" + GetDbStringValue(reader, itemFileNameIndex) + " : Key=" + GetDbStringValue(reader, itemKeyIndex));
                
                }

            }
        
        }

        private static string GetDbStringValue(SqlDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
        }

        // Get the keyNumber "contact/121 --> 121"
        private static string getKeyNumber(string key) 
        {
            int location = key.IndexOf("/");
            string keyNumber = key.Substring(location+1);
            return keyNumber;
        }
       

    }
}
