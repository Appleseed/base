namespace Appleseed.Portal.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Transactions;

    using Appleseed.Portal.Core.Models;
    using Appleseed.Portal.Database.Context;

    using Common.Logging;

    public class ModuleDomain : IModuleDomain
    {
        private readonly ILog log;

        private readonly IModuleDatabase database;

        public ModuleDomain(ILog log, IModuleDatabase database)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            this.database = database;
            this.log = log;
        }

        #region Page

        public List<ValidationResult> CreatePage(Page item)
        {
            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                using (var txn = new TransactionScope())
                {
                    try
                    {
                        this.database.CreatePage(item);

                        // TODO: add settings to the database

                        txn.Complete();
                    }
                    catch (SqlException dbException)
                    {
                        this.log.Debug(dbException);
                        errors.Add(
                            new ValidationResult(
                                "A database error has occurred while creating a page.  Error message has been logged."));
                    }
                    catch (Exception ex)
                    {
                        this.log.Debug(ex);
                        errors.Add(
                            new ValidationResult(
                                "An unhandled error has occurred while creating a page.  Error message has been logged."));
                    }
                }
            }

            return errors;
        }

        public Page RetrievePage(int pageId)
        {
            return this.database.RetrievePage(pageId);
        }

        public List<Page> RetrieveAllPages(int portalId)
        {
            return this.database.RetrieveAllPages(portalId);
        }

        public List<ValidationResult> UpdatePage(Page item)
        {
            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                    this.database.UpdatePage(item);
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(
                        new ValidationResult(
                            "A database error has occurred while updating page.  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(
                        new ValidationResult(
                            "An unhandled error has occurred while updating page.  Error message has been logged."));
                }
            }

            return errors;
        }

        public List<ValidationResult> DeletePage(int pageId)
        {
            var errors = new List<ValidationResult>();
            try
            {
                var result = this.database.DeletePage(pageId);

                //if (result.Equals(0))
                //{
                //}
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(
                    new ValidationResult(
                        "A database error has occurred while deleting the page.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(
                    new ValidationResult(
                        "An unhandled error has occurred while deleting the page.  Error message has been logged."));
            }

            return errors;
        }

        #endregion

        #region Module

        public List<ValidationResult> CreatePageModule(PageModule item)
        {
            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                    this.database.CreatePageModule(item);

                    // TODO: add settings to the database
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(new ValidationResult("A database error has occurred while creating the module.  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(new ValidationResult("An unhandled error has occurred while creating the module.  Error message has been logged."));
                }
            }

            return errors;
        }

        public PageModule RetrievePageModule(int moduleId)
        {
            return this.database.RetrievePageModule(moduleId);
        }

        public List<PageModule> RetrieveAllPageModules(int pageId)
        {
            return this.database.RetrieveAllPageModules(pageId);
        }

        public List<ValidationResult> UpdatePageModule(PageModule item)
        {
            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                    this.database.UpdatePageModule(item);
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(
                        new ValidationResult(
                            "A database error has occurred while updating the module.  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(
                        new ValidationResult(
                            "An unhandled error has occurred while updating the module.  Error message has been logged."));
                }
            }

            return errors;
        }

        public List<ValidationResult> DeletePageModule(int moduleId)
        {
            var errors = new List<ValidationResult>();
            try
            {
                var result = this.database.DeletePageModule(moduleId);

                //if (result.Equals(0))
                //{
                //}
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(
                    new ValidationResult(
                        "A database error has occurred while deleting the module.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(
                    new ValidationResult(
                        "An unhandled error has occurred while deleting the module.  Error message has been logged."));
            }

            return errors;
        }

        public List<PageModule> GetModulesByPortalID(int portalId)
        {
            return this.database.GetModulesByPortalID(portalId);
        }

        #endregion

        #region HtmlText

        public List<ValidationResult> CreateHtmlText(HtmlText item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                    this.database.CreateHtmlText(item);
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(new ValidationResult("A database error has occurred while creating the html text module.  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(new ValidationResult("An unhandled error has occurred while creating the html text module.  Error message has been logged."));
                }
            }

            return errors;
        }

        public HtmlText RetrieveHtmlText(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("id");
            }

            return this.database.RetrieveHtmlText(id);
        }

        public List<ValidationResult> UpdateHtmlText(HtmlText item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                    this.database.UpdateHtmlText(item);
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(
                        new ValidationResult(
                            "A database error has occurred while updating the html text module.  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(
                        new ValidationResult(
                            "An unhandled error has occurred while updating the html text module.  Error message has been logged."));
                }
            }

            return errors;
        }

        public List<ValidationResult> DeleteHtmlText(int id, int versionNo)
        {
            if (id <= 0 && versionNo <= 0)
            {
                throw new ArgumentNullException("id");
            }

            var errors = new List<ValidationResult>();
            try
            {
                var result = this.database.DeleteHtmlText(id, versionNo);

                //if (result.Equals(0))
                //{
                //}
            }
            catch (SqlException dbException)
            {
                this.log.Debug(dbException);
                errors.Add(
                    new ValidationResult(
                        "A database error has occurred while deleting the module.  Error message has been logged."));
            }
            catch (Exception ex)
            {
                this.log.Debug(ex);
                errors.Add(
                    new ValidationResult(
                        "An unhandled error has occurred while deleting the module.  Error message has been logged."));
            }

            return errors;
        }

        #endregion

        #region HtmlVersion
        public int UpdateHtmlVersionText(HtmlText item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                   return  this.database.UpdateHtmlVersion(item);
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(new ValidationResult("A database error has occurred while updating the Versioned html text .  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(new ValidationResult("An unhandled error has occurred while updating the Versioned html text .  Error message has been logged."));
                }
            }

            return 0;
        }

        public int CreateHtmlVersionText(HtmlText item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                    return this.database.CreateNewHtmlVersion(item);
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(new ValidationResult("A database error has occurred while creating the new Versioned html text .  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(new ValidationResult("An unhandled error has occurred while creating the new Versioned html text .  Error message has been logged."));
                }
            }
            return  0;

        }

        public List<HtmlText> GetHtmlVersionList(int moduleid)
        {
            if (moduleid <= 0)
            {
                throw new ArgumentNullException("moduleid");
            }
            return this.database.GetHtmlVersionList(moduleid);
        }

        public HtmlText GetHtmlByVersion(int moduleid, int versionno)
        {
            if (moduleid <= 0)
            {
                throw new ArgumentNullException("moduleid");
            }
            return this.database.GetHtmlByVersion(moduleid,versionno);
        }

        #endregion
    }
}
