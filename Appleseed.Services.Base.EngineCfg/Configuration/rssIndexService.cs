using Cfg.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appleseed.Services.Base.Engine.Configuration
{
    public class rssIndexServiceSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<WebsiteToIndexElement> Websites { get; set; }
    }

    /// <summary>
    /// Configuration element defining a website to be indexed by the Website Indexing Service
    /// </summary>
    public class rssIndexElement : CfgNode
    {
        [Cfg(required = false, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string SiteMapUrl { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string IndexPath { get; set; }
    }
}
