namespace Appleseed.Portal.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Portal
    {
        public int PortalId { get; set; }

        [Required, StringLength(128)]
        public string PortalAlias { get; set; }

        [Required, StringLength(128)]
        public string PortalName { get; set; }

        [StringLength(128)]
        public string PortalPath { get; set; }

        [Required]
        public bool AlwaysShowEditButton { get; set; }
    }
}
