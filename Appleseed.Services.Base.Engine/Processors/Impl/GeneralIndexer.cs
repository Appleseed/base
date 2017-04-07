namespace Appleseed.Services.Base.Engine.Processors.Impl
{

    using System;
    using System.Collections.Generic;
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


        //TODO: change / refactor / override different Build method that takes the data from a Queue - can be SQL at first with just the connString + table
        // TODO: BuildFromQueue(string queueName) ??
        // TODO: uses the helper method Queue_Receive_ItemIndex in batches to Build(indexableDate)


        public void Build(IEnumerable<Model.AppleseedModuleItemIndex> indexableData)
        {
            var indexesConfig = ConfigurationManager.GetSection("indexes") as IndexesSection;
            foreach (var name in GetIndexNames(this.indexPath))
            {
                
                foreach (IndexesElement index in indexesConfig.Indexes)
                {
                    if (index.Name == name)
                    {
                        if (index.Type == "Lucene.NET")
                        {
                            luceneIndexer = new WebContentIndexer(this.logger, index.Location);
                            luceneIndexer.Build(indexableData);
                        }

                        if(index.Type == "Solr")
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

                }
            }
            
            
        }


    }
}
