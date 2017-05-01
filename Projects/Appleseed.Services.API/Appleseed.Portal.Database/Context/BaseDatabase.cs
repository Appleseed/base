namespace Appleseed.Portal.Database.Context
{
    using System;

    using Common.Logging;

    public abstract class BaseDatabase
    {
        protected readonly ILog Logger;

        protected readonly string ConnectionString;

        protected readonly string ApplicationName;

        protected readonly int PortalId;

        protected BaseDatabase(ILog logger, string connectionString, string applicationName, int portalId)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName");
            }

            if (portalId < 0)
            {
                throw new ArgumentNullException("portalId");
            }

            this.Logger = logger;
            this.ConnectionString = connectionString;
            this.ApplicationName = applicationName;
            this.PortalId = portalId;
        }
    }
}
