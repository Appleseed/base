namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;

    using Appleseed.Services.Core.Extractors;
    using Appleseed.Services.Base.Model;

    using Common.Logging;

    /// <summary>
    /// Get all the links from a Sitemap file.  These links will be used to extract content.
    /// </summary>
    public class SiteMapCollector : ICollectThings
    {
        private ILog logger;

        private readonly string Sitemap;

        private readonly Core.Extractors.IUrlContentExtractor ContentExtractor;

        /*
         * CollectionItem needs to have a Data and a Collector - will replace collectedURLs
            Data = URL, ModuleID
	        Type = Page, File, Module 
         */

        private List<Model.BaseItem> CollectedItems { get; set; }

        public SiteMapCollector(ILog logger, string sitemapUrl, IUrlContentExtractor contentExtractor)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(sitemapUrl))
            {
                throw new ArgumentNullException("sitemapUrl");
            }

            if (contentExtractor == null)
            {
                throw new ArgumentNullException("contentExtractor");
            }

            this.logger = logger;
            this.Sitemap = sitemapUrl;
            this.ContentExtractor = contentExtractor;
        }

        public List<Model.BaseItem> CollectItems()
        {
            this.CollectedItems = new List<Model.BaseItem>();
            if (!string.IsNullOrEmpty(this.Sitemap))
            {
                this.PageCollectionItems(this.Sitemap, this.CollectedItems);
            }

            return this.CollectedItems;
        }

        public void CollectItems(List<Model.BaseItem> items)
        {
            if (!string.IsNullOrEmpty(this.Sitemap))
            {
                this.PageCollectionItems(this.Sitemap, items);
            }
        }

        //unsure where this list should be placed given the multiple indexes defined
        private void PageCollectionItems(string sitemapPath, List<Model.BaseItem> collectionPages) 
        {
            if (string.IsNullOrEmpty(sitemapPath))
            {
                throw new ArgumentNullException("sitemapPath");
            }

            var content = this.ContentExtractor.Extract(this.Sitemap);
            var txtReader = new StringReader(content.Content);
            var xmlReader = new XmlTextReader(txtReader);

            var document = new XPathDocument(xmlReader);
            var navigator = document.CreateNavigator();

            var resolver = new XmlNamespaceManager(xmlReader.NameTable);
            // TODO: need to add actual sitemap and verify schema, can set as class attribute
            resolver.AddNamespace("sitemap", "http://www.sitemaps.org/schemas/sitemap/0.9");

            var iterator = navigator.Select("/sitemap:urlset/sitemap:url/sitemap:loc", resolver);
            this.logger.Info("Collecting " + iterator.Count.ToString() + " items.");
            while (iterator.MoveNext())
            {
                if (iterator.Current == null)
                {
                    continue;
                }

                /*var pubDate = iterator.Current.SelectSingleNode("pubDate");

                DateTime publishDate;
                try
                {
                    publishDate = pubDate != null ? Convert.ToDateTime(pubDate.Value) : DateTime.Now;
                }
                catch (Exception)
                {
                    publishDate = DateTime.Now;
                }*/

                collectionPages.Add(new AppleseedModuleItem()
                {
                    Key = String.Format("{0:X}", iterator.Current.Value.GetHashCode()),
                    Path = iterator.Current.Value,
                    ModuleID = 0,
                    PageID = 0,
                    PortalID = 0,
                    Type = CollectionItemType.PAGE,
                    ViewRoles = CollectionItemSecurityType.PUBLIC,
                    PublishedDate =  DateTime.Now //TODO
                });
                this.logger.Info("Collected Page:" + iterator.Current.Value);
            }
        }
    }
}
