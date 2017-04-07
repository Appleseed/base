using System.Collections.Generic;
namespace Appleseed.Services.Base.Model
{
    public class SearchRequest
    {
        public string Query { get; set; }

        public int PageNumber { get; set; }

        public int RecordsPerPage { get; set; }

        public Dictionary<string, List<string>> FacetFilters { get; set; }
    }
}