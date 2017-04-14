using Appleseed.Services.Base.Engine.Configuration;
using Appleseed.Services.Base.Engine.Services.Impl;
using Appleseed.Services.Core.Helpers;
using Cassandra;
using Common.Logging;
using System;
using System.Collections.Generic;
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

            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            ISession session = cluster.Connect("appleseed_search_engines");

            var indexesSection = new IndexesSectionCfg();
            var indexesElementsList = new List<IndexesElementCfg>();
            var rssIndexesSection = new rssIndexServiceSection();
            var rssIndexesElementsList = new List<rssIndexElement>();
            var websiteIndexesSection = new WebsiteIndexServiceSection();
            var websiteIndexesElementsList = new List<WebsiteToIndexElement>();

            var engineTypes = session.Execute("select * from engine_types");
            /*foreach (var row in engineTypes)
            {
                var id = row["id"];
                var name = row["_name"];
                //Console.WriteLine(id.ToString() + name.ToString());
            }*/

            var engines = session.Execute("select * from engines");
            /*foreach (var row in engines)
            {
                var id = row["id"];
                var name = row["_name"];
                var typeId = row["_typeid"];
                //Console.WriteLine(id.ToString() + name.ToString() + typeId.ToString());
            }*/

            var engineItems = session.Execute("select * from engine_items");
            foreach (var row in engineItems)
            {
                var id = (row["id"] ?? "").ToString();
                var name = (row["_name"] ?? "").ToString();
                var engineId = (row["_engineid"] ?? "").ToString();
                var locationUrl = (row["_locationurl"] ?? "").ToString();
                var type = (row["_type"] ?? "").ToString();
                var collectionName = (row["_collectionname"] ?? "").ToString();
                var indexPath = (row["_indexpath"] ?? "").ToString();
                System.Console.WriteLine(id + name + engineId + locationUrl + type + collectionName + indexPath);

                //var engineName = session.Execute("select _name from engines where id = " + engineId + " limit 1").ToString();

                switch (engineId)
                {
                    case "1":
                        var indexElement = new IndexesElementCfg();
                        indexElement.Name = name;
                        indexElement.Location = locationUrl;
                        indexElement.Type = type;
                        indexElement.CollectionItem = collectionName;
                        indexesElementsList.Add(indexElement);
                        break;
                    case "2":
                        var rss = new rssIndexElement();
                        rss.Name = name;
                        rss.SiteMapUrl = locationUrl;
                        rss.IndexPath = indexPath;
                        rssIndexesElementsList.Add(rss);
                        break;
                    case "3":
                        var website = new WebsiteToIndexElement();
                        website.Name = name;
                        website.SiteMapUrl = locationUrl;
                        website.IndexPath = indexPath;
                        websiteIndexesElementsList.Add(website);
                        break;
                    default:
                        break;
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

    }
}
