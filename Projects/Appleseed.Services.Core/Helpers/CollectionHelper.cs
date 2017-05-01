namespace Appleseed.Services.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Appleseed.Services.Base.Model;
    using System.Data;
    using System.Data.SqlClient;

    public static class CollectionHelper
    {

        public static void AggregateItem(AppleseedModuleItem item, string connectionString, string collectionTable)
        {
            //TODO: rename to Queue_AggregateItem_Transmit?

            //TODO: replace with ORM or a Repository pattern?

            //TODO: replace with Stored procedure? Or Keep in SQL to work with other DBs?

            string Sql = @"INSERT INTO [tableName] (
		                                ItemPortalID,
                                        ItemKey,
		                                ItemType, 
		                                ItemPath, 
		                                ItemFileName, 
		                                ItemCreatedDate, 
		                                ItemModifiedDate,
		                                ItemViewRoles,
		                                ItemFileSize
	                                ) 
                                VALUES 
	                                (
		                                @ItemPortalID,
                                        @ItemKey, 
		                                @ItemType,
		                                @ItemPath,
		                                @ItemFileName,
		                                @ItemCreatedDate, 
		                                @ItemModifiedDate,
		                                @ItemViewRoles,
		                                @ItemFileSize
	                                )";

            Sql = Sql.Replace("tableName", collectionTable);

            //TODO: given information , put into cache collection 

            var parameters = new System.Data.SqlClient.SqlParameter[9];
            parameters[0] = new SqlParameter("@ItemPortalID", item.PortalID);
            parameters[1] = new SqlParameter("@ItemKey", item.Key);
            parameters[2] = new SqlParameter("@ItemType", item.Type);
            parameters[3] = new SqlParameter("@ItemPath", item.Path);
            parameters[4] = new SqlParameter("@ItemFileName", item.Name);
            parameters[5] = new SqlParameter("@ItemCreatedDate", DateTime.Now);
            parameters[6] = new SqlParameter("@ItemModifiedDate", DateTime.Now);
            parameters[7] = new SqlParameter("@ItemViewRoles", item.ViewRoles);
            parameters[8] = new SqlParameter("@ItemFileSize", item.FileSize);


            SqlClientHelper.ExecuteNonQuery(connectionString, CommandType.Text, Sql, parameters);

        }


        public static void Queue_ItemIndex_Receive(AppleseedModuleItemIndex itemIndex, string connectionString, string collectionTable)
        {
            //TODO: implement - maybe break this into batches as a parameter?
            //TODO: replace with ORM or a Repository pattern?
        }

        public static void Queue_ItemIndex_Transmit(AppleseedModuleItemIndex itemIndex, string connectionString, string collectionTable)
        {
            //TODO: replace with ORM or a Repository pattern?
            //TODO: replace with Stored procedure? Or Keep in SQL to work with other DBs?

            string Sql = @"INSERT INTO [tableName] (
		                                ItemPortalID,
                                        ItemKey,
		                                ItemType, 
		                                ItemPath,
                                        ItemContent,
                                        ItemSummary,
                                        ItemFilePath,
		                                ItemFileName, 
		                                ItemCreatedDate, 
		                                ItemModifiedDate,
		                                ItemViewRoles,
                                        ItemMeta,
		                                ItemFileSize
	                                ) 
                                VALUES 
	                                (
		                                @ItemPortalID,
                                        @ItemKey, 
		                                @ItemType,
		                                @ItemPath,
                                        @ItemContent,
                                        @ItemSummary,
                                        @ItemFilePath,
		                                @ItemFileName,
		                                @ItemCreatedDate, 
		                                @ItemModifiedDate,
		                                @ItemViewRoles,
                                        @ItemMeta,
		                                @ItemFileSize
	                                )";

            Sql = Sql.Replace("tableName", collectionTable);

            //TODO: given information , put into cache collection 

            var parameters = new System.Data.SqlClient.SqlParameter[9];
            parameters[0] = new SqlParameter("@ItemPortalID", itemIndex.PortalID);
            parameters[1] = new SqlParameter("@ItemKey", itemIndex.Key);
            parameters[2] = new SqlParameter("@ItemType", itemIndex.Type);
            parameters[3] = new SqlParameter("@ItemPath", itemIndex.Path);
            parameters[4] = new SqlParameter("@ItemContent", itemIndex.Content);
            parameters[5] = new SqlParameter("@ItemSummary", itemIndex.Summary);
            parameters[6] = new SqlParameter("@ItemFilePath", itemIndex.Path);
            parameters[7] = new SqlParameter("@ItemFileName", itemIndex.Name);
            parameters[8] = new SqlParameter("@ItemCreatedDate", DateTime.Now);
            parameters[9] = new SqlParameter("@ItemModifiedDate", DateTime.Now);
            parameters[10] = new SqlParameter("@ItemViewRoles", itemIndex.ViewRoles);
            parameters[11] = new SqlParameter("@ItemMeta", ""); // TODO: add smart fields here as JSON?
            parameters[12] = new SqlParameter("@ItemFileSize", itemIndex.FileSize);


            SqlClientHelper.ExecuteNonQuery(connectionString, CommandType.Text, Sql, parameters);

        }
    }
}
