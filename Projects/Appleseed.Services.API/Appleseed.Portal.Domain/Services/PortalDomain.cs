namespace Appleseed.Portal.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Transactions;

    using Appleseed.Portal.Database.Context;
    using Appleseed.Portal.Core.Models;

    using Common.Logging;

    public class PortalDomain : IPortalDomain
    {
        private readonly ILog log;

        private readonly IPortalDatabase portalContext;

        public PortalDomain(ILog log, IPortalDatabase portalContext)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            if (portalContext == null)
            {
                throw new ArgumentNullException("portalContext");
            }

            this.portalContext = portalContext;
            this.log = log;
        }

        public List<ValidationResult> CreatePortal(Portal item)
        {
            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                using (var txn = new TransactionScope())
                {
                    try
                    {
                        this.portalContext.CreatePortal(item);
                        txn.Complete();
                    }
                    catch (SqlException dbException)
                    {
                        this.log.Debug(dbException);
                        errors.Add(
                            new ValidationResult(
                                "A database error has occurred while creating a portal.  Error message has been logged."));
                    }
                    catch (Exception ex)
                    {
                        this.log.Debug(ex);
                        errors.Add(
                            new ValidationResult(
                                "An unhandled error has occurred while creating a portal.  Error message has been logged."));
                    }
                }
            }

            return errors;
        }

        public Portal RetrievePortalById(int id)
        {
            // TODO: Add caching, retrieve item from cache

            return this.portalContext.RetrievePortal(id);
        }

        public List<Portal> RetrievePortals()
        {
            // TODO: Add caching, retrieve items from cache

            return this.portalContext.RetrievePortals();
        }

        public List<ValidationResult> UpdatePortal(Portal item)
        {
            var errors = Validation.Validate(item);
            if (!errors.Any())
            {
                try
                {
                    this.portalContext.UpdatePortal(item);
                }
                catch (SqlException dbException)
                {
                    this.log.Debug(dbException);
                    errors.Add(
                        new ValidationResult(
                            "A database error has occurred while updating portal.  Error message has been logged."));
                }
                catch (Exception ex)
                {
                    this.log.Debug(ex);
                    errors.Add(
                        new ValidationResult(
                            "An unhandled error has occurred while updating portal.  Error message has been logged."));
                }
            }

            return errors;
        }

        public List<ValidationResult> DeletePortal(int portalId)
        {
            var errors = new List<ValidationResult>();
            try
            {
                var result = this.portalContext.DeletePortal(portalId);

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
    }
}
