namespace Appleseed.Services.UnitTests
{
    using Appleseed.Services.Core.Extractors.Impl;

    using NUnit.Framework;

    [TestFixture]
    public class ExtractorTests
    {
        [Test]
        public void HtmlExtractor_WebContent_ShouldSucceed()
        {
            // Arrange
            const string PageURL = "http://askfsis.custhelp.com/app/answers/list";
            var extractor = new Core.Extractors.Impl.HtmlContentExtractor();

            var content = extractor.Extract(PageURL);
            Assert.IsFalse(string.IsNullOrEmpty(content.Content));
        }

        [Test]
        public void XmlExtractor_ValidRssFeed_ShouldSucceed()
        {
            // Arrange
            const string FeedUrl = "http://feeds.abcnews.com/abcnews/topstories";
            var extractor = new XmlExtractor();

            // Act
            var content = extractor.Extract(FeedUrl);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(content.Content));
        }
    }
}
