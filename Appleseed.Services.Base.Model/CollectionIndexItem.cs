namespace Appleseed.Services.Base.Model
{
    public class CollectionIndexItem
    {
        // Item Unique Identity Fields
        public string ItemKey { get; set; }
        
        public int ItemPortalId { get; set; }
        
        public int ItemModuleId { get; set; }
        
        public int ItemPageId { get; set; }

        // Item Identity Fields
        public string ItemPath { get; set; }

        public string ItemFileSize { get; set; }
        
        public string ItemName { get; set; }
        
        public string ItemType { get; set; }

        // Item Privacy Fields
        public string ItemViewRoles { get; set; }

        // Item Content Fields
        public string ItemContent { get; set; }
        
        public string ItemSummary { get; set; }

        // Item Smart Content Fields
        public string SmartItemKeywords { get; set; }

        public string ItemCreatedDate { get; set; }
    }
}
