using Cfg.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appleseed.Services.Base.Engine.Configuration
{
    public class Engine : CfgNode
    { 
        public Engine()
        {

        }

        public Engine(string xml)
        {
            this.Load(xml);
        }

        [Cfg(required = false, ignoreCase = true)]
        public List<CrunchBaseApiCrawlerIndexServiceSection> CrunchBaseApiCrawlerIndexServiceSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<DbFileIndexServiceSection> DbFileIndexServiceSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<DropBoxCloudFileIndexServiceSection> DropBoxCloudFileIndexServiceSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<GraphDBsSection> GraphDBsSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<IlluminatorsSection> IlluminatorsSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<IndexesSection> IndexesSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<IndexesSectionCfg> IndexesSectionCfg { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<NutshellCRMApiCrawlerIndexServiceSection> NutshellCRMApiCrawlerIndexServiceSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<QueuesIndexServiceSection> QueuesIndexServiceSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<rssIndexServiceSection> rssIndexServiceSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<TagServiceSection> TagServiceSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<WebCrawlIndexServiceSection> WebCrawlIndexServiceSection { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<WebsiteIndexServiceSection> WebsiteIndexServiceSection { get; set; }
    }
}
