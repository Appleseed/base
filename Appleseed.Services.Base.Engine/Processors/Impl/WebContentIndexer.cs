namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Common.Logging;

    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.Store;
    using Version = Lucene.Net.Util.Version;

    /// <summary>
    /// Creates a Lucene Index from Appleseed Module Items
    /// </summary>
    public class WebContentIndexer : IIndexThings
    {
        private readonly Common.Logging.ILog logger;

        private readonly string indexPath;

        public WebContentIndexer(ILog logger, string pathToIndex)
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


       

        // TODO: should go back and create a generic class on get/set, after url is read, application needs to 
        // consume data from the XML file, parse RSS or perform a DB query.


        /// <summary>
        /// Builds the Lucene Index
        /// </summary>
        /// <param name="indexableData">List of Appleseed Module Items to be indexed</param>
        public void Build(IEnumerable<Model.AppleseedModuleItemIndex> indexableData)
        {
            if (indexableData.Count().Equals(0))
            {
                throw new Exception("Nothing to index");
            }

           

            //TODO: determine how to deal with existing indices
            //if (System.IO.Directory.Exists(this.indexPath))
            //{
            //    System.IO.Directory.Delete(this.indexPath, true);
            //}

            

            int errorCount = 0;

            using (var luceneIndexDirectory = FSDirectory.Open(this.indexPath))
            {
                using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
                {

                    bool isEmpty = !System.IO.Directory.EnumerateFiles(this.indexPath).Any();
                    

                    // if there is no index files pls set it true, otherwise set it false
                    var writer = new IndexWriter(luceneIndexDirectory, analyzer, isEmpty, IndexWriter.MaxFieldLength.LIMITED);
                    using (writer)
                    {
                        var indexItemTasks = new List<Task>();
                        var index = 0;

                        foreach (var data in indexableData)
                        {
                            indexItemTasks.Add(Task.Factory.StartNew(() =>
                            {
                                logger.Info(index + ". Index Start: " + data.Path);

                                if (!string.IsNullOrEmpty(data.Content))
                                {
                                    var indexItem = new Document();
                                    indexItem.Add(new Field("ItemPath", data.Path, Field.Store.YES, Field.Index.ANALYZED));
                                    indexItem.Add(new Field("ItemName", data.Name, Field.Store.YES, Field.Index.ANALYZED));
                                    indexItem.Add(new Field("ItemPortalID", data.PortalID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED)); //do we need analysis on Ids?
                                    indexItem.Add(new Field("ItemModuleID", data.ModuleID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED)); //do we need analysis on Ids?
                                    indexItem.Add(new Field("ItemKey", data.Key, Field.Store.YES, Field.Index.NOT_ANALYZED));
                                    indexItem.Add(new Field("ItemType", data.Type, Field.Store.YES, Field.Index.NOT_ANALYZED));
                                    indexItem.Add(new Field("ItemContent", data.Content, Field.Store.YES, Field.Index.ANALYZED));
                                    indexItem.Add(new Field("ItemPageID", data.PageID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                                    indexItem.Add(new Field("ItemFileSize", data.FileSize.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                                    // added from Collection_Index_Item_Pages as there was only one unique field - should this class 
                                    indexItem.Add(new Field("ItemSummary", data.Summary, Field.Store.YES, Field.Index.ANALYZED));
                                    indexItem.Add(new Field("ItemViewRoles", data.ViewRoles, Field.Store.YES, Field.Index.NOT_ANALYZED));
                                    indexItem.Add(new Field("ItemCreatedDate", data.CreatedDate, Field.Store.YES, Field.Index.NOT_ANALYZED));
                                    if (!string.IsNullOrEmpty(data.SmartKeywords)) indexItem.Add(new Field("SmartItemKeywords", data.SmartKeywords, Field.Store.YES, Field.Index.ANALYZED));
                                    if (!string.IsNullOrEmpty(data.Source)) indexItem.Add(new Field("ItemSource", data.Source, Field.Store.YES, Field.Index.ANALYZED));

                                    writer.AddDocument(indexItem);
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

                        writer.Optimize();
                        writer.Commit();
                        writer.Dispose();
                    } // using writer

                    analyzer.Dispose();
                } // using analyzer

                luceneIndexDirectory.Dispose();
            } // using lucinedirectory
        }
    }
}
