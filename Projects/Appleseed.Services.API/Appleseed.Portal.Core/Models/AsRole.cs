namespace Appleseed.Portal.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AsRole
    {
        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public Guid ApplicationId { get; set; }

        [Required, StringLength(256)]
        public string RoleName { get; set; }

        [Required, StringLength(256)]
        public string LoweredRoleName { get; set; }

        [StringLength(256)]
        public string Description { get; set; }
    }
}
