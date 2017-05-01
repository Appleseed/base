namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums
{
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Helpers;

    /// <summary>
    /// Available formats of input content
    /// </summary>
    public enum CalaisInputFormat
    {
        [EnumString("TEXT/XML")]
        Xml,

        [EnumString("TEXT/TXT")]
        Text,

        [EnumString("TEXT/HTML")]
        Html
    }
}