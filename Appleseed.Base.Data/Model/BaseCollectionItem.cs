using System;
using Appleseed.Services.Base.Model;

namespace Appleseed.Base.Data.Model
{
    /// <summary>
    ///     Base collection item. The base item in the collection of leaves or elements in the Appleseed.Base system of things.
    /// </summary>
    public class BaseCollectionItem
    {
        public BaseCollectionItem()
        {
            Data = new BaseCollectionItemData();
        }

        public bool ItemProcessed { set; get; }
        public Guid Id { get; set; }
        public int TableId { get; set; }
        public BaseCollectionItemData Data { get; set; }
        //public AppleseedModuleItem Data { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AggregationType { get; set; }
    }
}