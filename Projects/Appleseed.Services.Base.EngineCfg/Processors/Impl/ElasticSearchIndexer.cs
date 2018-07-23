namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Logging;
    using System.Threading.Tasks;
    using Nest;
    using Appleseed.Services.Base.Engine.Processors.Impl.ElasticSearchModel;
    using System.Text.RegularExpressions;

    public class ElasticSearchIndexer : IIndexThings
    {
        private readonly Common.Logging.ILog logger;

        private readonly string indexPath;

        public ElasticSearchIndexer(ILog logger, string pathToIndex)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(pathToIndex))
            {
                throw new ArgumentNullException("pathToIndex");
            }

            this.indexPath = pathToIndex;
            this.logger = logger;
        }

        public void Build(IEnumerable<Model.AppleseedModuleItemIndex> indexableData)
        {
            if (indexableData.Count().Equals(0))
            {
                throw new Exception("Nothing to index");
            }

            Regex reg = new Regex(@"[a-z0-9]+$");
            Match contentMatch = reg.Match(indexPath);
            var indexSep = contentMatch.Index;
            string elasticUri = indexPath.Substring(0, indexSep - 1);
            string indexName = indexPath.Substring(indexSep);

            var node = new Uri(elasticUri);
//            var settings = new ConnectionSettings(node,defaultIndex: indexName);
            var settings = new ConnectionSettings(node);
            var client = new ElasticClient(settings);

                var indexItemTasks = new List<Task>();
                int errorCount = 0;
                var index = 0;

                foreach (var data in indexableData)
                {
                    indexItemTasks.Add(Task.Factory.StartNew(() =>
                    {
                        logger.Info(index + ". Index Start: " + data.Path);

                        if (!string.IsNullOrEmpty(data.Content))
                        {
                            var indexItem = new ElasticSearchItem();
                            indexItem.ItemPath = data.Path;
                            indexItem.ItemName = data.Name;
                            indexItem.ItemPortalID = data.PortalID.ToString();
                            indexItem.ItemModuleID = data.ModuleID.ToString();
                            indexItem.ItemKey = data.Key;
                            indexItem.ItemType = data.Type;
                            indexItem.ItemContent = data.Content;
                            indexItem.ItemPageID = data.PageID.ToString();
                            indexItem.ItemFileSize = data.FileSize.ToString();
                            indexItem.ItemSummary = data.Summary;
                            indexItem.ItemViewRoles = data.ViewRoles;
                            indexItem.ItemCreatedDate = data.CreatedDate;
                            if (!string.IsNullOrEmpty(data.SmartKeywords)) indexItem.SmartItemKeywords = data.SmartKeywords;
                            if (!string.IsNullOrEmpty(data.Source)) indexItem.ItemSource = data.Source;

                            client.IndexDocument<ElasticSearchItem>(indexItem);
                        }
                    },
                        TaskCreationOptions.LongRunning).ContinueWith(t =>
                        {
                            if (t.Exception != null)
                            {
                                logger.Info(index + ". Index End EXCEPTION : " /* + t.Status */ + string.Empty + data.Path);
                                logger.Info("Data: " + data.ToDebugString());
                                logger.Error(t.Exception);
                                errorCount++;
                            }
                            else
                            {

                                logger.Info(index + ". Index End  : " /* + t.Status */ + string.Empty + data.Path);
                            }
                        }));

                    index++;
                }

                Task.WaitAll(indexItemTasks.ToArray());

                logger.Info("----------------INDEXER SUMMARY--------------------------------");
                logger.Info("Success Count: " + (indexItemTasks.Count - errorCount).ToString());
                logger.Info("Error Count: " + errorCount.ToString());
                logger.Info("----------------INDEXER SUMMARY--------------------------------");
            }
    }
}
