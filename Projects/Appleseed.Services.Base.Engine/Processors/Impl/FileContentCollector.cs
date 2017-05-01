namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    using Appleseed.Services.Core.Helpers;
    using Appleseed.Services.Base.Model;

    using Common.Logging;

    /// <summary>
    /// Get a collection of files to be indexed from a database
    /// </summary>
    public class FileContentCollector : ICollectThings
    {
        private ILog Logger { get; set; }

        private string DatabaseConnectionString { get; set; }

        private string DatabaseTableName { get; set; }

        /*
         * CollectionItem needs to have a Data and a Collector - will replace collectedURLs
            Data = URL, ModuleID
	        Type = Page, File, Module 
         */

        private List<Model.BaseItem> CollectedItems { get; set; }

        public FileContentCollector(ILog logger, string databaseConnectionString, string databaseTableName)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(databaseConnectionString))
            {
                throw new ArgumentNullException("databaseConnectionString");
            }

            if (string.IsNullOrEmpty(databaseTableName))
            {
                throw new ArgumentNullException("databaseTableName");
            }

            this.DatabaseTableName = databaseTableName;
            this.Logger = logger;
            this.DatabaseConnectionString = databaseConnectionString;
        }

        /// <summary>
        /// Perform collection
        /// </summary>
        /// <returns></returns>
        public List<Model.BaseItem> CollectItems()
        {
            this.CollectedItems = new List<Model.BaseItem>();
            this.CollectItems(this.CollectedItems);
            return this.CollectedItems;
        }

        /// <summary>
        /// Perform collection
        /// </summary>
        /// <returns></returns>
        public void CollectItems(List<Model.BaseItem> items)
        {
            this.FileCollectionItems(items);
        }

        private const string FilesCacheSql = "SELECT * FROM {0} WHERE ItemKey IS NOT NULL AND ItemType IN ('{1}','{2}')";

        private void FileCollectionItems(List<BaseItem> collectionPages)
        {
            var sql = string.Format(FilesCacheSql, this.DatabaseTableName, CollectionItemType.FILE, CollectionItemType.PAGE);

            this.Logger.Info("FileContentCollector: ConnectionString: " + this.DatabaseConnectionString);
            this.Logger.Info("FileContentCollector: Table: " + this.DatabaseTableName);

            // get SQL data reader
            using (var reader = SqlClientHelper.GetReader(this.DatabaseConnectionString, sql))
            {

                if (!reader.HasRows)
                {
                    this.Logger.Error("No data found in the database.");
                }

                var itemKeyIndex        = reader.GetOrdinal("ItemKey");
                var itemPathIndex       = reader.GetOrdinal("ItemPath");
                var itemFilePathIndex   = reader.GetOrdinal("ItemFilePath");
                var itemFileNameIndex   = reader.GetOrdinal("ItemFileName");
                var itemViewRolesIndex  = reader.GetOrdinal("ItemViewRoles");
                var itemTypeIndex = reader.GetOrdinal("ItemType");
                
                
                //create items of this data and show that we collected them 
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
                    // Item Type is Contact or Account

                    var itemPath = "";
                    var itemType = "";
                    switch (GetDbStringValue(reader, itemTypeIndex))
                    {
                        case CollectionItemType.FILE:
                            itemPath = GetDbStringValue(reader, itemFilePathIndex) + "\\" + GetDbStringValue(reader, itemFileNameIndex);
                            itemType = CollectionItemType.FILE;
                            break;

                        case CollectionItemType.PAGE:
                            itemPath = GetDbStringValue(reader, itemPathIndex);
                            itemType = CollectionItemType.PAGE;
                            break;

                        default:
                            itemPath = GetDbStringValue(reader, itemPathIndex);
                            itemType = CollectionItemType.PAGE;
                            break;
                    }

                    //TODO

                    
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

                    // add to collection 
                    collectionPages.Add(
                        new AppleseedModuleItem()
                            {
                                Key = GetDbStringValue(reader, itemKeyIndex),
                                Path = itemPath,
                                ModuleID = 0,
                                PageID = 0,
                                PortalID = 0,
                                ViewRoles = viewRoles,
                                PublishedDate = publishedDate,
                                Type = itemType,
                            });

                    this.Logger.Info("Collected Item:" + GetDbStringValue(reader, itemFileNameIndex) + " : Key=" + GetDbStringValue(reader, itemKeyIndex));
                }
            }
        }

        private static string GetDbStringValue(SqlDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
        }

        //private static void FileCollectionItems(List<BaseItem> collectionPages)
        //{
        //    collectionPages.Add(new AppleseedModuleItem("http://www.ifad.org/pub/policy/km/e.pdf") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://www.utexas.edu/depts/ic2/pubs/syed.pdf") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://www.innovation.at/wp-content/uploads/2011/02/An_Illustrated_Guide_to_Knowledge_Management.pdf") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://public.kenan-flagler.unc.edu/faculty/malhotra/kmjmis.pdf") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://www.oracle.com/us/products/applications/5-befit-knowlg-manag-cust-serv-wp-521298.pdf") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://www.oracle.com/us/products/applications/getting-knowledge-managt-right-wp-1353041.pdf") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://armypubs.army.mil/doctrine/DR_pubs/dr_a/pdf/fm6_01x1.pdf") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });

        //    collectionPages.Add(new AppleseedModuleItem("http://www.unlibrary-nairobi.org/PDFs/PhenKM.doc") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://www2.ed.gov/about/offices/list/oig/aireports/i13e0022.doc") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    //IndexFILES.Add("http://www.iai.uni-bonn.de/~prosec/ECSCW-XMWS/FullPapers/huysman.doc"){ ItemModuleID = 0, ItemPageID = 0, ItemPortalID = 0, ItemType = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://www.processrenewal.com/files/def-km.doc") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://umaine.edu/business/files/2009/06/46_km-champions.doc") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://ai.arizona.edu/mis480/syllabus/3_MIS580-KM-articles-alpha.doc") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });

        //    collectionPages.Add(new AppleseedModuleItem("http://128.206.119.157/nlp/papers/NLPTools.pdf") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //    collectionPages.Add(new AppleseedModuleItem("http://2009.rmll.info/IMG/pdf/FrancoisRegisChaumartin_UIMA_LSM09_paper.pdf") { ModuleID = 0, PageID = 0, PortalID = 0, Type = CollectionItemType.FILE });
        //}

        //private static List<AppleseedModuleItem> ModuleCollectionItems()
        //{
        //    List<AppleseedModuleItem> CollectionModules = new List<AppleseedModuleItem>();
        //    // the initial ItemPath for a module is "PORTALID/PAGEID/MODULEID/MODULEITEMID" like a coordinate 
        //    // it is used to create the internal ItemKey / Hashcode - however, after extraction, the ItemPath will be the actual page it is on. 
        //    // this only works for modules with single instance e.g. no moduleitem id 

        //    CollectionModules.Add(
        //            new AppleseedModuleItem("0/0/1")
        //            {
        //                PortalID = 0,
        //                PageID = 0,
        //                ModuleID = 1,
        //                Type = CollectionItemType.MODULE
        //            });

        //    // if multi instance -- add more 

        //    CollectionModules.Add(
        //        new AppleseedModuleItem("0/1/2")
        //        {
        //            PortalID = 0,
        //            PageID = 1,
        //            ModuleID = 2,
        //            Type = CollectionItemType.MODULE
        //        });

        //    CollectionModules.Add(
        //        new AppleseedModuleItem("0/1/3")
        //        {
        //            PortalID = 0,
        //            PageID = 1,
        //            ModuleID = 2,
        //            Type = CollectionItemType.MODULE
        //        });

        //    CollectionModules.Add(
        //        new AppleseedModuleItem("0/1/4")
        //        {
        //            PortalID = 0,
        //            PageID = 1,
        //            ModuleID = 2,
        //            Type = CollectionItemType.MODULE
        //        });

        //    return CollectionModules;
        //}
    }
}
