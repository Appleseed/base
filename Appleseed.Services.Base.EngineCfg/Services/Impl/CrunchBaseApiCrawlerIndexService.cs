
namespace Appleseed.Services.Base.Engine.Services.Impl
{
	using System;

    using Appleseed.Services.Base.Engine.Configuration;
	using Appleseed.Services.Core.Extractors.Impl;
	using Appleseed.Services.Base.Engine.Processors.Impl;

	using Common.Logging;
	public class CrunchBaseApiCrawlerIndexService : IAmABaseService
	{
        private readonly ILog logger;
        private readonly string indexPath;

        private readonly string sourceName;
		private readonly string sourceAuthAPIKey;
		private readonly string sourceAPIUri;
        private readonly string sourceAPIItemLimit;

		private readonly string queueConnectionString;
		private readonly string queueTableName;


		public CrunchBaseApiCrawlerIndexService(ILog logger, CrunchBaseApiCrawlerToIndexElement source)
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

            if (string.IsNullOrEmpty(source.itemsCollected) == null)
            {
                throw new ArgumentNullException("itemsCollected");
            }

            //source.ApiKey, source.ApiUrl, source.IndexPath, source.ConnectionString, source.TableName, source.itemsCollected

			this.logger = logger;
            this.sourceName = source.Name;
			this.sourceAuthAPIKey = source.ApiKey;
			this.sourceAPIUri = source.ApiUrl;
			this.indexPath = source.IndexPath;
			this.queueTableName = source.TableName;
			this.queueConnectionString = source.ConnectionString;
            this.sourceAPIItemLimit = source.itemsCollected;

		}

		public bool Run() {
            var aggregator = new CrunchBaseApiCrawlerAggregator(this.logger, this.sourceAuthAPIKey, this.sourceAPIUri, this.queueConnectionString, this.queueTableName, this.sourceAPIItemLimit);
			var collector = new CrunchBaseApiCrawlerCollector(this.logger, this.queueConnectionString, this.queueTableName);
			var extractor = new CrunchBaseApiCrawlerExtractor(this.logger,this.sourceAuthAPIKey, this.sourceAPIUri);
			var organizer = new AppleseedModuleItemIndexOrganizer(this.logger,this.sourceName);
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
