namespace Appleseed.Portal.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class PortalSetting
    {
        public int PortalId { get; set; }

        [Required, StringLength(50)]
        public string SettingName { get; set; }

        [Required, StringLength(1500)]
        public string SettingValue { get; set; }
    }
}
