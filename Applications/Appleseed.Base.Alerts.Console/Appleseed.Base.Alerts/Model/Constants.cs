using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appleseed.Base.Alerts.Model
{
    class Constants
    {
        #region App Config Values
        // Test Settings
        static string Mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];
        static string TestEmail = System.Configuration.ConfigurationManager.AppSettings["TestEmail"];
        static string TestSearchQuery = System.Configuration.ConfigurationManager.AppSettings["TestSearchQuery"];
        static string TestSearchLink = System.Configuration.ConfigurationManager.AppSettings["TestSearchLink"];
        //SendGridAPI Key
        static string APIKey = System.Configuration.ConfigurationManager.AppSettings["SendGridAPIKey"];
        //Mail Settings
        static string MailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
        static string MailFromName = System.Configuration.ConfigurationManager.AppSettings["MailFromName"];
        static string MailSubject = System.Configuration.ConfigurationManager.AppSettings["MailSubject"];
        static string MailHeaderText = System.Configuration.ConfigurationManager.AppSettings["MailHeaderText"];
        static string MailSchedule = System.Configuration.ConfigurationManager.AppSettings["MailSchedule"];
        //Search Settings
        static string SiteSearchLink = System.Configuration.ConfigurationManager.AppSettings["SiteSearchLink"];
        static string SolrURL = System.Configuration.ConfigurationManager.AppSettings["SolrURL"];
        static string RefreshQuery = System.Configuration.ConfigurationManager.AppSettings["RefreshQuery"];
        //Data Settings
        static IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        #endregion
    }
}
