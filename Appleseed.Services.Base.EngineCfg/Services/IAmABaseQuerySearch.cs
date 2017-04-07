namespace Appleseed.Services.Base.Engine.Services
{
    using Appleseed.Services.Base.Model;
    using System.Collections.Generic;

    public interface IAmASearchService
    {
        IEnumerable<string> GetSearchPredictions(SearchRequest request);

        SearchResult GetSearchResults(SearchRequest request);
    }
}
