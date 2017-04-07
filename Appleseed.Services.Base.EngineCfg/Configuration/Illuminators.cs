namespace Appleseed.Services.Base.Engine.Configuration
{
    using Cfg.Net;
    using System.Collections.Generic;
    using System.Configuration;

    public class IlluminatorsSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<IlluminatorsElement> illuminators { get; set; }
    }

    public class IlluminatorsElement : CfgNode
    {
        [Cfg(required = false, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string Type { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string ApiKey { get; set; }
    }
}