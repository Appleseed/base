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
   

        public List<UserAlert> GetUserAlertSchedules(string scheudle)
        {
            return Helpers.GetUserAlertSchedules(scheudle);
        }

        public bool UpdateUserSendDate(Guid userID, DateTime date)
        {
            return Helpers.UpdateUserSendDate(userID, date);
        }
        public JSONRootObject GetSearchAlert(string query)
        {
            // perform split function

            string url = Constants.SolrURL + WebUtility.HtmlDecode(query) + WebUtility.HtmlDecode(Constants.RefreshQuery);
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
            getRequest.Method = "GET";

            try
            {
                using (var getResponse = (HttpWebResponse)getRequest.GetResponse())
                {
                    Stream newStream = getResponse.GetResponseStream();
                    StreamReader sr = new StreamReader(newStream);

                    var result = sr.ReadToEnd();

                    var searchResults = JsonConvert.DeserializeObject<JSONRootObject>(result);

                    return searchResults;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : Reason - " + ex.Message);
                return null;
            }

        }

        public async Task SendAlert(string email, string link, JSONRootObject results, object mailResponse)
        {

            if (results != null && results.response != null && results.response.docs != null && results.response.docs.Count() > 0)
            {
                var client = new SendGridClient(Constants.APIKey);
                var from = new EmailAddress(Constants.MailFrom, Constants.MailFromName);
                var subject = Constants.MailSubject;
                var to = new EmailAddress(email, null);
                
                var plainTextContent = " ";

                StringBuilder sbHtmlContent = new StringBuilder();

                //Header
                sbHtmlContent.Append("<h1>" + Constants.MailHeaderText + "</h1>");
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
