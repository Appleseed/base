namespace Appleseed.Services.Base.Model
{
    public class AppleseedModuleItemIndex : AppleseedModuleItem
    {
        // Item Content Fields
        public string Content { get; set; }
        
        public string Summary { get; set; }

        // Item Smart Content Fields
        public string SmartKeywords { get; set; }

        // Item Smart Content Fields - Not yet implemented
        public string SmartConcepts { get; set; }
        
        public string SmartEntities { get; set; }
        
        public string SmartCategories { get; set; }
        
        public string SmartAuthors { get; set; }

        // Item Smart Summarized Data - Not yet implemented
        public string SmartSummary { get; set; }

        public string CreatedDate { get; set; }

        public string ToDebugString()
        {
            return "Content:" + string.IsNullOrEmpty(this.Content) + "; " +
                "Summary:" + string.IsNullOrEmpty(this.Summary) + "; " +
                "SmartKeywords:" + string.IsNullOrEmpty(this.SmartKeywords) + "; " +
                "SmartConcepts:" + string.IsNullOrEmpty(this.SmartConcepts) + "; " +
                "SmartEntities:" + string.IsNullOrEmpty(this.SmartEntities) + "; " +
                "SmartCategories:" + string.IsNullOrEmpty(this.SmartCategories) + "; " +
                "SmartAuthors:" + string.IsNullOrEmpty(this.SmartAuthors) + "; " +
                "SmartSummary:" + string.IsNullOrEmpty(this.SmartSummary) + "; " +
                "CreatedDate:" + string.IsNullOrEmpty(this.CreatedDate) + "; ";
        }
    }
}
