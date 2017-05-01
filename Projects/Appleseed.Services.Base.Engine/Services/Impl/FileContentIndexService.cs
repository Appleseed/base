namespace Appleseed.Services.Base.Engine.Services.Impl
{
    using System;

    using Appleseed.Services.Base.Engine.Configuration;
    using Appleseed.Services.Base.Engine.Processors.Impl;

    using Common.Logging;

    public class FileContentIndexService : IAmABaseService
    {
        private readonly string indexPath;        
        private readonly string queueConnectionString;
        private readonly string queueTableName;
        private readonly string sourceName;

        private readonly ILog logger;

        public FileContentIndexService(ILog logger, DbFileToIndexElement source)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (string.IsNullOrEmpty(source.IndexPath))
            {
                throw new ArgumentNullException("source.IndexPath");
            }

            if (string.IsNullOrEmpty(source.Name))
            {
                throw new ArgumentNullException("source.Name");
            }

            if (string.IsNullOrEmpty(source.ConnectionString))
            {
                throw new ArgumentNullException("source.ConnectionString");
            }

            if (string.IsNullOrEmpty(source.TableName))
            {
                throw new ArgumentNullException("source.TableName");
            }
            
            this.logger = logger;
            this.indexPath = indexPath;
            this.sourceName = source.Name;
            this.queueConnectionString = source.ConnectionString;
            this.queueTableName = source.TableName;
        }

        public bool Run()
        {
            var aggregator = new WebContentAggregator();
            //change to use WebCrawlerAggregator args: ILog logger, int crawlDepth, string connectionString, string crawlTable, string crawlSite
            //var aggregator = new WebCrawlerAggregator(this.logger, 10, this.connectionString, this.tableName, );
            var collector = new FileContentCollector(this.logger, this.queueConnectionString, this.queueTableName);
            var extractor = new WebContentExtractor(this.logger);
            var organizer = new AppleseedModuleItemIndexOrganizer(this.logger, this.sourceName);
            var indexer = new GeneralIndexer(this.logger, this.indexPath);

            var indexService = new IndexService(collector, aggregator, extractor, organizer, indexer);

            try
            {
                indexService.Run();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex);
                return false;
            }

            return true;
        }
    }
}