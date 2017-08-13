using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appleseed.Base.Alerts.Model;
using System.Data;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using Appleseed.Base.Alerts.Controller;

namespace Appleseed.Base.Alerts.Providers
{
    class EmailAlertProvider : IAlert
    {
        static IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        static string SolrURL = System.Configuration.ConfigurationManager.AppSettings["SolrURL"];

        static string MailSubject = System.Configuration.ConfigurationManager.AppSettings["MailSubject"];
        static string MailHeaderText = System.Configuration.ConfigurationManager.AppSettings["MailHeaderText"];
        static string MailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
        static string MailFromName = System.Configuration.ConfigurationManager.AppSettings["MailFromName"];

        static string APIKey = System.Configuration.ConfigurationManager.AppSettings["SendGridAPIKey"];
        static string RefreshQuery = System.Configuration.ConfigurationManager.AppSettings["RefreshQuery"];

        public List<UserAlert> GetUserAlertSchedules(string scheudle)
        {
            var userAlerts = db.Query<UserAlert>("GetPortalUserAlerts", new { alert_schedule = scheudle },
            commandType: CommandType.StoredProcedure).ToList<UserAlert>();

            return userAlerts;
        }

        public RootSolrObject GetSearchAlertViaSolr(string query)
        {
            // perform split function

            string url = SolrURL + WebUtility.HtmlDecode(query) + WebUtility.HtmlDecode(RefreshQuery);
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
            getRequest.Method = "GET";

            try
            {
                using (var getResponse = (HttpWebResponse)getRequest.GetResponse())
                {
                    Stream newStream = getResponse.GetResponseStream();
                    StreamReader sr = new StreamReader(newStream);

                    var result = sr.ReadToEnd();

                    var searchResults = JsonConvert.DeserializeObject<RootSolrObject>(result);

                    return searchResults;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : Reason - " + ex.Message);
                return null;
            }

        }

        public async Task SendAlert(string email, string link, RootSolrObject results, object mailResponse)
        {

            if (results != null && results.response != null && results.response.docs != null && results.response.docs.Count() > 0)
            {
                var client = new SendGridClient(APIKey);
                var from = new EmailAddress(MailFrom, MailFromName);
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

                    //  Create custom Email Alert 


                    //sbHtmlContent.Append("");
                }

                //Footer
                sbHtmlContent.Append("<br/><br/>");
                sbHtmlContent.Append("<a href=" + link + "> More Search Results </a>");

                var htmlContent = sbHtmlContent.ToString();
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                mailResponse = await client.SendEmailAsync(msg);
            }
        }
    }
}
