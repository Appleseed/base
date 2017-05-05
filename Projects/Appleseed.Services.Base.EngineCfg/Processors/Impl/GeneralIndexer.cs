namespace Appleseed.Services.Base.Engine.Processors.Impl
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Common.Logging;
    using System.Configuration;

    using Appleseed.Services.Base.Engine.Configuration;

    public class GeneralIndexer : IIndexThings
    {
        private readonly Common.Logging.ILog logger;

        private readonly string indexPath;

        private WebContentIndexer luceneIndexer = null;

        private SolrIndexer solrIndexer = null;

        private ElasticSearchIndexer elasticSearchIndexer  = null;

        private Engine engine;


        public GeneralIndexer(ILog logger, string pathToIndex)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(pathToIndex))
            {
                throw new ArgumentNullException("pathToIndex");
            }

            this.indexPath = pathToIndex;
            this.logger = logger;
        }

        public GeneralIndexer(ILog logger, string pathToIndex, Engine engine)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(pathToIndex))
            {
                throw new ArgumentNullException("pathToIndex");
            }

            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }


            this.indexPath = pathToIndex;
            this.engine = engine;
            this.logger = logger;
        }

        public string[] GetIndexNames(string indexPath)
        {

            string[] names = indexPath.Split(',');

            string[] indexNames = new string[names.Length];

            for(int i = 0; i < names.Length; i++)
            {
                int start = names[i].IndexOf('{');
                int end = names[i].IndexOf('}');
                string name = names[i].Substring(start + 1, end - start - 1);
                indexNames[i] = name;
            }

            return indexNames;

        }

        public string GetIndexName(string indexName)
        {
            
            int start = indexName.IndexOf('{');
            int end = indexName.IndexOf('}');
            if (start != -1 && end != -1)
            {
                indexName = indexName.Substring(start + 1, end - start - 1).ToLower();
            }

            return indexName;
        }

        //TODO: change / refactor / override different Build method that takes the data from a Queue - can be SQL at first with just the connString + table
        // TODO: BuildFromQueue(string queueName) ??
        // TODO: uses the helper method Queue_Receive_ItemIndex in batches to Build(indexableDate)


        public void Build(IEnumerable<Model.AppleseedModuleItemIndex> indexableData)
        {
            //var indexesConfig = ConfigurationManager.GetSection("indexes") as IndexesSection;
            var config = this.engine;
            var indexCollection = this.engine.IndexesSectionCfg;

            if (config.IndexesSectionCfg.Count == 0 || config.IndexesSectionCfg == null)
            {
                logger.Info("INDEX SOURCES NOT FOUND IN CONFIG");
            }

            foreach (IndexesSectionCfg section in config.IndexesSectionCfg)
            {
                section.Indexes.ForEach(delegate (IndexesElementCfg index)
                {
                    if (GetIndexName(index.Name) == GetIndexName(this.indexPath))
                    {
                        if (index.Type == "Lucene.NET")
                        {
                            luceneIndexer = new WebContentIndexer(this.logger, index.Location);
                            luceneIndexer.Build(indexableData);
                        }

                        if (index.Type == "Solr")
                        {
                            solrIndexer = new SolrIndexer(this.logger, index.Location);
                            solrIndexer.Build(indexableData);
                        }

                        if (index.Type == "ElasticSearch")
                        {
                            elasticSearchIndexer = new ElasticSearchIndexer(this.logger, index.Location);
                            elasticSearchIndexer.Build(indexableData);
                        }
                    }

                });
            }

        }


    }
}
