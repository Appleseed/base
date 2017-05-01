namespace Appleseed.Portal.Domain.Services
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Appleseed.Portal.Core.Models;

    public interface IPortalDomain
    {
        List<ValidationResult> CreatePortal(Portal item);

        Portal RetrievePortalById(int id);

        List<Portal> RetrievePortals();

        List<ValidationResult> UpdatePortal(Portal portal);

        List<ValidationResult> DeletePortal(int portalId);
    }
}