namespace Appleseed.Portal.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Page
    {
        public int PageID { get; set; }

        public int? ParentPageID { get; set; }

        [Required]
        public int PageOrder { get; set; }

        [Required]
        public int PortalID { get; set; }

        [Required, StringLength(50)]
        public string PageName { get; set; }

        [Required, StringLength(50)]
        public string MobilePageName { get; set; }

        [StringLength(512)]
        public string AuthorizedRoles { get; set; }

        [Required]
        public bool ShowMobile { get; set; }

        public int? PageLayout { get; set; }

        [Required, StringLength(50)]
        public string PageDescription { get; set; }
    }
}
