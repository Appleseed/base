namespace Appleseed.Services.Core.Extractors.Impl
{
    using System.Text;

    using Appleseed.Services.Core.Models;

    public class XmlExtractor : IUrlContentExtractor
    {
        public ExtractedWebContent Extract(string url)
        {
            var client = new System.Net.WebClient();
            var s = client.DownloadData(url);
            var data = Encoding.UTF8.GetString(s);
            return new ExtractedWebContent() { Content = data };
        }
    }
}
