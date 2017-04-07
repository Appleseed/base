namespace Appleseed.Portal.Database.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Appleseed.Portal.Core.Exceptions;
    using Appleseed.Portal.Core.Models;
    using Appleseed.Services.Core.Helpers;

    using Common.Logging;

    public class PortalDatabase : IPortalDatabase
    {
        private readonly ILog logger;

        private readonly string connectionString;

        public PortalDatabase(ILog logger, string connectionString)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            this.logger = logger;
            this.connectionString = connectionString;
        }

        /// <summary>
        /// The CreatePortal method add a new portal.<br/>
        /// CreatePortal Stored Procedure
        /// </summary>
        /// <param name="portal">
        /// The portal.
        /// </param>
        public void CreatePortal(Portal portal)
        {
            const string Sproc = "rb_AddPortal";

            var sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter() { ParameterName = "@PortalAlias", Value = portal.PortalAlias };
            sqlParams[1] = new SqlParameter() { ParameterName = "@PortalName", Value = portal.PortalName };
            sqlParams[2] = new SqlParameter() { ParameterName = "@PortalPath", Value = portal.PortalPath };
            sqlParams[3] = new SqlParameter() { ParameterName = "@AlwaysShowEditButton", Value = portal.AlwaysShowEditButton };
            sqlParams[4] = new SqlParameter() { ParameterName = "@PortalID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            SqlClientHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, Sproc, sqlParams);
            portal.PortalId = (int)sqlParams[4].Value;
        }

        /// <summary>
        /// The GetPortals method returns an ArrayList containing all of the
        ///   Portals registered in this database.<br/>
        ///   GetPortals Stored Procedure
        /// </summary>
        /// <returns>
        /// a list of portals
        /// </returns>
        public List<Portal> RetrievePortals()
        {
            const string Sproc = "rb_GetPortals";
            var portals = new List<Portal>();

            using (var reader = SqlClientHelper.GetReader(this.connectionString, CommandType.StoredProcedure, Sproc))
            {
                if (reader.HasRows)
                {
                    var portalIdIndex = reader.GetOrdinal("PortalID");
                    var portalNameIndex = reader.GetOrdinal("PortalName");
                    var portalPathIndex = reader.GetOrdinal("PortalPath");
                    var portalAliasIndex = reader.GetOrdinal("PortalAlias");
                    while (reader.Read())
                    {
                        var p = new Portal()
                        {
                            PortalName = reader.GetString(portalNameIndex),
                            PortalPath = SqlClientHelper.GetNullableString(reader, portalPathIndex),
                            PortalId = reader.GetInt32(portalIdIndex),
                            PortalAlias = reader.GetString(portalAliasIndex)
                        };
                        portals.Add(p);
                    }
                }
            }

            return portals;
        }

        public Portal RetrievePortal(int id)
        {
            const string Sql = "SELECT  rb_Portals.PortalID, rb_Portals.PortalAlias, rb_Portals.PortalName, rb_Portals.PortalPath, rb_Portals.AlwaysShowEditButton FROM rb_Portals";
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@PortalID", Value = id };
            
            using (var reader = SqlClientHelper.GetReader(this.connectionString, CommandType.Text, Sql))
            {
                if (reader.HasRows)
                {
                    var portalIdIndex = reader.GetOrdinal("PortalID");
                    var portalNameIndex = reader.GetOrdinal("PortalName");
                    var portalPathIndex = reader.GetOrdinal("PortalPath");
                    var portalaliasIndex = reader.GetOrdinal("PortalAlias");
                    reader.Read();
                    var p = new Portal()
                    {
                        PortalName = reader.GetString(portalNameIndex),
                        PortalPath = SqlClientHelper.GetNullableString(reader, portalPathIndex),
                        PortalId = reader.GetInt32(portalIdIndex),
                        PortalAlias = reader.GetString(portalaliasIndex)
                    };
                    return p;
                }
            }

            return null;
        }

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
        /// <param name="alwaysShow">
        /// if set to <c>true</c> [always show].
        /// </param>
        public int UpdatePortal(Portal portal)
        {
            const string Sproc = "rb_UpdatePortalInfo";

            var sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter() { ParameterName = "@PortalID", Value = portal.PortalId };
            sqlParams[1] = new SqlParameter() { ParameterName = "@PortalName", Value = portal.PortalName };
            sqlParams[2] = new SqlParameter() { ParameterName = "@PortalPath", Value = portal.PortalPath };
            sqlParams[3] = new SqlParameter() { ParameterName = "@AlwaysShowEditButton", Value = portal.AlwaysShowEditButton };

            return SqlClientHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, Sproc, sqlParams);
        }

        /// <summary>
        /// Removes portal from database. All tabs, modules and data wil be removed.
        /// </summary>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        public int DeletePortal(int portalId)
        {
            const string Sproc = "rb_DeletePortal";

            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@PortalID", Value = portalId };

            return SqlClientHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, Sproc, sqlParams);
        }

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
        public void UpdatePortalSetting(PortalSetting setting)
        {
            const string Sproc = "rp_UpdatePortalSetting";

            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter() { ParameterName = "@PortalID", Value = setting.PortalId };
            sqlParams[1] = new SqlParameter() { ParameterName = "@SettingName", Value = setting.SettingName };
            sqlParams[2] = new SqlParameter() { ParameterName = "@SettingValue", Value = setting.SettingValue };

            try
            {
                SqlClientHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, Sproc, sqlParams);
            }
            catch (Exception ex)
            {
                var exception = new AppleseedDatabaseException("A database error has occurred while updating a portal setting.  Error message has been logged.", ex);
                this.logger.Error(exception);
                throw exception;
            }
        }
    }
}
