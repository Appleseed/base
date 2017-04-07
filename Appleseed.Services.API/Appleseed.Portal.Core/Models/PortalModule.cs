namespace Appleseed.Portal.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// rb_ModuleDefinitions
    /// </summary>
    public class PortalModule
    {
        public int ModuleDefID { get; set; }

        [Required]
        public int PortalID { get; set; }

        [Required]
        public Guid GeneralModDefID { get; set; }
    }
}
