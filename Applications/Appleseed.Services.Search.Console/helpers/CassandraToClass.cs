using Appleseed.Services.Base.Engine.Configuration;
using Appleseed.Services.Base.Engine.Services.Impl;
using Appleseed.Services.Core.Helpers;
using Cassandra;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Appleseed.Services.Search.Console.helpers
{
    class CassandraToClass
    {

        public static Engine GetCassandraConfig()
        {
            var config = new Engine();
            var appConfigSource = ConfigurationManager.AppSettings["CassandraUrl"];
            var appConfigSourcePort = Convert.ToInt32(ConfigurationManager.AppSettings["CassandraPort"]);

            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();

            try
            {
                ISession session = cluster.Connect("appleseed_search_engines");

                var indexesSection = new IndexesSectionCfg();
                var indexesElementsList = new List<IndexesElementCfg>();
                var rssIndexesSection = new rssIndexServiceSection();
                var rssIndexesElementsList = new List<rssIndexElement>();
                var websiteIndexesSection = new WebsiteIndexServiceSection();
                var websiteIndexesElementsList = new List<WebsiteToIndexElement>();

                var engineTypes = session.Execute("select * from engine_types");
                foreach (var typeRow in engineTypes)
                {
                    //var typeId = typeRow.GetColumn("id").Index;
                    //var typeIds = typeRow[typeId];
                    var typeId = typeRow["id"];
                    var typeName = typeRow["_name"];
                    var engines = session.Execute("select * from engines where \"_typeid\" = " + typeId + " ALLOW FILTERING");
                    foreach (var engineRow in engines)
                    {
                        var engineId = typeRow["id"];
                        //var engineName = engineRow["_name"];
                        var engineTypeId = engineRow["_typeid"];
                        var engineItems = session.Execute("select * from engine_items where \"_engineid\" = " + engineId + " ALLOW FILTERING");
                        foreach (var itemRow in engineItems)
                        {
                            var itemId = (itemRow["id"] ?? "").ToString();
                            var itemName = (itemRow["_name"] ?? "").ToString();
                            var itemEngineId = (itemRow["_engineid"] ?? "").ToString();
                            var itemLocationUrl = (itemRow["_locationurl"] ?? "").ToString();
                            var itemType = (itemRow["_type"] ?? "").ToString();
                            var itemCollectionName = (itemRow["_collectionname"] ?? "").ToString();
                            var itemIndexPath = (itemRow["_indexpath"] ?? "").ToString();

                            switch (typeName.ToString())
                            {
                                case "index":
                                    var indexElement = new IndexesElementCfg();
                                    indexElement.Name = itemName;
                                    indexElement.Location = itemLocationUrl;
                                    indexElement.Type = itemType;
                                    indexElement.CollectionItem = itemCollectionName;
                                    indexesElementsList.Add(indexElement);
                                    break;
                                case "rss":
                                    var rss = new rssIndexElement();
                                    rss.Name = itemName;
                                    rss.SiteMapUrl = itemLocationUrl;
                                    rss.IndexPath = itemIndexPath;
                                    rssIndexesElementsList.Add(rss);
                                    break;
                                case "sitemap":
                                    var website = new WebsiteToIndexElement();
                                    website.Name = itemName;
                                    website.SiteMapUrl = itemLocationUrl;
                                    website.IndexPath = itemIndexPath;
                                    websiteIndexesElementsList.Add(website);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                // Indexes
                indexesSection.Name = "IndexesSection";
                indexesSection.Indexes = indexesElementsList;
                var indexesSectionList = new List<IndexesSectionCfg>();
                indexesSectionList.Add(indexesSection);
                // RSS feeds
                rssIndexesSection.Name = "rssIndexServiceSection";
                rssIndexesSection.Websites = rssIndexesElementsList;
                var rssIndexesSectionList = new List<rssIndexServiceSection>();
                rssIndexesSectionList.Add(rssIndexesSection);
                // Website Sitemaps
                websiteIndexesSection.Name = "WebsiteIndexServiceSection";
                websiteIndexesSection.Websites = websiteIndexesElementsList;
                var websiteIndexesSectionList = new List<WebsiteIndexServiceSection>();
                websiteIndexesSectionList.Add(websiteIndexesSection);


                // Add all lists to config
                config.IndexesSectionCfg = indexesSectionList;
                config.rssIndexServiceSection = rssIndexesSectionList;
                config.WebsiteIndexServiceSection = websiteIndexesSectionList;
                return config;

            }

            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            return config;
        }
    }
}
