namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums
{
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Helpers;

    //====================================================================
    // API Version Check
    //====================================================================
    // V3.0 = NO CHANGE
    //====================================================================
    // v4.0 = UPDATED
    //  Added :     Arrest
    //  Changed :   BonusShares => BonusSharesIssuance
    //  Added :     CompanyExpansion
    //  Added :     CompanyNameChange
    //  Added :     Conviction
    //  Added :     CreditRating
    //  Added :     EmploymentChange
    //  Added :     EnvironmentalIssue
    //  Added :     Extinction
    //  Added :     FDAPhase
    //  Added :     Indictment
    //  Added :     ManMadeDisaster
    //  Removed :   ManagementChange
    //  Added :     NaturalDisaster
    //  Added :     PatentFiling
    //  Added :     PatentIssuance
    //  Added :     ProductRecall
    //  Added :     ProductRelease
    //  Added :     SecondaryIssuance
    //  Added :     Trial
    //====================================================================
    public enum CalaisSimpleEventType
    {
        [EnumString("M&A")]
        Acquisition,
        [EnumString("Business Partnership")]
        Alliance,
        [EnumString("Earnings Estimate")]
        AnalystEarningsEstimate,
        [EnumString("Analyst Recommendation")]
        AnalystRecommendation,
        [EnumString("Judicial Event")]
        Arrest,
        [EnumString("Bankruptcy")]
        Bankruptcy,
        [EnumString("Bonus Shares")]
        BonusSharesIssuance,
        [EnumString("Business Partnership")]
        BusinessRelation,
        [EnumString("Security Buyback")]
        Buybacks,
        [EnumString("Earnings Announcement")]
        CompanyEarningsAnnouncement,
        [EnumString("Earnings Guidance")]
        CompanyEarningsGuidance,
        [EnumString("Company Expansion")]
        CompanyExpansion,
        [EnumString("Funding")]
        CompanyInvestment,
        [EnumString("Legal Issues")]
        CompanyLegalIssues,
        [EnumString("General or Shareholder Meeting")]
        CompanyMeeting,
        [EnumString("Name Change")]
        CompanyNameChange,
        [EnumString("Reorganization")]
        CompanyReorganization,
        [EnumString("Conference Call")]
        ConferenceCall,
        [EnumString("Judicial Event")]
        Conviction,
        [EnumString("Credit Rating")]
        CreditRating,
        [EnumString("Employment Change")]
        EmploymentChange,
        [EnumString("Environmental Issues")]
        EnvironmentalIssue,
        [EnumString("Extinction")]
        Extinction,
        [EnumString("FDA Phase")]
        FDAPhase,
        [EnumString("Judicial Event")]
        Indictment,
        [EnumString("IPO")]
        IPO,
        [EnumString("Business Partnership")]
        JointVenture,
        [EnumString("Man-Made Disaster")]
        ManMadeDisaster,
        [EnumString("M&A")]
        Merger,
        [EnumString("Movie Release")]
        MovieRelease,
        [EnumString("Music Album Release")]
        MusicAlbumRelease,
        [EnumString("Natural Disaster")]
        NaturalDisaster,
        [EnumString("Patent Filing")]
        PatentFiling,
        [EnumString("Patent Issuance")]
        PatentIssuance,
        [EnumString("Person Travel")]
        PersonTravel,
        [EnumString("Product Recall")]
        ProductRecall,
        [EnumString("Product Release")]
        ProductRelease,
        [EnumString("Second Stock Issuance")]
        SecondaryIssuance,
        [EnumString("Person Communication and Meetings")]
        PersonCommunication,
        [EnumString("Stock Split")]
        StockSplit,
        [EnumString("Judicial Event")]
        Trial
    }
}