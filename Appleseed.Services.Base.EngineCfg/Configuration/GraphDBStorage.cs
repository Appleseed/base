namespace Appleseed.Services.Base.Engine.Configuration
{
    using Cfg.Net;
    using System.Collections.Generic;

    public class GraphDBsSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<GraphDBsElement> GraphDBs { get; set; }
    }

    public class GraphDBsElement : CfgNode
    {
        [Cfg(required = false, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string Type { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string Location { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string Uri { get; set; }
    }
}