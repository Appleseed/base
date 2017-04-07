﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appleseed.Services.Base.Model
{
    /// <summary>
    /// Collection item. Specific overides related to what we currently call "CollectionItem" which is used in Appleseed.Search 
    /// as the base model that is used in processing, indexing, and retrieval of information. 
    /// </summary>
    public class BaseCollectionItemObject : BaseCollectionItem
    {
        public new List<string> ItemTags { set; get; }
        public new List<string> ItemKeywords { set; get; }
        public new List<string> ItemCategories { set; get; }
    }
}
