namespace Appleseed.Services.Base.Engine.Services.Impl
{
    using System;

    using Appleseed.Services.Base.Engine.Configuration;
    using Appleseed.Services.Core.Extractors.Impl;
    using Appleseed.Services.Base.Engine.Processors.Impl;

    using Common.Logging;
    public class DropBoxCloudFileIndexService : IAmABaseService
    {
        private readonly Common.Logging.ILog logger;
        private readonly string indexPath;
        private readonly string queueName;

        private readonly string sourceName;

        private readonly string sourceAuthAppKey;
        private readonly string sourceAuthAppSecret;
        private readonly string cacheFilesPath;
        private readonly string sourceAuthToken;
        private readonly string sourceAuthTokenSecret;
        private readonly string queueConnectionString;
        private readonly string queueTableName;
        

        public DropBoxCloudFileIndexService(ILog logger, DropBoxCloudFileToIndexElement source)
		    {
	            if (logger == null)
	            {
	                throw new ArgumentNullException("logger");
	            }

                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }



                if (string.IsNullOrEmpty(source.appKey))
                {
                    throw new ArgumentNullException("source.appKey");
                }


                if (string.IsNullOrEmpty(source.appSecret))
                {
                    throw new ArgumentNullException("source.appSecret");
                }


                if (string.IsNullOrEmpty(source.token))
                {
                    throw new ArgumentNullException("source.token");
                }

                if (string.IsNullOrEmpty(source.tokenSecret))
                {
                    throw new ArgumentNullException("source.tokenSecret");
                }

                /*if (string.IsNullOrEmpty(source.IndexPath))
                {
                    throw new ArgumentNullException("source.IndexPath");
                }*/

                if (string.IsNullOrEmpty(source.QueueName))
                {
                    throw new ArgumentNullException("source.QueueName");
                }

                if (string.IsNullOrEmpty(source.FilesPath))
                {
                    throw new ArgumentNullException("source.FilesPath");
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

                /*
                if (cacheFilesPath == null)
                {
                    throw new ArgumentNullException("filesPath");
                }
                */

	            //site.appKey, site.appSecret, site.token, site.tokenSecret, site.IndexPath, site.FilesPath, site.ConnectionString, site.TableName

	            this.logger = logger;
                this.sourceName = source.Name;
                this.sourceAuthAppKey = source.appKey;
                this.sourceAuthAppSecret = source.appSecret;
	            this.sourceAuthTokenSecret = source.tokenSecret;
                this.cacheFilesPath = source.FilesPath;
	            this.sourceAuthToken = source.token;
	            this.queueConnectionString = source.ConnectionString;
	            this.queueTableName = source.TableName;
                //this.indexPath = source.IndexPath;
                this.queueName = source.QueueName;
	            
	        }
        public bool Run() {

            var aggregator = new DropBoxCloudFileAggregator(logger, this.sourceAuthAppKey, this.sourceAuthAppSecret, this.sourceAuthToken, this.sourceAuthTokenSecret, this.cacheFilesPath, this.queueConnectionString, this.queueTableName);
            var collector = new DropboxCloudFileCollector(logger, this.queueConnectionString, this.queueTableName);
            var extractor = new WebContentExtractor(logger);
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
