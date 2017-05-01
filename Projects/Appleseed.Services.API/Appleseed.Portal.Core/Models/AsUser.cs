namespace Appleseed.Portal.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AsUser
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ApplicationId { get; set; }

        [Required, StringLength(256)]
        public string UserName { get; set; }

        [Required, StringLength(256)]
        public string LoweredUserName { get; set; }

        [StringLength(16)]
        public string MobileAlias { get; set; }

        [Required]
        public bool IsAnonymous { get; set; }

        [Required]
        public DateTime LastActivityDate { get; set; }
    }
}
