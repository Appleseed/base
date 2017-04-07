using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appleseed.Services.Base.Engine.Processors.Impl.ElasticSearchModel
{
    class ElasticSearchItem
    {
        public string ItemPath { get; set; }
        public string ItemName { get; set; }
        public string ItemPortalID { get; set; }
        public string ItemModuleID { get; set; }
        public string ItemKey { get; set; }
        public string ItemType { get; set; }
        public string ItemContent { get; set; }
        public string ItemPageID { get; set; }
        public string ItemFileSize { get; set; }
        public string ItemSummary { get; set; }
        public string ItemViewRoles { get; set; }
        public string ItemCreatedDate { get; set; }
        public string SmartItemKeywords { get; set; }
        public string ItemSource { get; set; }
    }
}
