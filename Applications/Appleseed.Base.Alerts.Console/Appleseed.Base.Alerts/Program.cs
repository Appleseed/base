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

        static RootSolrObject GetSearchAlertViaSolr(string query)
        {
            // perform split function

            string url = SolrURL + WebUtility.HtmlDecode(query);
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
            getRequest.Method = "GET";

            using (var getResponse = (HttpWebResponse)getRequest.GetResponse())
            {
                Stream newStream = getResponse.GetResponseStream();
                StreamReader sr = new StreamReader(newStream);

                var result = sr.ReadToEnd();

                var searchResults = JsonConvert.DeserializeObject<RootSolrObject>(result);

                return searchResults;

            }

        }

        static async Task SendAlert(string email, string link, RootSolrObject results)
        {

            if (results != null && results.response != null && results.response.docs != null && results.response.docs.Count() > 0)
            {
                var client = new SendGridClient(APIKey);
                var from = new EmailAddress(MailFrom, "Mailer");
                var subject = MailSubject;
                var to = new EmailAddress(email, null);

                var plainTextContent = " ";

                StringBuilder sbHtmlContent = new StringBuilder();

                //Header
                sbHtmlContent.Append("<h1>" + MailHeaderText + "</h1>");
                sbHtmlContent.Append("<br/><br/>");

                for (int i = 0; i < results.response.docs.Count(); i++)
                {
                    sbHtmlContent.Append("<br/><br/>");

                    sbHtmlContent.Append("<br/><h2>" + UppercaseFirst(results.response.docs[i].item_type[0]) + " : " + results.response.docs[i].recall_number[0] + "</h2>");
                    sbHtmlContent.Append("<strong>Status: </strong>" + results.response.docs[i].status[0] + "<br/>");
                    sbHtmlContent.Append("<strong>Classification: </strong>" + results.response.docs[i].classification + "<br/>");
                    sbHtmlContent.Append("<strong>Description: </strong>" + results.response.docs[i].product_description[0] + "<br/>");
                    sbHtmlContent.Append("<strong>Code Info: </strong>" + results.response.docs[i].code_info[0] + "<br/>");
                    sbHtmlContent.Append("<strong>Recall Reason: </strong> " + results.response.docs[i].reason_for_recall[0] + "<br/>");
                    sbHtmlContent.Append("<strong>Voluntary Mandated: </strong> " + results.response.docs[i].voluntary_mandated[0] + "<br/>");
                    sbHtmlContent.Append("<strong>Product Quantity: </strong> " + results.response.docs[i].reason_for_recall[0] + "<br/>");
                    sbHtmlContent.Append("<strong>Recalling Firm: </strong> " + results.response.docs[i].recalling_firm + "<br/>");
                    sbHtmlContent.Append("<strong>Recalling Firm Address: </strong> " + results.response.docs[i].address_1[0] + "<br/>");


                    //sbHtmlContent.Append("Report Date : " + results.response.docs[i].report_date + "<br/>");
                }

                //Footer
                sbHtmlContent.Append("<br/><br/>");
                sbHtmlContent.Append("<a href=" + link + "> More Search Results </a>");

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
