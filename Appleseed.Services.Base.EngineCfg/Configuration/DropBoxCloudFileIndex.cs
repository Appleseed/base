namespace Appleseed.Services.Base.Engine.Configuration
{
    using Cfg.Net;
    using System.Collections.Generic;

    public class DropBoxCloudFileIndexServiceSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<DropBoxCloudFileToIndexElement> Websites { get; set; }
    }

    public class DropBoxCloudFileToIndexElement : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string appKey { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string appSecret { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string token { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string tokenSecret { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string IndexPath { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string FilesPath { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string ConnectionString { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string QueueName { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string TableName { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string UserAgent { get; set; }
    }
}