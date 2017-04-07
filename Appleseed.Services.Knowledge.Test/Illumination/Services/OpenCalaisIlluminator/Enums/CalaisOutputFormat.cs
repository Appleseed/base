namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums
{
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Helpers;

    /// <summary>
    /// Available formats of input content.
    /// </summary>
    public enum CalaisOutputFormat
    {
        [EnumString("XML/RDF")]
        Rdf,
        
        [EnumString("Text/Simple")]
        Simple,
        
        [EnumString("Text/Microformats")]
        MicroFormats
    }
}
