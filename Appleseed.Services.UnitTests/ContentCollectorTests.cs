using System.Collections.Generic;

namespace Appleseed.Services.UnitTests
{
    using Appleseed.Services.Core.Extractors.Impl;
    using Appleseed.Services.Base.Engine.Processors.Impl;
    using Common.Logging;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ContentCollectorTests
    {

        #region Rss
        [Test]
        public void RssContentCollector_ValidRssFeed_ShouldSucceed()
        {
            // Arrange
            const string url = "http://feeds.abcnews.com/abcnews/topstories";
            var logMock = new Moq.Mock<ILog>();
            var collector = new RssContentCollector(logMock.Object, new XmlExtractor(), url);

            // Act
            var content = collector.CollectItems();

            // Assert
            Assert.IsNotNull(content);
            Assert.IsTrue(content.Count > 0);
        }

        [Test]
        public void RssContentCollector2_ValidRssFeed_ShouldSucceed()
        {
            // Arrange
            const string url = "http://feeds.abcnews.com/abcnews/topstories";
            var logMock = new Moq.Mock<ILog>();
            var collector = new RssContentCollector(logMock.Object, new XmlExtractor(), url);
            var content = new List<Base.Model.BaseItem>();

            // Act
            collector.CollectItems(content);

            // Assert
            Assert.IsNotNull(content);
            Assert.IsTrue(content.Count > 0);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void RssContentCollector_BlankRssFeed_ShouldFail()
        {
            // Arrange
            const string url = "";
            var logMock = new Moq.Mock<ILog>();
            var collector = new RssContentCollector(logMock.Object, new XmlExtractor(), url);

            // Act

            // Assert
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void RssContentCollector_NullExtractor_ShouldFail()
        {
            // Arrange
            const string url = "http://feeds.abcnews.com/abcnews/topstories";
            var logMock = new Moq.Mock<ILog>();
            var collector = new RssContentCollector(logMock.Object, null, url);

            // Act

            // Assert
        }

        [Test]
        [ExpectedException("System.Net.WebException")]
        public void RssContentCollector_InvalidRssFeed_ShouldFail()
        {
            // Arrange
            const string url = "http://feeds.abcnews.com/abcnews/topstorie";
            var logMock = new Moq.Mock<ILog>();
            var collector = new RssContentCollector(logMock.Object, new XmlExtractor(), url);

            // Act
            var content = collector.CollectItems();

            // Assert
        }
#endregion

        #region SiteMap
        [Test]
        public void SiteMapCollector_ValidSiteMap_ShouldSucceed()
        {
            // Arrange
            const string url = "http://www.anant.us/sitemap.axd";
            var mockLog = new Mock<ILog>();
            var collector = new SiteMapCollector(mockLog.Object, url, new XmlExtractor());

            // Act
            var content = collector.CollectItems();

            // Assert
            Assert.IsNotNull(content);
            Assert.IsTrue(content.Count > 0);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void SiteMapCollector_BlankSiteMap_ShouldFail()
        {
            // Arrange
            const string url = "";
            var mockLog = new Mock<ILog>();
            var collector = new SiteMapCollector(mockLog.Object, url, new XmlExtractor());

            // Act

            // Assert
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void SiteMapCollector_NullExtractor_ShouldFail()
        {
            // Arrange
            const string url = "http://www.anant.us/sitemap.axd";
            var mockLog = new Mock<ILog>();
            var collector = new SiteMapCollector(mockLog.Object, url, null);

            // Act

            // Assert
        }
        #endregion
    }
}
