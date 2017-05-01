namespace Appleseed.Services.Base.Engine.Configuration
{
    using Cfg.Net;
    using System.Collections.Generic;

    /// <summary>
    /// Configuration section for a File Indexing Service
    /// </summary>
    public class DbFileIndexServiceSection : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = false, ignoreCase = true)]
        public List<DbFileToIndexElement> Files { get; set; }
    }

    /// <summary>
    /// Configuration element defining a directory to be indexed by the File Indexing Service
    /// </summary>
    public class DbFileToIndexElement : CfgNode
    {
        [Cfg(required = true, ignoreCase = true)]
        public string Name { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string IndexPath { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string ConnectionString { get; set; }

        [Cfg(required = true, ignoreCase = true)]
        public string TableName { get; set; }
    }
}
