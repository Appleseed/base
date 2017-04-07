namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums
{
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Helpers;

    /// <summary>
    /// Available sub-types for an entity.
    /// </summary>
    public enum CalaisRdfEntitySubType
    {
        [EnumString("None")]
        None,
        [EnumString("Nationality")]
        Nationality,
        [EnumString("Organization Type")]
        OrganizationType,
        [EnumString("Person Type")]
        PersonType,
        [EnumString("Product Type")]
        ProductType
    }
}
