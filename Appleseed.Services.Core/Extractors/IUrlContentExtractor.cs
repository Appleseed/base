namespace Appleseed.Services.Core.Extractors
{
    using Appleseed.Services.Core.Models;

    public interface IUrlContentExtractor
    {
        ExtractedWebContent Extract(string url);
    }
}
