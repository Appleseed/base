namespace Appleseed.Portal.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// rb_Modules
    /// </summary>
    public class PageModule
    {
        [Required]
        public int ModuleID { get; set; }
	    
        [Required]
        public int TabID { get; set; }
	    
        [Required]
        public int ModuleDefID { get; set; }
	    
        [Required]
        public int ModuleOrder { get; set; }
	    
        [Required, StringLength(50)]
        public string PaneName { get; set; }
        
        [StringLength(256)]
	    public string ModuleTitle { get; set; }
	    
        [StringLength(256)]
	    public string AuthorizedEditRoles { get; set; }
	    
        [StringLength(256)]
	    public string AuthorizedViewRoles { get; set; }
	    
        [StringLength(256)]
	    public string AuthorizedAddRoles { get; set; }
	    
        [StringLength(256)]
	    public string AuthorizedDeleteRoles { get; set; }
	    
        [StringLength(256)]
	    public string AuthorizedPropertiesRoles { get; set; }
	    
        [Required]
        public int CacheTime { get; set; }
	    
        public bool ShowMobile { get; set; }
	    
        [StringLength(256)]
	    public string AuthorizedPublishingRoles { get; set; }
	    
        public bool NewVersion { get; set; }
	    
        public bool SupportWorkflow { get; set; }
	    
        [StringLength(256)]
	    public string AuthorizedApproveRoles { get; set; }

        public byte? WorkflowState { get; set; }
	    
        public DateTime LastModified { get; set; }
	    
        [StringLength(256)]
	    public string LastEditor { get; set; }
	    
        public DateTime StagingLastModified { get; set; }
	    
        [StringLength(256)]
	    public string StagingLastEditor { get; set; }
	    
        public bool SupportCollapsable { get; set; }
	    
        public bool ShowEveryWhere { get; set; }
	    
        [StringLength(256)]
	    public string AuthorizedMoveModuleRoles { get; set; }
	    
        [StringLength(256)]
	    public string AuthorizedDeleteModuleRoles { get; set; }

        //public List<ModuleSetting> ModuleSettings { get; set; } 
    }
}
