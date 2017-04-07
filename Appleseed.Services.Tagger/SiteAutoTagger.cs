using Appleseed.Services.Core.Extractors;
using Appleseed.Services.Core.Extractors.Impl;

namespace Appleseed.Services.Tagger
{
    using Common.Logging;
    using Core.Helpers;
    //using Knowledge.Model.Illumination.Models;
    //using Knowledge.Model.Illumination.Services.OpenCalaisIlluminator;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    public class SiteAutoTagger
    {
        private const string GetPageKeywordsSql = "SELECT SettingValue FROM [rb_TabSettings] WHERE TabID = @TabID AND SettingName = 'TabMetaKeyWords'";

        private const string InsertPageKeywordsSql = "INSERT INTO [rb_TabSettings] (TabID, SettingName, SettingValue) VALUES (@TabID, 'TabMetaKeyWords', @SettingValue)";

        private const string UpdatePageKeywordsSql = "UPDATE [rb_TabSettings] SET SettingValue = @SettingValue WHERE TabID = @TabID AND SettingName = 'TabMetaKeyWords'";

        private const string PageHasKeywordsSql = "SELECT COUNT(*) FROM [rb_TabSettings] WHERE TabID = @TabID AND SettingName = 'TabMetaKeyWords'";

        private readonly ILog logger;

        private readonly IUrlContentExtractor contentExtractor;

        private readonly string databaseConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteAutoTagger"/> class. 
        /// Adds Metadata to Appleseed site.  Defaults to HtmlContentExtractor
        /// </summary>
        /// <param name="logger">
        /// </param>
        /// <param name="databaseConnectionString">
        /// </param>
        public SiteAutoTagger(ILog logger, string databaseConnectionString)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(databaseConnectionString))
            {
                throw new ArgumentNullException("databaseConnectionString", "No database connection string was provided to the Site Tagging Service.");
            }

            this.logger = logger;
            this.databaseConnectionString = databaseConnectionString;
            this.contentExtractor = new HtmlContentExtractor();
        }

        public SiteAutoTagger(IUrlContentExtractor extractor, string databaseConnectionString, ILog logger)
        {
            if (extractor == null)
            {
                throw new ArgumentNullException("extractor", "No web page extractor was provided to the Site Tagging Service.");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(databaseConnectionString))
            {
                throw new ArgumentNullException("databaseConnectionString", "No database connection string was provided to the Site Tagging Service.");
            }

            this.logger = logger;
            this.contentExtractor = extractor;
            this.databaseConnectionString = databaseConnectionString;
        }

        public void Tag(string baseUrl, int portalId)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException("baseUrl", "No valid base url was provided to the Site Tagging Service.");
            }

            //if (portalId <= 0)
            //{
            //    throw new ArgumentNullException("portalId", "No portal ID was provided to the Site Tagging Service.");
            //}

            var pageIds = this.GetPageIds(portalId);
            this.logger.Info(string.Format("Tagging {0} Pages", pageIds.Count()));
            var i = 1;
            foreach (var pageId in pageIds)
            {
                var url = string.Format("{0}/{1}", baseUrl, pageId);
                this.logger.Info(i + ": Extracting page: " + url);
                var extractedPage = this.contentExtractor.Extract(url);
                if (extractedPage != null && !string.IsNullOrEmpty(extractedPage.Content))
                {
                    this.logger.Info(i + ": Enlightening page content.");
                    //var enlightenedResults =  this.IlluminatePageContent(extractedPage.Content);
                    //var tags = (from o in enlightenedResults select o.Value).ToList();
                    //this.logger.Info(i + ": Updating database");
                    //this.UpdatePageTags(pageId, string.Join(",", tags));
                }

                i++;
            }
        }

        private static void InsertPageKeywords(string connectionString, int pageId, string filteredTags)
        {
            var sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@TabID", pageId);
            sqlParams[1] = new SqlParameter("@SettingValue", filteredTags);
            SqlClientHelper.ExecuteNonQuery(connectionString, CommandType.Text, InsertPageKeywordsSql, sqlParams);
        }

        private static void UpdatePageKeywords(string connectionString, int pageId, string filteredTags)
        {
            var sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@SettingValue", filteredTags);
            sqlParams[1] = new SqlParameter("@TabID", pageId);
            SqlClientHelper.ExecuteNonQuery(connectionString, CommandType.Text, UpdatePageKeywordsSql, sqlParams);
        }

        private static string GetPageKeywords(string connectionString, int pageId)
        {
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@TabID", pageId);
            var keywords = SqlClientHelper.ExecuteScalar(connectionString, CommandType.Text, GetPageKeywordsSql, sqlParams);
            return keywords;
        }

        private static bool PageHasKeywords(string connectionString, int pageId)
        {
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@TabID", pageId);
            var countString = SqlClientHelper.ExecuteScalar(connectionString, CommandType.Text, PageHasKeywordsSql, sqlParams);
            var count = int.Parse(countString);
            return count > 0;
        }

        // Get all page ids
        private IEnumerable<int> GetPageIds(int portalId)
        {
            var results = new List<int>();
            const string Sql = "SELECT PageID FROM rb_Pages WHERE AuthorizedRoles LIKE '%All Users%' AND PortalID = @PortalID";
            ////var sql = "SELECT PageID FROM rb_Pages WHERE PortalID = @PortalID";
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@PortalID", SqlDbType.Int, 4) { Value = portalId };

            // Read the result set
            using (var reader = SqlClientHelper.GetReader(this.databaseConnectionString, CommandType.Text, Sql, sqlParams))
            {
                while (reader.Read())
                {
                    results.Add((int)reader["PageID"]);
                }
            }

            return results;
        }

        // run opencalais on data
        //private IEnumerable<IlluminationResult> IlluminatePageContent(string content)
        //{
        //    var illuminator = new CalaisIlluminationService();
        //    var result = illuminator.Illuminate(content);
        //    return result;
        //}

        private void UpdatePageTags(int pageId, string newTags)
        {
            var currentTags = GetPageKeywords(this.databaseConnectionString, pageId);
            var pageHasKeywordsRecord = PageHasKeywords(this.databaseConnectionString, pageId);
            if (!string.IsNullOrEmpty(currentTags)) currentTags += ",";
            currentTags += newTags;

            // cleanup the tags
            var allTags = currentTags.Split(',');
            for (var i = 0; i < allTags.Count(); i++)
            {
                allTags[i] = allTags[i].Trim().ToLower();
            }

            // grab distinct tags to prevent duplicates
            var filteredTags = string.Join(",", allTags.Distinct());

            // save back to db
            if (pageHasKeywordsRecord)
            {
                UpdatePageKeywords(this.databaseConnectionString, pageId, filteredTags);
            }
            else
            {
                InsertPageKeywords(this.databaseConnectionString, pageId, filteredTags);
            }
        }
    }
}
