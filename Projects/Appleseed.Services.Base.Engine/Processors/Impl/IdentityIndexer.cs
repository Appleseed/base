namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Common.Logging;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.Store;

    using Directory = Lucene.Net.Store.Directory;
    using Version = Lucene.Net.Util.Version;

    public class IdentityIndexer : IIndexThings
    {
        private readonly Common.Logging.ILog logger;

        public IdentityIndexer(ILog logger, string pathToIndex)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(pathToIndex))
            {
                throw new ArgumentNullException("pathToIndex");
            }

            if (System.IO.Directory.Exists(pathToIndex))
            {
                System.IO.Directory.Delete(pathToIndex, true);
            }

            this.logger = logger;
            this.luceneIndexDirectory = FSDirectory.Open(pathToIndex); // Version issue... 
            this.writer = new IndexWriter(this.luceneIndexDirectory, this.analyzer, true, IndexWriter.MaxFieldLength.LIMITED);
        }

        private Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);

        private Directory luceneIndexDirectory;

        private IndexWriter writer;

        ////private string indexLocal;

        public IEnumerable<Model.AppleseedModuleItemIndex> IndexableData { get; set; }

        // 3. Build the index 
        public void Build(IEnumerable<Model.AppleseedModuleItemIndex> indexableData) // should go back and create a generic class on get/set, after url is read, application needs to consume data from the XML file, parse RSS or perform a DB query.
        {
            if (indexableData.Count().Equals(0))
            {
                throw new Exception("Nothing to index");
            }

            List<Task> indexItemTasks = new List<Task>();
            var index = 0;

            foreach(var indexableItem in this.IndexableData)
            {
                var data = indexableItem;

                indexItemTasks.Add(
                    Task.Factory.StartNew(() =>
                    {
                        this.logger.Info(index + ". Index Start: " + data.Path);

                        Document indexItem = new Document();
                        indexItem.Add(new Field("ItemPath", data.Path, Field.Store.YES, Field.Index.ANALYZED));
                        indexItem.Add(new Field("ItemName", data.Name, Field.Store.YES, Field.Index.ANALYZED));
                        indexItem.Add(new Field("ItemPortalID", data.PortalID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED)); // do we need analysis on Ids?
                        indexItem.Add(new Field("ItemModuleID", data.ModuleID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED)); // do we need analysis on Ids?
                        indexItem.Add(new Field("ItemKey", data.Key.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        //indexItem.Add(new Field("ItemType", data.Type, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        indexItem.Add(new Field("ItemContent", data.Content, Field.Store.YES, Field.Index.ANALYZED));
                        //indexItem.Add(new Field("ItemPageID", data.PageID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED)); // added from Collection_Index_Item_Pages as there was only one unique field - should this class 
                        indexItem.Add(new Field("ItemSummary", data.Summary, Field.Store.YES, Field.Index.ANALYZED));
                        //indexItem.Add(new Field("ItemViewRoles", data.ViewRoles, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        indexItem.Add(new Field("ItemCreatedDate", data.CreatedDate, Field.Store.YES, Field.Index.NOT_ANALYZED));

                        indexItem.Add(new Field("SmartItemKeywords", data.SmartKeywords, Field.Store.YES, Field.Index.ANALYZED));

                        this.writer.AddDocument(indexItem);
                    }, 
                    TaskCreationOptions.LongRunning).ContinueWith(t => 
                    {
                        this.logger.Info(index + ". Index End  : " /*+ t.Status */ + data.Path);
                    }));

                index++;
            }

            Task.WaitAll(indexItemTasks.ToArray());

            this.writer.Optimize();
            this.writer.Commit();
            this.writer.Dispose();
            this.luceneIndexDirectory.Dispose();
        }
    }
}
