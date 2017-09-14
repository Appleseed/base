namespace Appleseed.Services.Base.Engine.Services.Impl
{
    using System;

    using Appleseed.Services.Base.Engine.Configuration;
    using Appleseed.Services.Core.Extractors.Impl;
    using Appleseed.Services.Base.Engine.Processors.Impl;

    using Common.Logging;

    public class WebCrawlerIndexService : IAmABaseService
    {
        private readonly string siteUrl;

        private readonly string indexPath;
        private readonly string queueConnectionString;
        private readonly string queueTableName;
        private readonly string sourceName;

        private readonly ILog logger;

        private readonly Engine engine;

        public WebCrawlerIndexService(ILog logger, WebCrawlToIndexElement source, Engine engine)
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

            if (string.IsNullOrEmpty(source.SiteMapUrl))
            {
                throw new ArgumentNullException("source.SiteMapUrl");
            }


            this.logger = logger;
            this.indexPath = source.IndexPath;
            this.sourceName = source.Name;
            this.siteUrl = source.SiteMapUrl;
            this.queueConnectionString = source.ConnectionString;
            this.queueTableName = source.TableName;
            this.engine = engine;

        }

        public bool Run()
        {
            var aggregator = new WebCrawlerAggregator(this.logger, 0, this.queueConnectionString, this.queueTableName, this.siteUrl);
            var collector = new FileContentCollector(this.logger, this.queueConnectionString, this.queueTableName);
            var extractor = new WebContentExtractor(this.logger);
            var organizer = new AppleseedModuleItemIndexOrganizer(this.logger, this.sourceName, this.engine);
            var indexer = new GeneralIndexer(this.logger, this.indexPath, this.engine);
            var engine = this.engine;

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