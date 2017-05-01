namespace Appleseed.Portal.Database.Context
{
    using System.Collections.Generic;

    using Appleseed.Portal.Core.Models;

    public interface IPortalDatabase
    {
        /// <summary>
        /// The CreatePortal method add a new portal.<br/>
        /// CreatePortal Stored Procedure
        /// </summary>
        /// <param name="portal">
        /// The portal.
        /// </param>
        void CreatePortal(Portal portal);

        /// <summary>
        /// The GetPortals method returns an ArrayList containing all of the
        ///   Portals registered in this database.<br/>
        ///   GetPortals Stored Procedure
        /// </summary>
        /// <returns>
        /// a list of portals
        /// </returns>
        List<Portal> RetrievePortals();

        Portal RetrievePortal(int id);

        /// <summary>
        /// The UpdatePortalInfo method updates the name and access settings for the portal.<br/>
        ///   Uses UpdatePortalInfo Stored Procedure.
        /// </summary>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        /// <param name="portalName">
        /// Name of the portal.
        /// </param>
        /// <param name="portalPath">
        /// The portal path.
        /// </param>
        /// <param name="alwaysShow">
        /// if set to <c>true</c> [always show].
        /// </param>
        int UpdatePortal(Portal portal);

        /// <summary>
        /// Removes portal from database. All tabs, modules and data wil be removed.
        /// </summary>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        int DeletePortal(int portalId);

        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        ///   in the PortalSettings database table.
        /// </summary>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        /// <param name="key">
        /// The setting key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <remarks>
        /// </remarks>
        void UpdatePortalSetting(PortalSetting setting);
    }
}