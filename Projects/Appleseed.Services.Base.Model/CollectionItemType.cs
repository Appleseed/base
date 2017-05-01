namespace Appleseed.Services.Base
{
    public static class CollectionItemType
    {
        //Web/File
        public const string PAGE            = "Page";
        public const string FILE            = "File";
        
        //Appleseed
        public const string MODULE          = "Module";
        
        //Nutshell
        public const string NS_ACC          = "Account";
        public const string NS_CON          = "Contact";
        
        //CrunchBase
        public const string CB_ORG          = "Organization";
        public const string CB_PER          = "Person";
        public const string CB_PRO          = "Product";

    }

    // TODO: temporarily only one value - always set the most secure for now
    public static class CollectionItemSecurityType
    {
        public const string PUBLIC      = "Public";
        public const string AUTHORIZED  = "Private";
        public const string ADMIN       = "Admin";
    }
}
