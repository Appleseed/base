namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Documents
{
    public class CalaisSimpleDocumentDescription
    {
        public bool AllowDistribution { get; set; }

        public bool AllowSearch { get; set; }
        
        public string ExternalId { get; set; }
        
        public string Id { get; set; }
        
        public string About { get; set; }
        
        public string IlluminationRequestId { get; set; }
    }
}