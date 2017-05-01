using Cfg.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appleseed.Services.Base.Engine.Configuration
{
    public class QueuesIndexServiceSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<QueuesIndexElement> Queues { get; set; }
    }

    /// <summary>
    /// Configuration element defining a website to be indexed by the Website Indexing Service
    /// </summary>
    public class QueuesIndexElement : CfgNode
    {
        [Cfg(required = false, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string ConnectionString { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string QueueName { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public string Type { get; set; }
    }
}
