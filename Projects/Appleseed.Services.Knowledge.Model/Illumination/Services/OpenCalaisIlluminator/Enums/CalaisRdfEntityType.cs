namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums
{
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Helpers;

    //====================================================================
    // API Version Check
    //====================================================================
    // V3.0 = NO CHANGE 
    //====================================================================
    // v4.0 = UPDATED
    //
    //  Removed :   NaturalDisaster
    //  Added :     Position
    //====================================================================

    /// <summary>
    /// Available types for an entity.
    /// </summary>
    public enum CalaisRdfEntityType
    {
        [EnumString("Anniversary")]
        Anniversary,
        [EnumString("City")]
        City,
        [EnumString("Company")]
        Company,
        [EnumString("Continent")]
        Continent,
        [EnumString("Country")]
        Country,
        [EnumString("Currency")]
        Currency,
        [EnumString("Email Address")]
        EmailAddress,
        [EnumString("Entertainment Award Event")]
        EntertainmentAwardEvent,
        [EnumString("Facility")]
        Facility,
        [EnumString("Fax Number")]
        FaxNumber,
        [EnumString("Holiday")]
        Holiday,
        [EnumString("Industry Term")]
        IndustryTerm,
        [EnumString("Market Index")]
        MarketIndex,
        [EnumString("Medical Condition")]
        MedicalCondition,
        [EnumString("Medical Treatment")]
        MedicalTreatment,
        [EnumString("Movie")]
        Movie,
        [EnumString("Music Album")]
        MusicAlbum,
        [EnumString("Music Group")]
        MusicGroup,
        [EnumString("Natural Feature")]
        NaturalFeature,
        [EnumString("Operating System")]
        OperatingSystem,
        [EnumString("Organization")]
        Organization,
        [EnumString("Person")]
        Person,
        [EnumString("Phone Number")]
        PhoneNumber,
        [EnumString("Position")]
        Position,
        [EnumString("Product")]
        Product,
        [EnumString("Programming Language")]
        ProgrammingLanguage,
        [EnumString("Province Or State")]
        ProvinceOrState,
        [EnumString("Published Medium")]
        PublishedMedium,
        [EnumString("Radio Program")]
        RadioProgram,
        [EnumString("Radio Station")]
        RadioStation,
        [EnumString("Region")]
        Region,
        [EnumString("Sports Event")]
        SportsEvent,
        [EnumString("Sports League")]
        SportsLeague,
        [EnumString("Sports Game")]
        SportsGame,
        [EnumString("Technology")]
        Technology,
        [EnumString("TV Show")]
        TVShow,
        [EnumString("TV Station")]
        TVStation,
        [EnumString("URL")]
        URL
    }
}
