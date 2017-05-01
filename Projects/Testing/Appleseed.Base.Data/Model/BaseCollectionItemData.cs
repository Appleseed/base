using System;

namespace Appleseed.Base.Data.Model
{
    [Serializable]
    public class BaseCollectionItemData
    {





        public long ItemID { get; set; }

        public string Type { get; set; }

        public string Source { get; set; }
        // Item Privacy Fields
        public string ViewRoles { get; set; }

        public int PortalID { get; set; }

        public int ModuleID { get; set; }

        public int PageID { get; set; }

        public long FileSize { get; set; }


        public string Key { get; set; }

        // Item Identity Fields
        public string Path { get; set; }

        public string Name { get; set; }

        public DateTime PublishedDate { get; set; }



        public DateTime ItemCreatedDate { set; get; }
        public DateTime ItemProcessedDate { set; get; }
        public bool ItemProcessed { set; get; }
        public string ItemQueue { set; get; }





        //public long ItemID { get; set; }
        //public string ItemPath { set; get; }
        //public string ItemUrl { set; get; }
        //public string ItemName { set; get; }
        //public string ItemTitle { set; get; }
        //public string ItemDescription { set; get; }
        //public string ItemSummary { set; get; }
        //public string ItemContent_Raw { set; get; }
        //public string ItemContent_Rich { set; get; }
        //public string ItemContent_Text { set; get; }
        //public string ItemContent_Image { set; get; }
        //public string ItemContent_Image_Url { set; get; }
        //public string ItemTags { set; get; }
        //public string ItemKeywords { set; get; }
        //public string ItemCategories { set; get; }
        //public DateTime ItemCreatedDate { set; get; }
        //public DateTime ItemProcessedDate { set; get; }
        //public bool ItemProcessed { set; get; }
        //public string ItemQueue { set; get; }
    }
}