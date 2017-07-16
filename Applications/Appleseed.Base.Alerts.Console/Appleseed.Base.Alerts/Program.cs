using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Dapper;

namespace Appleseed.Base.Alerts
{
    class UserAlert
    {
        public Guid user_id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string source { get; set; }
    }

    class RootSolrObject
    {

        public SolrResponse response { get; set; }
    }
    class SolrResponse
    {

        public SolrResponseItem[] docs { get; set; }
    }
    class SolrResponseItem

    {
        public string id { get; set; }
        public string[] item_type { get; set; }

        public string[] address_1 { get; set; }

        public string city { get; set; }

        public string state { get; set; }
        public string classification { get; set; }
        public string country { get; set; }
        public string[] postal_code { get; set; }

        public string[] product_description { get; set; }
        public string[] product_quantity { get; set; }
        public string[] product_type { get; set; }
        public string[] code_info { get; set; }

        public string[] reason_for_recall { get; set; }
        public DateTime recall_initiation_date { get; set; }
        public string[] recall_number { get; set; }
        public string recalling_firm { get; set; }
        public string[] voluntary_mandated { get; set; }

        public DateTime report_date { get; set; }
        public string[] status { get; set; }


    }
    class Program
    {

        static string Mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];
        static string TestEmail = System.Configuration.ConfigurationManager.AppSettings["TestEmail"];
        static string SolrURL = System.Configuration.ConfigurationManager.AppSettings["SolrURL"];
        static string APIKey = System.Configuration.ConfigurationManager.AppSettings["SendGridAPIKey"];
        static string MailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
        static string MailSubject = System.Configuration.ConfigurationManager.AppSettings["MailSubject"];
        static string MailHeaderText = System.Configuration.ConfigurationManager.AppSettings["MailHeaderText"];

        static IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);




        static void Main(string[] args)
        {
            CheckAlertSchedule("daily");
        }
        static void CheckAlertSchedule(string alert_schedule)
        {


            // Test Mode
            if (Mode != "prod" || Mode != "production")
            {
                RootSolrObject response = GetSearchAlertViaSolr("*:*&fl=*&rows=10");

                SendAlert(TestEmail, "http://www.domain.com/Search.aspx#/q=*:*&fl=*&rows=10", response).Wait();
            }
            else
            {
                // perform production option
                // Run SQL to pull schedule
                // Iterate through users and send emails

                var userAlerts = db.Query<UserAlert>("GetPortalUserAlerts", new { alert_schedule = alert_schedule },
               commandType: CommandType.StoredProcedure).ToList<UserAlert>();

                if (userAlerts != null && userAlerts.Count > 0)
                {
                    foreach (UserAlert ua in userAlerts)
                    {
                        try
                        {
                            // Need a better split function here q=
                            RootSolrObject response = GetSearchAlertViaSolr(ua.source.Replace("http://www.domain.com/Search.aspx#/q=", ""));
                            if (response != null)
                            {
                                SendAlert(TestEmail, ua.source, response).Wait();
                            }


                        }
                        catch (Exception ex)
                        {
                            // log exception
                            ;
                        }
                    }
                }
            }

        }



        #region helpers
        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        #endregion
    }
}
