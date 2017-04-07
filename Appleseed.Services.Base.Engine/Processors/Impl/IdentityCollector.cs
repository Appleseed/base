namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    using System.Data.SqlClient;

    using Appleseed.Services.Core.Helpers;
    using Appleseed.Services.Base.Model;

    /// <summary>
    /// Get all "identities" from a table [MemberContacts] for indexing
    /// </summary>
    public class IdentityCollector : ICollectThings
    {
        private const string Sql = "SELECT TOP 10 Name, Meetup, Introduction, Developer, Mobile, Topics, Id FROM [MemberContacts] WHERE HasBeenIndexed = 0;";
        
        public IEnumerable<Model.BaseItem> CollectItems()
        {
            var collectedItems = new List<Model.IdentityItem>();
            var connectionString = ConfigurationManager.AppSettings["AppleseedConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("'AppleseedConnectionString' was not found in the appSettings of the configuration file.");
            }

            using (var reader = SqlClientHelper.GetReader(connectionString, Sql))
            {
                while (reader.Read())
                {
                    var item = new Model.IdentityItem();
                    item.DatabaseId = reader.GetInt32(6);
                    item.Name = reader.GetString(0);
                    item.Path = reader.GetString(1);
                    item.Introduction = reader.GetString(2);
                    item.Developer = reader.GetString(3);
                    item.Mobile = reader.GetString(4);
                    item.Topics = reader.GetString(5);
                    collectedItems.Add(item);
                }
            }

            return collectedItems;
        }

        public void CollectItems(List<BaseItem> items)
        {
            this.Collect(items);
        }

        List<BaseItem> ICollectThings.CollectItems()
        {
            var items = new List<BaseItem>();
            this.Collect(items);
            return items;
        }

        private void Collect(List<BaseItem> collectedItems)
        {
            var connectionString = ConfigurationManager.AppSettings["AppleseedConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("'AppleseedConnectionString' was not found in the appSettings of the configuration file.");
            }

            using (var reader = SqlClientHelper.GetReader(connectionString, Sql))
            {
                while (reader.Read())
                {
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


                    var item = new Model.IdentityItem
                                   {
                                       DatabaseId = reader.GetInt32(6),
                                       Name = reader.GetString(0),
                                       Path = reader.GetString(1),
                                       Introduction = reader.GetString(2),
                                       Developer = reader.GetString(3),
                                       Mobile = reader.GetString(4),
                                       PublishedDate = publishedDate,
                                       Topics = reader.GetString(5)
                                   };
                    collectedItems.Add(item);
                }
            }
        }

        private static string GetDbStringValue(SqlDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
        }

    }
}
