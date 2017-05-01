namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.XPath;
    using System.IO;

    using Common.Logging;

    using Appleseed.Services.Core.Extractors;
    using Appleseed.Services.Base.Model;

    /// <summary>
    /// Get all the links from an RSS feed.  These links will be used to extract content.
    /// </summary>
    public class RssContentCollector : ICollectThings
    {
        private readonly ILog Logger;

        private readonly Core.Extractors.IUrlContentExtractor ContentExtractor;

        private readonly string RssFeedUrl;

        private List<Model.BaseItem> CollectedItems { get; set; }

        public RssContentCollector(ILog logger, IUrlContentExtractor contentExtractor, string rssFeedUrl)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (contentExtractor == null)
            {
                throw new ArgumentNullException("contentExtractor");

            }

            if (string.IsNullOrEmpty(rssFeedUrl))
            {
                throw new ArgumentNullException("rssFeedUrl");
            }

            this.Logger = logger;
            this.RssFeedUrl = rssFeedUrl;
            this.ContentExtractor = contentExtractor;
        }

        public List<Model.BaseItem> CollectItems()
        {
            this.CollectedItems = new List<Model.BaseItem>();
            if (!string.IsNullOrEmpty(this.RssFeedUrl))
            {
                this.CollectionItems(this.RssFeedUrl, this.CollectedItems);
            }

            return this.CollectedItems;
        }

        public void CollectItems(List<Model.BaseItem> items)
        {
            this.CollectionItems(this.RssFeedUrl, items);
        }

        private void CollectionItems(string rssFeed, List<Model.BaseItem> collectionPages)
        {
            if (string.IsNullOrEmpty(rssFeed))
            {
                throw new ArgumentNullException("rssFeed");
            }

            var content = this.ContentExtractor.Extract(rssFeed);
            using (var txtReader = new StringReader(content.Content))
            {
                using (var xmlReader = new XmlTextReader(txtReader))
                {
                    var document = new XPathDocument(xmlReader);
                    var navigator = document.CreateNavigator();
                    var iterator = navigator.Select("/rss/channel/item");

                    while (iterator.MoveNext())
                    {
                        if (iterator.Current == null)
                        {
                            continue;
                        }

                        var linkNode = iterator.Current.SelectSingleNode("link");
                        if (linkNode != null)
                        {
                            var link = linkNode.Value;
                            
                            DateTime publishDate;
                            try
                            {
                                var pubDate = iterator.Current.SelectSingleNode("pubDate");
                                publishDate = pubDate != null ? Convert.ToDateTime(pubDate.Value) : DateTime.Now;
                            }
                            catch (Exception)
                            {
                                publishDate = DateTime.Now;
                            }

                            collectionPages.Add(
                                new AppleseedModuleItem()
                                    {
                                        Key = String.Format("{0:X}", link.GetHashCode()), 
                                        Path = link,
                                        ModuleID = 0,
                                        PageID = 0,
                                        PortalID = 0,
                                        Type = CollectionItemType.PAGE,
                                        ViewRoles = CollectionItemSecurityType.PUBLIC,
                                        PublishedDate = publishDate
                                    });

                            this.Logger.Info("Collected Rss Feed Item:" + link);
                        }
                    }
                }
            }
        }
    }
}
