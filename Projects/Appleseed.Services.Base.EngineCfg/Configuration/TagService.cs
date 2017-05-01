namespace Appleseed.Services.Base.Engine.Configuration
{
    using Cfg.Net;
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Configuration section for a Metadata Tagging Service
    /// </summary>
    public class TagServiceSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<SiteToTagElement> Sites { get; set; }
    }

    /// <summary>
    /// Configuration element defining a site to be tagged by the Metadata Tagging Service
    /// </summary>
    public class SiteToTagElement : CfgNode
    {
        [Cfg(required = false, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string ConnectionString { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string Url { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public int PortalId { get; set; }
    }
}