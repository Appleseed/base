namespace Appleseed.Services.Base.Engine.Configuration
{
    using Cfg.Net;
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Configuration section for a Website Indexing Service
    /// </summary>
    public class WebsiteIndexServiceSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<WebsiteToIndexElement> Websites { get; set; }
    }

    /// <summary>
    /// Configuration element defining a website to be indexed by the Website Indexing Service
    /// </summary>
    public class WebsiteToIndexElement : CfgNode
    {
        [Cfg(required = false, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string SiteMapUrl { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string IndexPath { get; set; }
    }
}