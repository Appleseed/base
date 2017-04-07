namespace Appleseed.Portal.Database.Context
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Appleseed.Portal.Core.Models;
    using Appleseed.Services.Core.Helpers;

    using Common.Logging;
    using System;

    public class ModuleDatabase : BaseDatabase, IModuleDatabase
    {
        public ModuleDatabase(ILog logger, string connectionString, string applicationName, int portalId)
            : base(logger, connectionString, applicationName, portalId)
        {
        }

        #region PageModule
        public int CreatePageModule(PageModule pageModule)
        {
            const string Sproc = "rb_AddModule";

            var sqlParams = new SqlParameter[19];
            sqlParams[0] = new SqlParameter() { ParameterName = "@TabID", Value = pageModule.TabID };
            sqlParams[1] = new SqlParameter() { ParameterName = "@ModuleOrder", Value = pageModule.ModuleOrder };
            sqlParams[2] = new SqlParameter() { ParameterName = "@ModuleTitle", Value = pageModule.ModuleTitle };
            sqlParams[3] = new SqlParameter() { ParameterName = "@PaneName", Value = pageModule.PaneName };
            sqlParams[4] = new SqlParameter() { ParameterName = "@ModuleDefID", Value = pageModule.ModuleDefID };
            sqlParams[5] = new SqlParameter() { ParameterName = "@CacheTime", Value = pageModule.CacheTime };
            sqlParams[6] = new SqlParameter() { ParameterName = "@EditRoles", Value = pageModule.AuthorizedEditRoles };
            sqlParams[7] = new SqlParameter() { ParameterName = "@AddRoles", Value = pageModule.AuthorizedAddRoles };
            sqlParams[8] = new SqlParameter() { ParameterName = "@ViewRoles", Value = pageModule.AuthorizedViewRoles };
            sqlParams[9] = new SqlParameter() { ParameterName = "@DeleteRoles", Value = pageModule.AuthorizedDeleteRoles };
            sqlParams[10] = new SqlParameter() { ParameterName = "@PropertiesRoles", Value = pageModule.AuthorizedPropertiesRoles };
            sqlParams[11] = new SqlParameter() { ParameterName = "@MoveModuleRoles", Value = pageModule.AuthorizedMoveModuleRoles };
            sqlParams[12] = new SqlParameter() { ParameterName = "@DeleteModuleRoles", Value = pageModule.AuthorizedDeleteModuleRoles };
            sqlParams[13] = new SqlParameter() { ParameterName = "@ShowMobile", Value = pageModule.ShowMobile };
            sqlParams[14] = new SqlParameter() { ParameterName = "@PublishingRoles", Value = pageModule.AuthorizedPublishingRoles };
            sqlParams[15] = new SqlParameter() { ParameterName = "@SupportWorkflow", Value = pageModule.SupportWorkflow };
            sqlParams[16] = new SqlParameter() { ParameterName = "@ShowEveryWhere", Value = pageModule.ShowEveryWhere };
            sqlParams[17] = new SqlParameter() { ParameterName = "@SupportCollapsable", Value = pageModule.SupportCollapsable };
            sqlParams[18] = new SqlParameter() { ParameterName = "@ModuleID ", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            var result = SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
            pageModule.ModuleID = (int)sqlParams[18].Value;
            return result;
        }

        public PageModule RetrievePageModule(int moduleId)
        {
            const string Sql = "SELECT * FROM rb_Modules WHERE (ModuleID = @ModuleID)";
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID", Value = moduleId };
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.Text, Sql, sqlParams))
            {
                if (reader.HasRows)
                {
                    var moduleIdIndex = reader.GetOrdinal("ModuleID");
                    var tabIdIndex = reader.GetOrdinal("TabID");
                    var moduleDefIdIndex = reader.GetOrdinal("ModuleDefID");
                    var moduleOrderIndex = reader.GetOrdinal("ModuleOrder");
                    var paneNameIndex = reader.GetOrdinal("PaneName");
                    var moduleTitleIndex = reader.GetOrdinal("ModuleTitle");
                    var authorizedEditRolesIndex = reader.GetOrdinal("AuthorizedEditRoles");
                    var authorizedViewRolesIndex = reader.GetOrdinal("AuthorizedViewRoles");
                    var authorizedAddRolesIndex = reader.GetOrdinal("AuthorizedAddRoles");
                    var authorizedDeleteRolesIndex = reader.GetOrdinal("AuthorizedDeleteRoles");
                    var authorizedPropertiesRolesIndex = reader.GetOrdinal("AuthorizedPropertiesRoles");
                    var cacheTimeIndex = reader.GetOrdinal("CacheTime");
                    var showMobileIndex = reader.GetOrdinal("ShowMobile");
                    var authorizedPublishingRolesIndex = reader.GetOrdinal("AuthorizedPublishingRoles");
                    var newVersionIndex = reader.GetOrdinal("NewVersion");
                    var supportWorkflowIndex = reader.GetOrdinal("SupportWorkflow");
                    var authorizedApproveRolesIndex = reader.GetOrdinal("AuthorizedApproveRoles");
                    var workflowStateIndex = reader.GetOrdinal("WorkflowState");
                    var lastModifiedIndex = reader.GetOrdinal("LastModified");
                    var lastEditorIndex = reader.GetOrdinal("LastEditor");
                    var stagingLastModifiedIndex = reader.GetOrdinal("StagingLastModified");
                    var stagingLastEditorIndex = reader.GetOrdinal("StagingLastEditor");
                    var supportCollapsableIndex = reader.GetOrdinal("SupportCollapsable");
                    var showEveryWhereIndex = reader.GetOrdinal("ShowEveryWhere");
                    var authorizedMoveModuleRolesIndex = reader.GetOrdinal("AuthorizedMoveModuleRoles");
                    var authorizedDeleteModuleRolesIndex = reader.GetOrdinal("AuthorizedDeleteModuleRoles");

                    reader.Read();
                    var item = new PageModule()
                    {
                        ModuleID = reader.GetInt32(moduleIdIndex),
                        TabID = reader.GetInt32(tabIdIndex),
                        ModuleDefID = reader.GetInt32(moduleDefIdIndex),
                        ModuleOrder = reader.GetInt32(moduleOrderIndex),
                        PaneName = reader.GetString(paneNameIndex),
                        ModuleTitle = SqlClientHelper.GetNullableString(reader, moduleTitleIndex),
                        AuthorizedEditRoles = SqlClientHelper.GetNullableString(reader, authorizedEditRolesIndex),
                        AuthorizedViewRoles = SqlClientHelper.GetNullableString(reader, authorizedViewRolesIndex),
                        AuthorizedAddRoles = SqlClientHelper.GetNullableString(reader, authorizedAddRolesIndex),
                        AuthorizedDeleteRoles = SqlClientHelper.GetNullableString(reader, authorizedDeleteRolesIndex),
                        AuthorizedPropertiesRoles = SqlClientHelper.GetNullableString(reader, authorizedPropertiesRolesIndex),
                        CacheTime = reader.GetInt32(cacheTimeIndex),
                        ShowMobile = reader.GetBoolean(showMobileIndex),
                        AuthorizedPublishingRoles = SqlClientHelper.GetNullableString(reader, authorizedPublishingRolesIndex),
                        NewVersion = reader.GetBoolean(newVersionIndex),
                        SupportWorkflow = reader.GetBoolean(supportWorkflowIndex),
                        AuthorizedApproveRoles = SqlClientHelper.GetNullableString(reader, authorizedApproveRolesIndex),
                        WorkflowState = reader.GetByte(workflowStateIndex),
                        LastModified = reader.GetDateTime(lastModifiedIndex),
                        LastEditor = SqlClientHelper.GetNullableString(reader, lastEditorIndex),
                        StagingLastModified = reader.GetDateTime(stagingLastModifiedIndex),
                        StagingLastEditor = SqlClientHelper.GetNullableString(reader, stagingLastEditorIndex),
                        SupportCollapsable = reader.GetBoolean(supportCollapsableIndex),
                        ShowEveryWhere = reader.GetBoolean(showEveryWhereIndex),
                        AuthorizedMoveModuleRoles = SqlClientHelper.GetNullableString(reader, authorizedMoveModuleRolesIndex),
                        AuthorizedDeleteModuleRoles = SqlClientHelper.GetNullableString(reader, authorizedDeleteModuleRolesIndex)
                    };
                    return item;
                }
            }

            return null;
        }

        public List<PageModule> RetrieveAllPageModules(int pageId)
        {
            const string Sql = "SELECT m.* FROM rb_Modules m INNER JOIN rb_ModuleDefinitions md ON m.ModuleDefID = md.ModuleDefID INNER JOIN rb_GeneralModuleDefinitions gmd ON md.GeneralModDefID = gmd.GeneralModDefID WHERE (m.TabID = @TabID) AND (md.PortalID = @PortalID)";
            var results = new List<PageModule>();
            var sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter() { ParameterName = "@PortalID", Value = this.PortalId };
            sqlParams[1] = new SqlParameter() { ParameterName = "@TabID", Value = pageId };
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.Text, Sql, sqlParams))
            {
                if (reader.HasRows)
                {
                    var moduleIdIndex = reader.GetOrdinal("ModuleID");
                    var tabIdIndex = reader.GetOrdinal("TabID");
                    var moduleDefIdIndex = reader.GetOrdinal("ModuleDefID");
                    var moduleOrderIndex = reader.GetOrdinal("ModuleOrder");
                    var paneNameIndex = reader.GetOrdinal("PaneName");
                    var moduleTitleIndex = reader.GetOrdinal("ModuleTitle");
                    var authorizedEditRolesIndex = reader.GetOrdinal("AuthorizedEditRoles");
                    var authorizedViewRolesIndex = reader.GetOrdinal("AuthorizedViewRoles");
                    var authorizedAddRolesIndex = reader.GetOrdinal("AuthorizedAddRoles");
                    var authorizedDeleteRolesIndex = reader.GetOrdinal("AuthorizedDeleteRoles");
                    var authorizedPropertiesRolesIndex = reader.GetOrdinal("AuthorizedPropertiesRoles");
                    var cacheTimeIndex = reader.GetOrdinal("CacheTime");
                    var showMobileIndex = reader.GetOrdinal("ShowMobile");
                    var authorizedPublishingRolesIndex = reader.GetOrdinal("AuthorizedPublishingRoles");
                    var newVersionIndex = reader.GetOrdinal("NewVersion");
                    var supportWorkflowIndex = reader.GetOrdinal("SupportWorkflow");
                    var authorizedApproveRolesIndex = reader.GetOrdinal("AuthorizedApproveRoles");
                    var workflowStateIndex = reader.GetOrdinal("WorkflowState");
                    var lastModifiedIndex = reader.GetOrdinal("LastModified");
                    var lastEditorIndex = reader.GetOrdinal("LastEditor");
                    var stagingLastModifiedIndex = reader.GetOrdinal("StagingLastModified");
                    var stagingLastEditorIndex = reader.GetOrdinal("StagingLastEditor");
                    var supportCollapsableIndex = reader.GetOrdinal("SupportCollapsable");
                    var showEveryWhereIndex = reader.GetOrdinal("ShowEveryWhere");
                    var authorizedMoveModuleRolesIndex = reader.GetOrdinal("AuthorizedMoveModuleRoles");
                    var authorizedDeleteModuleRolesIndex = reader.GetOrdinal("AuthorizedDeleteModuleRoles");

                    while (reader.Read())
                    {
                        var role = new PageModule();
                        role.ModuleID = reader.GetInt32(moduleIdIndex);
                        role.TabID = reader.GetInt32(tabIdIndex);
                        role.ModuleDefID = reader.GetInt32(moduleDefIdIndex);
                        role.ModuleOrder = reader.GetInt32(moduleOrderIndex);
                        role.PaneName = reader.GetString(paneNameIndex);
                        role.ModuleTitle = SqlClientHelper.GetNullableString(reader, moduleTitleIndex);
                        role.AuthorizedEditRoles = SqlClientHelper.GetNullableString(reader, authorizedEditRolesIndex);
                        role.AuthorizedViewRoles = SqlClientHelper.GetNullableString(reader, authorizedViewRolesIndex);
                        role.AuthorizedAddRoles = SqlClientHelper.GetNullableString(reader, authorizedAddRolesIndex);
                        role.AuthorizedDeleteRoles = SqlClientHelper.GetNullableString(reader, authorizedDeleteRolesIndex);
                        role.AuthorizedPropertiesRoles = SqlClientHelper.GetNullableString(reader, authorizedPropertiesRolesIndex);
                        role.CacheTime = reader.GetInt32(cacheTimeIndex);
                        role.ShowMobile = reader.GetBoolean(showMobileIndex);
                        role.AuthorizedPublishingRoles = SqlClientHelper.GetNullableString(reader, authorizedPublishingRolesIndex);
                        role.NewVersion = reader.GetBoolean(newVersionIndex);
                        role.SupportWorkflow = reader.GetBoolean(supportWorkflowIndex);
                        role.AuthorizedApproveRoles = SqlClientHelper.GetNullableString(reader, authorizedApproveRolesIndex);

                        if (reader.IsDBNull(workflowStateIndex).Equals(false))
                        {
                            role.WorkflowState = reader.GetByte(workflowStateIndex);
                        }

                        role.LastModified = reader.GetDateTime(lastModifiedIndex);
                        role.LastEditor = SqlClientHelper.GetNullableString(reader, lastEditorIndex);
                        role.StagingLastModified = reader.GetDateTime(stagingLastModifiedIndex);
                        role.StagingLastEditor = SqlClientHelper.GetNullableString(reader, stagingLastEditorIndex);
                        role.SupportCollapsable = reader.GetBoolean(supportCollapsableIndex);
                        role.ShowEveryWhere = reader.GetBoolean(showEveryWhereIndex);
                        role.AuthorizedMoveModuleRoles = SqlClientHelper.GetNullableString(reader, authorizedMoveModuleRolesIndex);
                        role.AuthorizedDeleteModuleRoles = SqlClientHelper.GetNullableString(reader, authorizedDeleteModuleRolesIndex);

                        results.Add(role);
                    }
                }
            }

            return results;
        }

        public int UpdatePageModule(PageModule pageModule)
        {
            const string Sproc = "rb_UpdateModule";

            var sqlParams = new SqlParameter[19];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID ", Value = pageModule.ModuleID };
            sqlParams[1] = new SqlParameter() { ParameterName = "@TabID", Value = pageModule.TabID };
            sqlParams[2] = new SqlParameter() { ParameterName = "@ModuleOrder", Value = pageModule.ModuleOrder };
            sqlParams[3] = new SqlParameter() { ParameterName = "@ModuleTitle", Value = pageModule.ModuleTitle };
            sqlParams[4] = new SqlParameter() { ParameterName = "@PaneName", Value = pageModule.PaneName };
            sqlParams[5] = new SqlParameter() { ParameterName = "@CacheTime", Value = pageModule.CacheTime };
            sqlParams[6] = new SqlParameter() { ParameterName = "@EditRoles", Value = pageModule.AuthorizedEditRoles };
            sqlParams[7] = new SqlParameter() { ParameterName = "@AddRoles", Value = pageModule.AuthorizedAddRoles };
            sqlParams[8] = new SqlParameter() { ParameterName = "@ViewRoles", Value = pageModule.AuthorizedViewRoles };
            sqlParams[9] = new SqlParameter() { ParameterName = "@DeleteRoles", Value = pageModule.AuthorizedDeleteRoles };
            sqlParams[10] = new SqlParameter() { ParameterName = "@PropertiesRoles", Value = pageModule.AuthorizedPropertiesRoles };
            sqlParams[11] = new SqlParameter() { ParameterName = "@ShowMobile", Value = pageModule.ShowMobile };
            sqlParams[12] = new SqlParameter() { ParameterName = "@PublishingRoles", Value = pageModule.AuthorizedPublishingRoles };
            sqlParams[13] = new SqlParameter() { ParameterName = "@MoveModuleRoles", Value = pageModule.AuthorizedMoveModuleRoles };
            sqlParams[14] = new SqlParameter() { ParameterName = "@DeleteModuleRoles", Value = pageModule.AuthorizedDeleteModuleRoles };
            sqlParams[15] = new SqlParameter() { ParameterName = "@SupportWorkflow", Value = pageModule.SupportWorkflow };
            sqlParams[16] = new SqlParameter() { ParameterName = "@ApprovalRoles", Value = pageModule.AuthorizedApproveRoles };
            sqlParams[17] = new SqlParameter() { ParameterName = "@SupportCollapsable", Value = pageModule.SupportCollapsable };
            sqlParams[18] = new SqlParameter() { ParameterName = "@ShowEveryWhere", Value = pageModule.ShowEveryWhere };

            return SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
        }

        public int DeletePageModule(int moduleId)
        {
            const string Sproc = "rb_DeleteModule";

            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID ", Value = moduleId };

            return SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
        }

        public List<PageModule> GetModulesByPortalID(int portalId)
        {
            const string procudure = "rb_GetHtmlModuleByPortalID";
            var result = new List<PageModule>();
            var sqlParam = new SqlParameter[1];
            sqlParam[0] = new SqlParameter() { ParameterName = "@portalID", Value = portalId };

            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.StoredProcedure, procudure, sqlParam))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var item = new PageModule();
                        item.ModuleID = reader.GetInt32(0);
                        item.ModuleTitle = reader.GetString(1);
                        result.Add(item);
                    };

                }
            }
            return result;
        }

        #endregion

        #region HtmlText

        public int CreateHtmlText(HtmlText item)
        {
            const string Sql = "INSERT INTO [rb_HtmlText] ([ModuleID], [DesktopHtml], [MobileSummary], [MobileDetails],[VersionNo],[CreatedDate],[CreatedByUserName]) VALUES (@ModuleID, @DesktopHtml, @MobileSummary, @MobileDetails,@VersionNo,@CreatedDate,@CreatedByUserName);";

            var sqlParams = new SqlParameter[7];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID", Value = item.ModuleId };
            sqlParams[1] = new SqlParameter() { ParameterName = "@DesktopHtml", Value = item.DesktopHtml };
            sqlParams[2] = new SqlParameter() { ParameterName = "@MobileSummary", Value = item.MobileSummary };
            sqlParams[3] = new SqlParameter() { ParameterName = "@MobileDetails", Value = item.MobileDetails };
            sqlParams[4] = new SqlParameter() { ParameterName = "@VersionNo", Value = item.VersionNo };
            sqlParams[5] = new SqlParameter() { ParameterName = "@CreatedDate", Value = item.CreatedDate };
            sqlParams[6] = new SqlParameter() { ParameterName = "@CreatedByUserName", Value = item.CreatedBy };

            var result = SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, Sql, sqlParams);
            return result;
        }

        public HtmlText RetrieveHtmlText(int id)
        {
            const string Sql = "SELECT [ModuleID],[DesktopHtml],[MobileSummary],[MobileDetails],[VersionNo] FROM [rb_HtmlText] WHERE [ModuleID] = @ModuleID and published=1";
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID", Value = id };

            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.Text, Sql, sqlParams))
            {
                if (reader.HasRows)
                {
                    var moduleIdIndex = reader.GetOrdinal("ModuleID");
                    var desktopIndex = reader.GetOrdinal("DesktopHtml");
                    var mobileSummaryIndex = reader.GetOrdinal("MobileSummary");
                    var mobileDetailsIndex = reader.GetOrdinal("MobileDetails");
                    var versionNoIndex = reader.GetOrdinal("VersionNo");

                    reader.Read();
                    var item = new HtmlText()
                    {
                        ModuleId = reader.GetInt32(moduleIdIndex),
                        DesktopHtml = reader.GetString(desktopIndex),
                        MobileDetails = reader.GetString(mobileDetailsIndex),
                        MobileSummary = reader.GetString(mobileSummaryIndex),
                        VersionNo = reader.GetInt32(versionNoIndex)
                    };
                    return item;
                }
            }

            return null;
        }

        public int UpdateHtmlText(HtmlText item)
        {
            const string Sql = "UPDATE [rb_HtmlText] SET [DesktopHtml]=@DesktopHtml,[MobileSummary]=@MobileSummary,[MobileDetails]=@MobileDetails WHERE [ModuleID] = @ModuleID and [VersionNo]=@VersionNo";
            var sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter() { ParameterName = "@DesktopHtml", Value = item.DesktopHtml };
            sqlParams[1] = new SqlParameter() { ParameterName = "@MobileSummary", Value = item.MobileSummary };
            sqlParams[2] = new SqlParameter() { ParameterName = "@MobileDetails", Value = item.MobileDetails };
            sqlParams[3] = new SqlParameter() { ParameterName = "@ModuleID", Value = item.ModuleId };
            sqlParams[4] = new SqlParameter() { ParameterName = "@VersionNo", Value = item.VersionNo };

            var result = SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, Sql, sqlParams);
            return result;
        }

        public int DeleteHtmlText(int id, int versionNo)
        {
            const string Sql = "DELETE FROM [rb_HtmlText] WHERE [ModuleID] = @ModuleID and [VersionNo]=@VersionNo";
            var sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID", Value = id };
            sqlParams[1] = new SqlParameter() { ParameterName = "@VersionNo", Value = versionNo };

            var result = SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, Sql, sqlParams);
            return result;
        }

        #endregion

        #region HtmlVersion
        public int UpdateHtmlVersion(HtmlText item)
        {
            if (item.Published)
            {
                // It will make other versions unpublish
                const string querypublish = "UPDATE rb_HtmlText SET Published = 0 WHERE ModuleID = @ModuleID";
                var sqlPublishParam = new SqlParameter[1];
                sqlPublishParam[0] = new SqlParameter() { ParameterName = "@ModuleID", Value = item.ModuleId };
                SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, querypublish, sqlPublishParam);
            }
            const string Sproc = "UPDATE rb_HtmlText SET DesktopHtml = @DesktopHtml,MobileSummary = @MobileSummary,MobileDetails = @MobileDetails,VersionNo = @VersionNo,ModifiedDate = @ModifiedDate,ModifiedByUserName = @ModifiedByUserName,Published = @Published WHERE ModuleID = @ModuleID AND VersionNo = @VersionNo";
            var sqlParams = this.SetSqlParmas(item);
            SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, Sproc, sqlParams);
            return 2;
        }

        private SqlParameter[] SetSqlParmas(HtmlText item, bool isNew = false)
        {
            var sqlParams = new SqlParameter[8];
            if (isNew)
            {
                sqlParams = new SqlParameter[10];
            }
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID", Value = item.ModuleId };
            sqlParams[1] = new SqlParameter() { ParameterName = "@DesktopHtml", Value = item.DesktopHtml };
            sqlParams[2] = new SqlParameter() { ParameterName = "@MobileSummary", Value = item.MobileSummary };
            sqlParams[3] = new SqlParameter() { ParameterName = "@MobileDetails", Value = item.MobileDetails };
            sqlParams[4] = new SqlParameter() { ParameterName = "@VersionNo", Value = item.VersionNo };
            sqlParams[5] = new SqlParameter() { ParameterName = "@Published", Value = item.Published };
            sqlParams[6] = new SqlParameter() { ParameterName = "@ModifiedDate", Value = item.ModifiedDate };
            sqlParams[7] = new SqlParameter() { ParameterName = "@ModifiedByUserName", Value = item.ModifiedBy };
            if (isNew)
            {
                sqlParams[8] = new SqlParameter() { ParameterName = "@CreatedDate", Value = item.CreatedDate };
                sqlParams[9] = new SqlParameter() { ParameterName = "@CreatedByUserName", Value = item.CreatedBy };
            }
            // sqlParams[10] = new SqlParameter() { ParameterName = "@ReturnCode", Direction = ParameterDirection.ReturnValue };
            return sqlParams;
        }

        public int CreateNewHtmlVersion(HtmlText item)
        {
            const string Sql = "select * from rb_HtmlText where ModuleID=@ModuleID";
            var sqlParams = new SqlParameter[1];
            int newVersionNo = 1;
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID", Value = item.ModuleId };
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.Text, Sql, sqlParams))
            {
                while (reader.Read())
                {
                    newVersionNo++;
                }
            }
            item.VersionNo = newVersionNo;

            const string SqlInsert = "INSERT INTO rb_HtmlText ([ModuleID],[DesktopHtml],[MobileSummary],[MobileDetails],[VersionNo],[CreatedDate],[CreatedByUserName],[Published]) VALUES (@ModuleID,@DesktopHtml,@MobileSummary,@MobileDetails,@VersionNo,@CreatedDate,@CreatedByUserName,@Published	)";
            var sqlAddParams = this.SetSqlParmas(item, true);
            SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, SqlInsert, sqlAddParams);
            return 1;
        }

        public List<HtmlText> GetHtmlVersionList(int moduleid)
        {
            const string Sproc = "SELECT VersionNo, CreatedByUserName, CreatedDate, ModifiedByUserName, ModifiedDate, Published from rb_HtmlText where ModuleID = @ModuleID";
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID", Value = moduleid };
            List<HtmlText> lstHtml = new List<HtmlText>();
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.Text, Sproc, sqlParams))
            {
                if (reader.HasRows)
                {
                    var createduser = reader.GetOrdinal("CreatedByUserName");
                    var createddt = reader.GetOrdinal("CreatedDate");
                    var modifieddt = reader.GetOrdinal("ModifiedByUserName");
                    var versionno = reader.GetOrdinal("VersionNo");
                    var published = reader.GetOrdinal("Published");

                    while (reader.Read())
                    {
                        var htmlVersion = new HtmlText()
                         {
                             VersionNo = reader.GetInt32(versionno),
                             CreatedBy = reader.GetString(createduser),
                             CreatedDate = reader.GetDateTime(createddt),
                             Published = reader.GetBoolean(published),
                             ModifiedBy = SqlClientHelper.GetNullableString(reader, modifieddt)
                             
                         };
                        lstHtml.Add(htmlVersion);
                    }

                    while (reader.Read())
                    {
                        var htmlVersion = new HtmlText()
                        {
                            CreatedBy = reader.GetString(reader.GetOrdinal("CreatedByUserName")),
                            DesktopHtml = reader.GetString(reader.GetOrdinal("DesktopHtml")),
                            MobileSummary = reader.GetString(reader.GetOrdinal("MobileSummary")),
                            MobileDetails = reader.GetString(reader.GetOrdinal("MobileDetails")),
                            VersionNo = reader.GetInt32(reader.GetOrdinal("VersionNo")),
                            Published = reader.GetBoolean(reader.GetOrdinal("Published")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            ModifiedBy = SqlClientHelper.GetNullableString(reader, reader.GetOrdinal("ModifiedByUserName"))
                        };
                        lstHtml.Add(htmlVersion);
                    }
                }
            }
            return lstHtml;
            //SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams);
        }

        public HtmlText GetHtmlByVersion(int moduleid, int versionno)
        {
            const string Sproc = "rb_GetHtmlText";
            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ModuleID", Value = moduleid };
            sqlParams[1] = new SqlParameter() { ParameterName = "@VersionNo", Value = versionno };
            sqlParams[2] = new SqlParameter() { ParameterName = "@WorkflowVersion", Value = 1 };
            HtmlText htmlVersion = null;
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.StoredProcedure, Sproc, sqlParams))
            {
                if (reader.HasRows)
                {
                    var createduser = reader.GetOrdinal("CreatedByUserName");
                    var desktophtml = reader.GetOrdinal("DesktopHtml");
                    var mobsum = reader.GetOrdinal("MobileSummary");
                    var mobdetail = reader.GetOrdinal("MobileDetails");
                    var published = reader.GetOrdinal("Published");
                    var createddt = reader.GetOrdinal("CreatedDate");
                    var modifieddt = reader.GetOrdinal("ModifiedByUserName");

                    while (reader.Read())
                    {
                        htmlVersion = new HtmlText()
                        {
                            CreatedBy = reader.GetString(createduser),

                            DesktopHtml = reader.GetString(desktophtml),
                            MobileSummary = reader.GetString(mobsum),
                            MobileDetails = reader.GetString(mobdetail),

                            Published = reader.GetBoolean(published),
                            CreatedDate = reader.GetDateTime(createddt),
                            ModifiedBy = SqlClientHelper.GetNullableString(reader, modifieddt)
                        };
                    }
                }
            }
            return htmlVersion;

        }
        #endregion

        #region ModuleSetting
        public void CreateModuleSetting(ModuleSetting item)
        {
            const string Sql = "INSERT INTO [rb_ModuleSettings] ([ModuleID], [SettingName], [SettingValue]) VALUES (@ModuleID, @SettingName, @SettingValue);";
        }

        public ModuleSetting RetrieveModuleSetting(int id)
        {
            return new ModuleSetting();
        }

        public List<ModuleSetting> RetrieveAllModuleSettings(int moduleId)
        {
            return new List<ModuleSetting>();
        }

        public void UpdateModuleSetting(ModuleSetting item)
        {
        }

        public void DeleteModuleSetting(int id)
        {
        }
        #endregion

        #region Page
        public void CreatePage(Page item)
        {
            const string Sql = "INSERT INTO [rb_Pages] ([ParentPageID], [PageOrder], [PortalID], [PageName], [AuthorizedRoles], [PageDescription], [MobilePageName], [ShowMobile]) VALUES (@ParentPageID ,@PageOrder ,@PortalID ,@PageName ,@AuthorizedRoles ,@PageDescription ,@MobilePageName ,@ShowMobile); SELECT @@IDENTITY;";

            var sqlParams = new SqlParameter[8];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ParentPageID", Value = item.ParentPageID };
            sqlParams[1] = new SqlParameter() { ParameterName = "@PageOrder", Value = item.PageOrder };
            sqlParams[2] = new SqlParameter() { ParameterName = "@PortalID", Value = item.PortalID };
            sqlParams[3] = new SqlParameter() { ParameterName = "@PageName", Value = item.PageName };
            sqlParams[4] = new SqlParameter() { ParameterName = "@AuthorizedRoles", Value = item.AuthorizedRoles };
            sqlParams[5] = new SqlParameter() { ParameterName = "@PageDescription", Value = item.PageDescription };
            sqlParams[6] = new SqlParameter() { ParameterName = "@MobilePageName", Value = item.MobilePageName };
            sqlParams[7] = new SqlParameter() { ParameterName = "@ShowMobile", Value = item.ShowMobile };

            item.PageID = int.Parse(SqlClientHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, Sql, sqlParams));
        }

        public Page RetrievePage(int pageId)
        {
            const string Sql = "SELECT * FROM rb_Pages WHERE (PageID = @PageID)";
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@PageID", Value = pageId };
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.Text, Sql, sqlParams))
            {
                if (reader.HasRows)
                {
                    var pageIdIndex = reader.GetOrdinal("PageID");
                    var parentPageIdIndex = reader.GetOrdinal("ParentPageID");
                    var pageOrderIndex = reader.GetOrdinal("PageOrder");
                    var portalIdIndex = reader.GetOrdinal("PortalID");
                    var pageNameIndex = reader.GetOrdinal("PageName");
                    var mobilePageNameIndex = reader.GetOrdinal("MobilePageName");
                    var authorizedRolesIndex = reader.GetOrdinal("AuthorizedRoles");
                    var showMobileIndex = reader.GetOrdinal("ShowMobile");
                    var pageLayoutIndex = reader.GetOrdinal("PageLayout");
                    var pageDescriptionIndex = reader.GetOrdinal("PageDescription");

                    reader.Read();
                    var item = new Page()
                    {
                        PageID = reader.GetInt32(pageIdIndex),
                        PageOrder = reader.GetInt32(pageOrderIndex),
                        PortalID = reader.GetInt32(portalIdIndex),
                        PageName = reader.GetString(pageNameIndex),
                        MobilePageName = SqlClientHelper.GetNullableString(reader, mobilePageNameIndex),
                        AuthorizedRoles = SqlClientHelper.GetNullableString(reader, authorizedRolesIndex),
                        ShowMobile = reader.GetBoolean(showMobileIndex),
                        PageDescription = SqlClientHelper.GetNullableString(reader, pageDescriptionIndex),
                    };

                    if (!reader.IsDBNull(parentPageIdIndex))
                    {
                        item.ParentPageID = reader.GetInt32(parentPageIdIndex);
                    }

                    if (!reader.IsDBNull(pageLayoutIndex))
                    {
                        item.PageLayout = reader.GetInt32(pageLayoutIndex);
                    }

                    return item;
                }
            }

            return null;
        }

        public List<Page> RetrieveAllPages(int portalId)
        {
            const string Sql = "SELECT * FROM rb_Pages WHERE (PortalId = @PortalId)";
            var results = new List<Page>();
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@PortalId", Value = portalId };
            using (var reader = SqlClientHelper.GetReader(this.ConnectionString, CommandType.Text, Sql, sqlParams))
            {
                if (reader.HasRows)
                {
                    var pageIdIndex = reader.GetOrdinal("PageID");
                    var parentPageIdIndex = reader.GetOrdinal("ParentPageID");
                    var pageOrderIndex = reader.GetOrdinal("PageOrder");
                    var portalIdIndex = reader.GetOrdinal("PortalID");
                    var pageNameIndex = reader.GetOrdinal("PageName");
                    var mobilePageNameIndex = reader.GetOrdinal("MobilePageName");
                    var authorizedRolesIndex = reader.GetOrdinal("AuthorizedRoles");
                    var showMobileIndex = reader.GetOrdinal("ShowMobile");
                    var pageLayoutIndex = reader.GetOrdinal("PageLayout");
                    var pageDescriptionIndex = reader.GetOrdinal("PageDescription");

                    while (reader.Read())
                    {
                        var item = new Page()
                        {
                            PageID = reader.GetInt32(pageIdIndex),
                            PageOrder = reader.GetInt32(pageOrderIndex),
                            PortalID = reader.GetInt32(portalIdIndex),
                            PageName = reader.GetString(pageNameIndex),
                            MobilePageName = SqlClientHelper.GetNullableString(reader, mobilePageNameIndex),
                            AuthorizedRoles = SqlClientHelper.GetNullableString(reader, authorizedRolesIndex),
                            ShowMobile = reader.GetBoolean(showMobileIndex),
                            PageDescription = SqlClientHelper.GetNullableString(reader, pageDescriptionIndex),
                        };

                        if (!reader.IsDBNull(parentPageIdIndex))
                        {
                            item.ParentPageID = reader.GetInt32(parentPageIdIndex);
                        }

                        if (!reader.IsDBNull(pageLayoutIndex))
                        {
                            item.PageLayout = reader.GetInt32(pageLayoutIndex);
                        }

                        results.Add(item);
                    }
                }
            }

            return results;
        }

        public int UpdatePage(Page item)
        {
            const string Sql = @"UPDATE rb_Pages SET [ParentPageID] = @ParentPageID, [PageOrder] = @PageOrder, [PortalID] = @PortalID, [PageName] = @PageName, [MobilePageName] = @MobilePageName, [AuthorizedRoles] = @AuthorizedRoles, [ShowMobile] = @ShowMobile, [PageLayout] = @PageLayout, [PageDescription] = @PageDescription WHERE [PageID] = @PageId";
            var sqlParams = new SqlParameter[8];
            sqlParams[0] = new SqlParameter() { ParameterName = "@ParentPageID", Value = item.ParentPageID };
            sqlParams[1] = new SqlParameter() { ParameterName = "@PageOrder", Value = item.PageOrder };
            sqlParams[2] = new SqlParameter() { ParameterName = "@PortalID", Value = item.PortalID };
            sqlParams[3] = new SqlParameter() { ParameterName = "@PageName", Value = item.PageName };
            sqlParams[4] = new SqlParameter() { ParameterName = "@MobilePageName", Value = item.PageName };
            sqlParams[5] = new SqlParameter() { ParameterName = "@AuthorizedRoles", Value = item.AuthorizedRoles };
            sqlParams[6] = new SqlParameter() { ParameterName = "@ShowMobile", Value = item.ShowMobile };
            sqlParams[7] = new SqlParameter() { ParameterName = "@PageLayout", Value = item.PageLayout };
            sqlParams[8] = new SqlParameter() { ParameterName = "@PageDescription", Value = item.PageDescription };
            sqlParams[9] = new SqlParameter() { ParameterName = "@PageId", Value = item.PageID };

            return SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, Sql, sqlParams);
        }

        public int DeletePage(int pageId)
        {
            const string Sql = "DELETE rb_Pages WHERE [PageID] = @PageId;";

            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter() { ParameterName = "@PageId ", Value = pageId };

            return SqlClientHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, Sql, sqlParams);
        }
        #endregion



    }
}
