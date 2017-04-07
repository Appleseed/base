namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums
{
    //====================================================================
    // API Version Check
    //====================================================================
    // V3.0 = NO CHANGE 
    //====================================================================
    // v4.0 = UPDATED
    //
    //  Added :     Arrest
    //  Update:     BonusShares => BonusSharesIssuance
    //  Added :     CompanyEmployeesNumber
    //  Added :     CompanyExpansion
    //  Added :     CompanyFounded
    //  Added :     CompanyNameChange
    //  Added :     CompanyProduct
    //  Added :     Conviction
    //  Added :     EmploymentChange
    //  Added :     EmploymentRelation
    //  Added :     EnvironmentalIssue
    //  Added :     Extinction
    //  Added :     FDAPhase
    //  Added :     Indictment
    //  Added :     ManMadeDisaster
    //  Remove:     ManagementChange
    //  Added :     NaturalDisaster
    //  Added :     PatentFiling
    //  Added :     PatentIssuance 
    //  Added :     PersonCareer
    //  Added :     PersonEmailAddress
    //  Added :     PersonRelation
    //  Remove:     PersonPolitical 
    //  Remove:     PersonPoliticalPast 
    //  Remove:     PersonProfessional 
    //  Remove:     PersonProfessionalPast 
    //  Added :     ProductRecall
    //  Added :     ProductRelease
    //  Added :     SecondaryIssuance
    //  Added :     Trial
    //  
    //====================================================================

    /// <summary>
    /// Available types for a relationship (Event / Fact).
    /// </summary>
    public enum CalaisRdfRelationshipType
    {
        Acquisition, 
        Alliance, 
        AnalystEarningsEstimate, 
        AnalystRecommendation,
        Arrest,
        Bankruptcy, 
        BonusSharesIssuance, 
        BusinessRelation, 
        Buybacks, 
        CompanyAffiliates, 
        CompanyCustomer, 
        CompanyEarningsAnnouncement, 
        CompanyEarningsGuidance,
        CompanyEmployeesNumber,
        CompanyExpansion, 
        CompanyFounded,
        CompanyInvestment, 
        CompanyLegalIssues, 
        CompanyLocation, 
        CompanyMeeting,
        CompanyNameChange,
        CompanyProduct,
        CompanyReorganization, 
        CompanyTechnology, 
        CompanyTicker, 
        ConferenceCall,
        Conviction,
        CreditRating,
        EmploymentChange, 
        EmploymentRelation, 
        EnvironmentalIssue, 
        Extinction,
        FamilyRelation,
        FDAPhase, 
        Indictment, 
        IPO, 
        JointVenture,
        ManMadeDisaster,  
        Merger, 
        MovieRelease, 
        MusicAlbumRelease,
        NaturalDisaster, 
        PatentFiling,
        PatentIssuance, 
        PersonAttributes,
        PersonCareer,
        PersonCommunication, 
        PersonEducation,
        PersonEmailAddress, 
        PersonRelation,  
        PersonTravel,
        ProductRecall,
        ProductRelease,
        Quotation,
        SecondaryIssuance, 
        StockSplit,
        Trial 
    }
}
