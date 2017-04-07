namespace Appleseed.Portal.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Module
    {
        public Guid GeneralModDefID { get; set; }

        [Required, StringLength(128)]
        public string FriendlyName { get; set; }

        [Required, StringLength(256)]
        public string DesktopSrc { get; set; }

        [Required, StringLength(256)]
        public string MobileSrc { get; set; }

        [Required, StringLength(50)]
        public string AssemblyName { get; set; }

        [StringLength(128)]
        public string ClassName { get; set; }
     
        public bool Admin { get; set; }
        
        public bool Searchable { get; set; }
    }
}
