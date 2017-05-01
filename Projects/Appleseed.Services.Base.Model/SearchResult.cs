using System.Collections.Generic;

namespace Appleseed.Services.Base.Model
{
    public class SearchResult
    {
        public SearchRequest SearchRequest { get; set; }

        public IList<CollectionIndexItem> Data { get; set; }

        public int TotalRecordsFound { get; set; }

        public Dictionary<string, List<string>> FacetContent { get; set; }
    }
}
