﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appleseed.Base.Alerts.Model
{
    public class Constants
    {
        #region App Config Values
        // Test Settings
        public static string Mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];
        public static string TestEmail = System.Configuration.ConfigurationManager.AppSettings["TestEmail"];
        public static string TestSearchQuery = System.Configuration.ConfigurationManager.AppSettings["TestSearchQuery"];
        public static string TestSearchLink = System.Configuration.ConfigurationManager.AppSettings["TestSearchLink"];
        public static string StringLimit = System.Configuration.ConfigurationManager.AppSettings["StringLimit"];
        //SendGridAPI Key
        public static string APIKey = System.Configuration.ConfigurationManager.AppSettings["SendGridAPIKey"];
        //Mail Settings
        public static string MailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
        public static string MailFromName = System.Configuration.ConfigurationManager.AppSettings["MailFromName"];
        public static string MailSubject = System.Configuration.ConfigurationManager.AppSettings["MailSubject"];
        public static string MailHeaderText = System.Configuration.ConfigurationManager.AppSettings["MailHeaderText"];
        //public static string MailSchedule = System.Configuration.ConfigurationManager.AppSettings["MailSchedule"];
        public static string GetUserAlertQuery = System.Configuration.ConfigurationManager.AppSettings["GetUserAlertQuery"];
        public static string UpdateUserSendQuery = System.Configuration.ConfigurationManager.AppSettings["UpdateUserSendQuery"];
        

        //Search Settings
        public static string SiteSearchLink = System.Configuration.ConfigurationManager.AppSettings["SiteSearchLink"];
        public static string SolrURL = System.Configuration.ConfigurationManager.AppSettings["SolrURL"];
        public static string RefreshQuery = System.Configuration.ConfigurationManager.AppSettings["RefreshQuery"];
      
        #endregion
    }
}
