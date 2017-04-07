namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using Abot.Crawler;
    using Abot.Poco;
    using Abot.Core;
    using System.Net;
    using Common.Logging;

    using Appleseed.Services.Core.Helpers;
    using Appleseed.Services.Base.Model;

    public class WebCrawlerAggregator : IAggregateData
    {
        private readonly Common.Logging.ILog logger;
        private readonly int crawlDepth;
        private readonly string crawlSite;
        private readonly string connectionString;
        private readonly string crawlTable;

        private CrawlConfiguration crawlConfig;

        public WebCrawlerAggregator(ILog logger, int crawlDepth, string connectionString, string crawlTable, string crawlSite)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(crawlSite))
            {
                throw new ArgumentNullException("crawlSite");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(crawlTable))
            {
                throw new ArgumentNullException("crawlTable");
            }

            if (crawlDepth < 0)
            {
                throw new Exception("Crawl Depth cannot be less than zero.");
            }

            this.crawlDepth = crawlDepth;
            this.crawlSite = crawlSite;
            this.crawlTable = crawlTable;
            this.connectionString = connectionString;
            this.logger = logger;
            //TODO: read from config and pass params - not yet implemented
            
            this.logger.Info("----------------CONFIGURING WEBCRAWLER--------------------------------");
            this.crawlConfig = this.CrawConfiguration();
        }

        private CrawlConfiguration CrawConfiguration()
        {
            //Load from XML 
            CrawlConfiguration configuration = AbotConfigurationSectionHandler.LoadFromXml().Convert();

            //overrite crawl depth if set 
            if(this.crawlDepth!=0){
                configuration.MaxCrawlDepth = this.crawlDepth;
            }

            /*
            CrawlConfiguration configuration = new CrawlConfiguration()
            {
                CrawlTimeoutSeconds = 100,
                MaxConcurrentThreads = 10,
                MaxCrawlDepth = this.crawlDepth,
                MaxPagesToCrawl = 10,
                // MaxPagesToCrawlPerDomain = 100, 
                // MaxPageSizeInBytes = 0,
                DownloadableContentTypes = "text/html, text/plain",
                IsUriRecrawlingEnabled = false,
                IsExternalPageCrawlingEnabled = true,
                IsExternalPageLinksCrawlingEnabled = true,
                HttpServicePointConnectionLimit = 200,
                HttpRequestTimeoutInSeconds = 15,
                HttpRequestMaxAutoRedirects = 7,
                IsHttpRequestAutoRedirectsEnabled = true,
                IsHttpRequestAutomaticDecompressionEnabled = true,
                MinAvailableMemoryRequiredInMb = 0,
                //MaxMemoryUsageInMb = 0,
                //MaxMemoryUsageCacheTimeInSeconds = 0,
                //IsForcedLinkParsingEnabled = false,
                UserAgentString = "abot v1.0 http://code.google.com/p/abot"
            };*/

            return configuration;
        }

        public void Aggregate()
        {
            this.logger.InfoFormat("----------------STARTING WEBCRAWLER.AGGREGATE--------------------------- {0}", 1);


            //TODO: Implement ABot here to 
            PoliteWebCrawler crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null, null);

            //TODO crawlbag is not working -- maybe doesnt work in scriptcs?
            //crawler.CrawlBag.VendorName 	= vendor.Name;
            //crawler.CrawlBag.VendorCategory = vendor.Category;
            //crawler.CrawlBag.VendorRegion 	= vendor.Region;

            crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

            try
            {

                CrawlResult result = crawler.Crawl(new Uri(this.crawlSite));

                if (result.ErrorOccurred)
                    this.logger.InfoFormat("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
                else
                    this.logger.InfoFormat("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);

            }
            catch (System.UriFormatException e)
            {

                this.logger.InfoFormat("Crawl of {0} completed with error.", e.ToString());
            }

            this.logger.Info("----------------ENDING WEBCRAWLER.AGGREGATE--------------------------------");

        }

        public void AggregateItem(AppleseedModuleItem item)
        {
            //refactored to core helpers because it will be used everywhere 
            Appleseed.Services.Core.Helpers.CollectionHelper.AggregateItem(item, this.connectionString, this.crawlTable);
        }

        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            //TODO: make this into a logging call
            this.logger.InfoFormat("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            CrawlContext context = e.CrawlContext;

            //TODO: make console writes into a logging call

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)

                this.logger.InfoFormat("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                this.logger.InfoFormat("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                this.logger.InfoFormat("Page had no content {0}", crawledPage.Uri.AbsoluteUri);

            //this.logger.InfoFormat("Crawl of page data {0}", crawledPage.Content.Text);

            //TODO figure out why crawlbag / crawlcontext arent working in scriptcs
            //DONE - save some data

            var newItem = new AppleseedModuleItem()
            {
                Key = String.Format("{0:X}", crawledPage.Uri.AbsoluteUri.GetHashCode()), //DONE: create hash of URL
                Path = crawledPage.Uri.AbsoluteUri,
                ModuleID = 0,
                PageID = 0,
                PortalID = 0,
                ViewRoles = CollectionItemSecurityType.PUBLIC,
                Type = CollectionItemType.PAGE,
                FileSize = crawledPage.Content.Bytes.Length,
                Name = "Name"
            };
            

            //TODO: add this into the DB using AggregateItem() 

            this.AggregateItem(newItem);
            this.logger.InfoFormat("WEBCRAWLER.AGGREGATE.ITEM Added {0} into Cache Collection", newItem.Path);

        }

        void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            
            var newItem = new AppleseedModuleItem()
            {
                Key = String.Format("{0:X}", crawledPage.Uri.AbsoluteUri.GetHashCode()), //DONE: create hash of URL
                Path = crawledPage.Uri.AbsoluteUri,
                ModuleID = 0,
                PageID = 0,
                PortalID = 0,
                ViewRoles = CollectionItemSecurityType.PUBLIC,
                Type = CollectionItemType.FILE,
                FileSize = crawledPage.Content.Bytes.Length,
                Name = "Name"
            };


            //TODO: add this into the DB using AggregateItem() 

            this.AggregateItem(newItem);
            this.logger.InfoFormat("WEBCRAWLER.AGGREGATE.ITEM Added {0} into Cache Collection", newItem.Path);

            this.logger.InfoFormat("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            this.logger.InfoFormat("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }

    }
}
