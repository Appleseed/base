namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Entities
{
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums;

    public class CalaisSimpleEntity
    {
        public string Value { get; set; }
        public int Frequency { get; set; }
        public string Relevance { get; set; }
        public CalaisSimpleEntityType Type { get; set; }
    }
}