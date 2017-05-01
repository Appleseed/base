
namespace Appleseed.Services.Base.Engine.Services.Impl
{
    using System;

    using Appleseed.Services.Base.Engine.Configuration;
    using Appleseed.Services.Core.Extractors.Impl;
    using Appleseed.Services.Base.Engine.Processors.Impl;

    using Common.Logging;
    public class NutshellCRMApiCrawlerIndexService : IAmABaseService
    {
        private readonly ILog logger;
        private readonly string indexPath;

        private readonly string sourceName;
        private readonly string sourceAuthAPIKey;
        private readonly string sourceAuthUserEmail;
        private readonly string sourceAPIUri;
        private readonly string queueConnectionString;
        private readonly string queueTableName;


        public NutshellCRMApiCrawlerIndexService(ILog logger, NutshellCRMApiCrawlerToIndexElement source)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }



            if (string.IsNullOrEmpty(source.ApiKey))
            {
                throw new ArgumentNullException("source.ApiKey");
            }


            if (string.IsNullOrEmpty(source.ApiUrl))
            {
                throw new ArgumentNullException("source.ApiUrl");
            }


            if (string.IsNullOrEmpty(source.ApiUserEmail))
            {
                throw new ArgumentNullException("source.ApiUserEmail");
            }

            if (string.IsNullOrEmpty(source.IndexPath))
            {
                throw new ArgumentNullException("source.IndexPath");
            }

            if (string.IsNullOrEmpty(source.ConnectionString))
            {
                throw new ArgumentNullException("source.ConnectionString");
            }

            if (string.IsNullOrEmpty(source.TableName))
            {
                throw new ArgumentNullException("source.TableName");
            }

            if (string.IsNullOrEmpty(source.Name))
            {
                throw new ArgumentNullException("source.Name");
            }


            this.logger = logger;
            this.sourceName = source.Name;
            this.sourceAuthAPIKey = source.ApiKey;
            this.sourceAuthUserEmail = source.ApiUserEmail;
            this.sourceAPIUri = source.ApiUrl;
            this.indexPath = source.IndexPath;
            this.queueTableName = source.TableName;
            this.queueConnectionString = source.ConnectionString;
           
        }

        public bool Run() {
            var aggregator = new NutshellCRMApiCrawlerAggregator(this.logger, this.sourceAuthAPIKey, this.sourceAuthUserEmail, this.sourceAPIUri, this.queueConnectionString, this.queueTableName);
            var collector = new NutshellCRMApiCrawlerCollector(this.logger, this.queueConnectionString, this.queueTableName);
            var extractor = new NutshellCRMApiCrawlerExtractor(this.logger, this.sourceAuthAPIKey, this.sourceAuthUserEmail, this.sourceAPIUri);
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
