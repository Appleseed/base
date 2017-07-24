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

        public List<UserAlert> GetUserAlertSchedules(string scheudle)
        {
            var userAlerts = db.Query<UserAlert>("GetPortalUserAlerts", new { alert_schedule = scheudle },
            commandType: CommandType.StoredProcedure).ToList<UserAlert>();

            return userAlerts;
        }

        public RootSolrObject GetSearchAlertViaSolr(string query)
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

        public async Task SendAlert(string email, string link, RootSolrObject results)
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

                    sbHtmlContent.Append("<br/><h2>" + Helpers.UppercaseFirst(results.response.docs[i].item_type[0]) + " : " + results.response.docs[i].recall_number[0] + "</h2>");
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

                var htmlContent = sbHtmlContent.ToString();
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
        }
    }
}
