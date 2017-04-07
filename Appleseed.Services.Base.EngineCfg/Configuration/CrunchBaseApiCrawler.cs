namespace Appleseed.Services.Base.Engine.Configuration
{
    using Cfg.Net;
    using System.Collections.Generic;

    public class CrunchBaseApiCrawlerIndexServiceSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name
        {
            get;
            set;
        }

        [Cfg(required = false, ignoreCase = true)]
        public List<CrunchBaseApiCrawlerToIndexElement> Websites { get; set; }
    }

    public class CrunchBaseApiCrawlerToIndexElement : CfgNode 
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name
        {
            get;
            set;
        }

        [Cfg(required = true, ignoreCase = true)]
        public string ApiUrl
        {
            get;
            set;
        }

        [Cfg(required = true, ignoreCase = true)]
        public string ApiKey
        {
            get;
            set;
        }

        [Cfg(required = true, ignoreCase = true)]
        public string IndexPath
        {
            get;
            set;
        }

        [Cfg(required = true, ignoreCase = true)]
        public string ConnectionString
        {
            get;
            set;
        }

        [Cfg(required = true, ignoreCase = true)]
        public string TableName
        {
            get;
            set;
        }

        [Cfg(required = true, ignoreCase = true)]
        public string itemsCollected
        {
            get;
            set;
        }
    }
}
