namespace Appleseed.Services.Base.Engine.Configuration
{
    using Cfg.Net;
    using System.Collections.Generic;
    using System.Configuration;

    public class NutshellCRMApiCrawlerIndexServiceSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<NutshellCRMApiCrawlerToIndexElement> Websites { get; set; }
    }

    public class NutshellCRMApiCrawlerToIndexElement : CfgNode
    {
        [Cfg(required = false, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string ApiUrl { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string ApiKey { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string ApiUserEmail { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string IndexPath { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string ConnectionString { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string TableName { get; set; }
    }
}