namespace Appleseed.Services.Base.Engine.Services.Impl
{
    using System;

    using Appleseed.Services.Base.Engine.Configuration;
    using Appleseed.Services.Core.Extractors.Impl;
    using Appleseed.Services.Base.Engine.Processors.Impl;

    using Common.Logging;

    public class RssContentIndexService : IAmABaseService
    {
        private readonly string rssUrl;

        private readonly string indexPath;
        private readonly string sourceName;

        private readonly ILog logger;

        private readonly Engine engine;

        public RssContentIndexService(ILog logger, rssIndexElement source, string rssUrl, Engine engine)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (rssUrl == null)
            {
                throw new ArgumentNullException("rssUrl");
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
            this.rssUrl = rssUrl;
            this.engine = engine;
        }

        public bool Run()
        {
            var aggregator = new WebContentAggregator();
            var collector = new RssContentCollector(this.logger,  new XmlExtractor(), this.rssUrl);
            var extractor = new WebContentExtractor(this.logger);
            var organizer = new AppleseedModuleItemIndexOrganizer(this.logger, this.sourceName);
            var indexer = new GeneralIndexer(this.logger, this.indexPath, this.engine);
            var engine = this.engine;

            var indexService = new IndexService(collector, aggregator, extractor, organizer, indexer, engine);

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


