
namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common.Logging;
    using SolrNet;
    using Appleseed.Services.Base.Engine.Processors.Impl.SolrModel;
    using Microsoft.Practices.ServiceLocation;
    using java.util;
    using System.Threading.Tasks;
    public class SolrIndexer : IIndexThings
    {
        private readonly Common.Logging.ILog logger;

        private readonly string indexPath;

        public SolrIndexer(ILog logger, string pathToIndex)
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

        public void Build(IEnumerable<Model.AppleseedModuleItemIndex> indexableData)
        {
            if (indexableData.Count().Equals(0))
            {
                throw new Exception("Nothing to index");
            }
            
            try { 
                //Initialize the solr server
                Startup.Init<SolrItem>(indexPath);
                }
            catch (Exception exc){}

            ISolrOperations<SolrItem> solr = ServiceLocator.Current.GetInstance<ISolrOperations<SolrItem>>();


            // grouping the results
            List<SolrItem> solrResults = null;
            List<List<SolrItem>> listResults = new List<List<SolrItem>>();
            int size = 0;
            int count = 0;
            int errorCount = 0;

            foreach (var data in indexableData)
            {
                if (size == 0)
                {
                    solrResults = new List<SolrItem>();
                }

                count++;
                if (!string.IsNullOrEmpty(data.Key) || !string.IsNullOrEmpty(data.Name))
                {
                    var solrItem = new SolrItem
                    {
                        Key = data.Key,
                        Path = data.Path,
                        Name = data.Name,
                        Content = data.Content,
                        Summary = data.Summary,
                        SmartKeywords = data.SmartKeywords,
                        Type = data.Type,
                        ViewRoles = data.ViewRoles,
                        PortalID = data.PortalID,
                        ModuleID = data.ModuleID,
                        PageID = data.PageID,
                        FileSize = data.FileSize,
                        CreatedDate = DateTime.Parse(data.CreatedDate),
                        Source = data.Source
                    };

                    solrResults.Add(solrItem);
                    size++;
                    if (size == 3 || count == indexableData.Count())
                    {
                        size = 0;
                        listResults.Add(new List<SolrItem>(solrResults));
                    }
                }
                else
                    errorCount++;
            }

            // Sequencial Indexing
            /*
            int index = 0;
            foreach (var list in listResults)
            {
                index++;

                logger.Info("List " + index + ". Index Start: ");
                try
                {
                    solr.AddRange(list);
                    solr.Optimize();
                }
                catch (Exception e)
                {
                    logger.Info("List " + index + ". Index End EXCEPTION : "  + string.Empty);

                    logger.Error(e.Message);
                }

                logger.Info("List " + index + ". Index End  : " + string.Empty);

            }
            */

            AddParameters addParameters = new AddParameters { Overwrite = true };
            // Parallel Indexing 

            Parallel.ForEach(listResults, group => solr.AddRange(group, addParameters));
            solr.Optimize();

            logger.Info("----------------INDEXER SUMMARY--------------------------------");
            logger.Info("Success Count: " + (count - errorCount).ToString());
            logger.Info("Error Count: " + errorCount.ToString());
            logger.Info("----------------INDEXER SUMMARY--------------------------------");


        }
    }
}
