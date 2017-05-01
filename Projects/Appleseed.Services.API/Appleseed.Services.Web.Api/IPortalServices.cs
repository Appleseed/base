namespace Appleseed.Services.Web.Api
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    //using Appleseed.Services.Web.Api.Entity;
    using Appleseed.Portal.Core.Models;

    using WCFExtras.Soap;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [SoapHeaders]
    [ServiceContract]
    public interface IPortalServices
    {
        #region Users

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        bool ValidateUser(int portalId, string username, string password);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<AsUser> GetUsers(int portalId);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<AsRole> GetRoles(int portalId);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> CreateUser(int portalId, AsUser item);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> CreateRole(int portalId, AsRole item);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> AddUserToRole(int portalId, AsUser user, AsRole role);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> RemoveUserFromRole(int portalId, AsUser user, AsRole role);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> RemoveUser(int portalId, string username);

        #endregion

        #region Pages

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<Page> GetPages(int portalId);

        #endregion

        #region Modules

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<PageModule> GetModulesOnPage(int portalId, int pageId);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        void GetAllInstalledModules();

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        void UnInstallModule();

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> AddModuleToPage(int portalId, PageModule pageModule);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> ModifyPageModule(int portalId, PageModule pageModule);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<PageModule> GetModuleByPortalID(int portalId);

        #endregion

        #region Portal

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<Portal> GetPortals();

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> CreatePortal(Portal portal);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> UpdatePortal(Portal portal);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> DeletePortal(int portalId);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        void ChangePortalSettings();

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        void ChangeUserEmail(int portalId, Guid userId, string newEmailAddress);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        bool ValidAPIKey();

        #endregion

        #region HtmlText

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> CreateHtmlText(int portalId, HtmlText item);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        HtmlText RetrieveHtmlText(int portalId, int moduleId);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<string> UpdateHtmlText(int portalId, HtmlText item);

        List<string> DeleteHtmlText(int portalId, int textId, int versionNo);

        #endregion

        #region HtmlVersion

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        int CreateNewHtmlVersion(int portalId, HtmlText item);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        int UpdateHtmlByVersion(int portalId, HtmlText item);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        int PublishHtmlVersion(int portalId, HtmlText item);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        HtmlText GetHtmlByVersion(int portalId, int versionNo, int moduleId);

        [SoapHeader("AuthHeader", typeof(AuthHeader), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        List<HtmlText> GetHtmlVersionHistory(int portalId, int ModuleId);

        #endregion
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return this.boolValue; }
            set { this.boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return this.stringValue; }
            set { this.stringValue = value; }
        }
    }
}
