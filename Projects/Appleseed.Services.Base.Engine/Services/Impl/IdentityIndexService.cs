namespace Appleseed.Services.Base.Engine.Services.Impl
{
    using System;

    using Appleseed.Services.Base.Engine.Processors.Impl;

    using Common.Logging;

    public class IdentityIndexService : IAmABaseService
    {
        private readonly string indexPath;

        private readonly ILog logger;

        public IdentityIndexService(ILog logger, string indexPath)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (indexPath == null)
            {
                throw new ArgumentNullException("indexPath");
            }
            
            this.logger = logger;
            this.indexPath = indexPath;
        }

        public bool Run()
        {
            var aggregator = new IdentityAggregator();
            var collector = new IdentityCollector();
            var extractor = new IdentityExtractor();
            var organizer = new AppleseedModuleItemIndexOrganizer(this.logger,"Identities");
            var indexer = new IdentityIndexer(this.logger, this.indexPath);

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