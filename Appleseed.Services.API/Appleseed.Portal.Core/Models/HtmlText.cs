namespace Appleseed.Portal.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class HtmlText
    {
        public int ModuleId { get; set; }

        [Required]
        public string DesktopHtml { get; set; }

        [Required]
        public string MobileSummary { get; set; }

        [Required]
        public string MobileDetails { get; set; }

        public int VersionNo { get; set; }

        [Required]
        public bool Published { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public System.DateTime CreatedDate { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        [Required]
        public System.DateTime ModifiedDate { get; set; }
    }
}
