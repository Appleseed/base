namespace Appleseed.Services.Base.Engine.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    using Appleseed.Services.Base.Model;
    using Appleseed.Services.Core;

    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;
    using Lucene.Net.Search.Highlight;
    using Lucene.Net.Store;

    using Version = Lucene.Net.Util.Version;
    using System.Text.RegularExpressions;
   
    using BoboBrowse.Net;
    using BoboBrowse.Net.Facets.Impl;
    using BoboBrowse.Net.Facets;
    
   

    public class LuceneIndexSearchService : IAmASearchService
    {
        private readonly string pathToIndex;

        private readonly string pathToFileContent;

        public LuceneIndexSearchService(string pathToIndex, string pathToFileContent = "")
        {
            if (string.IsNullOrEmpty(pathToIndex))
            {
                throw new ArgumentNullException("pathToIndex");
            }

            this.pathToIndex = pathToIndex;
            this.pathToFileContent = pathToFileContent;
        }

        public IEnumerable<string> GetSearchPredictions(SearchRequest request)
        {
            var frequencyMap = new Dictionary<string, int>();

            var termlist = new List<string>();
            using (var directory = new SimpleFSDirectory(new DirectoryInfo(this.pathToIndex)))
            {
                using (var reader = DirectoryReader.Open(directory, true))
                {
                    GetTermsForField(request.Query, "IndexContent", frequencyMap, termlist, reader);
                    GetTermsForField(request.Query, "SmartItemKeywords", frequencyMap, termlist, reader);
                }
            }

            var sortedlist = from term in frequencyMap
                             orderby term.Value descending
                             select term.Key;

            return sortedlist.ToList();
        }

        public SearchResult GetSearchResults(SearchRequest request)
        {
            var result = new SearchResult { Data = new List<CollectionIndexItem>(), FacetContent = new Dictionary<string,List<string>>() };
            
            // parse the query, "text" is the default field to search
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            var contentParser = new QueryParser(Version.LUCENE_30, "ItemContent", analyzer) { AllowLeadingWildcard = true };
            var contentQuery = contentParser.Parse(QueryParser.Escape(request.Query) + "*");
            var nameParser = new QueryParser(Version.LUCENE_30, "ItemName", analyzer) { AllowLeadingWildcard = true };
            var nameQuery = nameParser.Parse(QueryParser.Escape(request.Query) + "*");
            Query combinedQuery = contentQuery.Combine(new[] { contentQuery, nameQuery });

            // create highlighter
            var formatter = new SimpleHTMLFormatter("<span style=\"font-weight:bold;\">", "</span>");
            var fragmenter = new SimpleFragmenter(80);
            var scorer = new QueryScorer(combinedQuery);
            var highlighter = new Highlighter(formatter, scorer);

            highlighter.TextFragmenter = fragmenter;

             
            //------------------  Use BoBoBrowse for Facet, if filters is NOT empty  ------------------//
            if (request.FacetFilters.Keys.Count != 0)
            {
                // 1. open Lucene Index with index reader
                using (var reader = IndexReader.Open(FSDirectory.Open(this.pathToIndex), true))
                {
                    var faceHandlers = new FacetHandler[request.FacetFilters.Keys.Count];
                    int i = 0;
                    foreach (var facetFilterKey in request.FacetFilters.Keys)
                    {
                        faceHandlers[i] = new MultiValueFacetHandler(facetFilterKey);
                        i++;
                    }
                    // 2. decorate it with bobo index reader
                    using (BoboIndexReader boboReader = BoboIndexReader.GetInstance(reader, faceHandlers))
                    {
                        // 3. retrieve documents with stored fields
                        BrowseRequest browseRequest = new BrowseRequest();
                        browseRequest.Count = 1000;
                        browseRequest.Offset = 0;
                        browseRequest.FetchStoredFields = true;

                        // 4. edit facet selection, and filter out specific results
                        foreach (var fieldContentPair in request.FacetFilters)
                        {
                            BrowseSelection sel = new BrowseSelection(fieldContentPair.Key);
                            // If filterContent is NOT empty, add filter condition and values
                            if (fieldContentPair.Value.Count != 0)
                            {
                                foreach (string filterContent in fieldContentPair.Value)
                                {
                                    string filterNew = filterContent;
                                    Regex filterReg = new Regex(@"\([0-9]+\)");
                                    var matches = filterReg.Matches(filterContent);
                                    if (matches.Count != 0)
                                    {
                                        Match match = matches[matches.Count - 1];
                                        filterNew = filterContent.Substring(0, match.Index);
                                        sel.AddValue(filterNew);
                                    }
                                    else
                                    {
                                        if (filterNew != "")
                                        {
                                            sel.AddValue(filterNew);
                                        }
                                    }
                                    
                                }
                            }
                            // else, only add filter condition
                            browseRequest.AddSelection(sel);
                        }

                        // 5. parse the query
                        browseRequest.Query = combinedQuery;

                        // 6. add the facet output specs
                        foreach (string facetFilterKey in request.FacetFilters.Keys)
                        {
                            FacetSpec facetSpec = new FacetSpec();
                            facetSpec.OrderBy = FacetSpec.FacetSortSpec.OrderHitsDesc;
                            browseRequest.SetFacetSpec(facetFilterKey, facetSpec);
                        }

                        // 7. perform browse
                        IBrowsable browser = new BoboBrowser(boboReader);
                        BrowseResult browseResult = browser.Browse(browseRequest);

                        // 8. generate facetContent Dictionary and get finalHits
                        result.TotalRecordsFound = browseResult.NumHits;
                        BrowseHit[] hits = browseResult.Hits;

                        Dictionary<String, IFacetAccessible> facetMap = browseResult.FacetMap;
                        foreach (string facetFilterKey in request.FacetFilters.Keys)
                        {
                            IFacetAccessible facets = facetMap[facetFilterKey];
                            IEnumerable<BrowseFacet> facetVals = facets.GetFacets();
                            string key = facetFilterKey;
                            List<string> value = new List<string>();
                            // only show the top 10 facets for each facet group
                            int numFacets = Math.Min(facetVals.Count(), 10);
                            for (int j = 0; j < numFacets; j++)
                            {
                                value.Add(facetVals.ToList()[j].ToString());
                            }
                            result.FacetContent.Add(key, value);
                        }

                        // 8. generate resultDictionary and update finalHits
                        // how many items we should show - less than defined at the end of the results
                        var start = (request.PageNumber - 1) * request.RecordsPerPage;
                        var resultsCount = Math.Min(result.TotalRecordsFound, request.RecordsPerPage + start);
                        for (int k = start; k < resultsCount; k++)
                        {
                            var doc = hits[k];
                            var resultItem = new CollectionIndexItem
                            {
                                ItemKey = doc.StoredFields.Get("ItemKey"),
                                ItemFileSize = doc.StoredFields.Get("ItemFileSize"),
                                ItemPath = doc.StoredFields.Get("ItemPath"),
                                ItemType = doc.StoredFields.Get("ItemType")
                            };

                            var itemRawContent = Appleseed.Services.Core.Helpers.Utilities.RemoveHtml(doc.StoredFields.Get("ItemContent"));

                            using (var stream = analyzer.TokenStream(string.Empty, new StringReader(itemRawContent)))
                            {
                                switch (resultItem.ItemType)
                                {
                                    case "File":
                                        if (!string.IsNullOrEmpty(this.pathToFileContent))
                                        {
                                            resultItem.ItemPath = resultItem.ItemPath.Replace(this.pathToFileContent, "/");
                                            resultItem.ItemPath = resultItem.ItemPath.Replace("\\", "/");
                                        }

                                        break;
                                    case "Page":
                                        resultItem.ItemPath = resultItem.ItemPath;
                                        break;
                                    default:
                                        resultItem.ItemPath = resultItem.ItemPath;
                                        break;
                                }

                                resultItem.ItemName = doc.StoredFields.Get("ItemName");
                                try
                                {
                                    resultItem.ItemSummary = highlighter.GetBestFragments(stream, itemRawContent, 4, "...");
                                }
                                catch (Exception ex)
                                {
                                    resultItem.ItemSummary = itemRawContent.Substring(0, 100);
                                }

                                // TODO: May be too heavy for now -- until we can cache this data
                                //resultItem.ItemContent = doc.Get("ItemContent");
                            }

                            result.Data.Add(resultItem);
                        }

                        boboReader.Dispose();
                    }

                    reader.Dispose();
                }

            }


            //------------------  No Facet, if filters is empty  ------------------//
            else
            {
                using (var searcher = new IndexSearcher(FSDirectory.Open(this.pathToIndex)))
                {
                    // search
                    var hits = searcher.Search(combinedQuery, request.RecordsPerPage + ((request.PageNumber - 1) * request.RecordsPerPage));
                    result.TotalRecordsFound = hits.TotalHits;

                    // how many items we should show - less than defined at the end of the results
                    var start = (request.PageNumber - 1) * request.RecordsPerPage;
                    var resultsCount = Math.Min(result.TotalRecordsFound, request.RecordsPerPage + start);
                    for (int i = start; i < resultsCount; i++)
                    {
                        // get the document from index
                        var doc = searcher.Doc(hits.ScoreDocs[i].Doc);

                        var resultItem = new CollectionIndexItem
                        {
                            ItemKey = doc.Get("ItemKey"),
                            ItemFileSize = doc.Get("ItemFileSize"),
                            ItemPath = doc.Get("ItemPath"),
                            ItemType = doc.Get("ItemType")
                        };

                        var itemRawContent = Appleseed.Services.Core.Helpers.Utilities.RemoveHtml(doc.Get("ItemContent"));

                        using (var stream = analyzer.TokenStream(string.Empty, new StringReader(itemRawContent)))
                        {
                            switch (resultItem.ItemType)
                            {
                                case "File":
                                    if (!string.IsNullOrEmpty(this.pathToFileContent))
                                    {
                                        resultItem.ItemPath = resultItem.ItemPath.Replace(this.pathToFileContent, "/");
                                        resultItem.ItemPath = resultItem.ItemPath.Replace("\\", "/");
                                    }

                                    break;
                                case "Page":
                                    resultItem.ItemPath = resultItem.ItemPath;
                                    break;
                                default:
                                    resultItem.ItemPath = resultItem.ItemPath;
                                    break;
                            }

                            resultItem.ItemName = doc.Get("ItemName");
                            try
                            {
                                resultItem.ItemSummary = highlighter.GetBestFragments(stream, itemRawContent, 4, "...");
                            }
                            catch (Exception ex)
                            {
                                resultItem.ItemSummary = itemRawContent.Substring(0, 100);
                            }
                            // TODO: May be too heavy for now -- until we can cache this data
                            //resultItem.ItemContent = doc.Get("ItemContent");
                        }

                        result.Data.Add(resultItem);
                    }

                    searcher.Dispose();
                }
            }

            return result;
        }



        private static void GetTermsForField(string termStartsWith, string termField, Dictionary<string, int> frequencyMap, List<string> termlist, IndexReader reader)
        {
            // get terms from the field 
            TermEnum terms = reader.Terms(new Term(termField, termStartsWith));

            while (terms.Next())
            {
                Term term = terms.Term;
                string termText = term.Text;
                int frequency = reader.DocFreq(term);
                if (!frequencyMap.ContainsKey(termText))
                {
                    frequencyMap.Add(termText, frequency);
                    termlist.Add(termText);
                }
            }
        }
    }
}