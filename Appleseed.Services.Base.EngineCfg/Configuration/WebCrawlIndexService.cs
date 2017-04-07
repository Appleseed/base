namespace Appleseed.Services.Base.Engine.Configuration
{
    using Cfg.Net;
    using System.Collections.Generic;
    using System.Configuration;

    public class WebCrawlIndexServiceSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<WebCrawlToIndexElement> Websites { get; set; }
    }

    /// <summary>
    /// Configuration element defining a website to be indexed by the WebCrawl Indexing Service
    /// </summary>
    public class WebCrawlToIndexElement : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string SiteMapUrl { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public int crawlDepth { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string IndexPath { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string ConnectionString { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string TableName { get; set; }
    }
}