namespace Appleseed.Portal.Database.Context
{
    using System.Collections.Generic;

    using Appleseed.Portal.Core.Models;

    public interface IModuleDatabase
    {
        int CreatePageModule(PageModule pageModule);

        PageModule RetrievePageModule(int moduleId);

        List<PageModule> RetrieveAllPageModules(int pageId);

        int UpdatePageModule(PageModule pageModule);

        int DeletePageModule(int moduleId);

        int CreateHtmlText(HtmlText item);

        HtmlText RetrieveHtmlText(int id);

        int UpdateHtmlText(HtmlText item);

        int DeleteHtmlText(int id, int versionNo);

        void CreateModuleSetting(ModuleSetting item);

        ModuleSetting RetrieveModuleSetting(int id);

        List<ModuleSetting> RetrieveAllModuleSettings(int moduleId);

        void UpdateModuleSetting(ModuleSetting item);

        void DeleteModuleSetting(int id);

        void CreatePage(Page item);

        Page RetrievePage(int pageId);

        List<Page> RetrieveAllPages(int portalId);

        int UpdatePage(Page item);

        int DeletePage(int pageId);

        int UpdateHtmlVersion(HtmlText item);

        int CreateNewHtmlVersion(HtmlText item);

        List<HtmlText> GetHtmlVersionList(int moduleid);

        HtmlText GetHtmlByVersion(int moduleid, int versionno);

        List<PageModule> GetModulesByPortalID(int portalId);

    }
}