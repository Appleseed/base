namespace Appleseed.Services.Base.Engine.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Reflection;

    using Appleseed.Services.Base.Engine.Processors;
    using Appleseed.Services.Base.Model;

    public class IndexService : IAmABaseService
    {
        private readonly IAggregateData aggregator;
        private readonly ICollectThings collector;
        private readonly IExtractThings extractor;
        private readonly IOrganizeThings organizer;
        private readonly IIndexThings indexer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="aggregator"></param>
        /// <param name="extractor"></param>
        /// <param name="organizer"></param>
        /// <param name="indexer"></param>
        public IndexService(ICollectThings collector, IAggregateData aggregator, IExtractThings extractor, IOrganizeThings organizer, IIndexThings indexer)
        {
            this.collector = collector;
            this.aggregator = aggregator;
            this.extractor = extractor;
            this.organizer = organizer;
            this.indexer = indexer;
        }

        public bool Run()
        {
            //TODO use to create temporary data, but this needs to be used in different places, not just here. 
            //TODO create a unique id for each sequence / pipe
            var combinedHash = collector.GetHashCode().ToString() + aggregator.GetHashCode().ToString() + extractor.GetHashCode().ToString() + organizer.GetHashCode().ToString() + organizer.GetHashCode().ToString();
 
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Data";
            var xmlFile = path + "\\ExtractedData.xml";
            deleteCachedFile(xmlFile);

            // Delete OrganizeData as well
            xmlFile = path + "\\OrganizedData.xml";
            deleteCachedFile(xmlFile);

            this.aggregator.Aggregate();
            var items = this.collector.CollectItems();
            if (!items.Any())
            {
                return false;
            }

            var dataForIndexing = this.extractor.ExtractDataForIndexing(items);
            
            this.organizer.Organize(dataForIndexing);
            this.indexer.Build((IEnumerable<AppleseedModuleItemIndex>)dataForIndexing);

 
           
            return true;
        }

        void deleteCachedFile(string cacheXmlFile)
        {
            if (File.Exists(cacheXmlFile))
            {
                File.Delete(cacheXmlFile);
            }
        }
    }
}