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
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

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

                var engineItems = session.Execute("select * from config");
                
                foreach (var itemRow in engineItems)
                {
                    var configName = (itemRow["config_name"] ?? "").ToString();
                    var configType = (itemRow["config_type"] ?? "").ToString();

                    var itemValues = new SortedDictionary<string, IDictionary<string, string>>();
                    itemValues = (SortedDictionary<string, IDictionary<string, string>>)(itemRow["config_values"]);
                    foreach (var values in itemValues)
                    {
                        var itemName = values.Key.ToString();
                        var itemSitemapUrl = (values.Value.ContainsKey("siteMapUrl") ? values.Value["siteMapUrl"] : "").ToString();
                        var itemLocationUrl = (values.Value.ContainsKey("location") ? values.Value["location"] : "").ToString();
                        var itemCollectionName = (values.Value.ContainsKey("collectionName") ? values.Value["collectionName"] : "").ToString();
                        var itemIndexPath = (values.Value.ContainsKey("indexPath") ? values.Value["indexPath"] : "").ToString();
                        var itemType = (values.Value.ContainsKey("type") ? values.Value["type"] : "").ToString();

                        switch (configName)
                        {
                            case "Search.Index":
                                var indexElement = new IndexesElementCfg();
                                indexElement.Name = itemName;
                                indexElement.Location = itemLocationUrl;
                                indexElement.Type = itemType;
                                indexElement.CollectionItem = itemCollectionName;
                                indexesElementsList.Add(indexElement);
                                break;
                            case "Web.Site.RSS.XML":
                                var rss = new rssIndexElement();
                                rss.Name = itemName;
                                rss.SiteMapUrl = itemSitemapUrl;
                                rss.IndexPath = itemIndexPath;
                                rssIndexesElementsList.Add(rss);
                                break;
                            case "Web.Site.Sitemap.XML":
                                var website = new WebsiteToIndexElement();
                                website.Name = itemName;
                                website.SiteMapUrl = itemSitemapUrl;
                                website.IndexPath = itemIndexPath;
                                websiteIndexesElementsList.Add(website);
                                break;
                            default:
                                break;
                        }
                    };
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
                Logger.Error(ex);
            }
            return config;
        }
    }
}
