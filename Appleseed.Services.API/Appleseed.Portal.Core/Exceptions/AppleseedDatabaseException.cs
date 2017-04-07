namespace Appleseed.Portal.Core.Exceptions
{
    using System;

    public class AppleseedDatabaseException : Exception
    {
        public AppleseedDatabaseException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
