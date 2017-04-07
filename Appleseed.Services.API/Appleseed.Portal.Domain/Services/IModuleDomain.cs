namespace Appleseed.Portal.Domain.Services
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Appleseed.Portal.Core.Models;

    public interface IModuleDomain
    {
        List<ValidationResult> CreatePage(Page item);

        Page RetrievePage(int pageId);

        List<Page> RetrieveAllPages(int portalId);

        List<ValidationResult> UpdatePage(Page item);

        List<ValidationResult> DeletePage(int pageId);

        List<ValidationResult> CreatePageModule(PageModule item);

        PageModule RetrievePageModule(int moduleId);

        List<PageModule> RetrieveAllPageModules(int pageId);

        List<ValidationResult> UpdatePageModule(PageModule item);

        List<ValidationResult> DeletePageModule(int moduleId);
    }
}