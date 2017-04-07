using System;

namespace Appleseed.Services.Base.Model
{
    [Serializable]
    public class AppleseedModuleItem : BaseItem
    {
        public string Type { get; set; }

        public string Source { get; set; }
        // Item Privacy Fields
        public string ViewRoles { get; set; }

        public int PortalID { get; set; }
        
        public int ModuleID { get; set; }
        
        public int PageID { get; set; }

        public long FileSize { get; set; }
    }
}
