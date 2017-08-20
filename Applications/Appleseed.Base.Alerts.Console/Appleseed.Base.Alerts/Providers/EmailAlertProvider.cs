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
         
        public List<UserAlert> GetUserAlerts()
        {
            return Helpers.GetUserAlerts();
        }

        public bool UpdateUserSendDate(Guid userID, DateTime date)
        {
            return Helpers.UpdateUserSendDate(userID, date);
        }
        public JSONRootObject GetAlert(string query)
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

        /// <summary>
        /// To Do : Define Run operation for standard provider
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {

            // Test Mode
            if (String.Compare(Constants.Mode, "production", true) != 0)
            {
                Console.WriteLine("INFO : Mode - Test");
                JSONRootObject solrResponse = GetAlert(Constants.TestSearchQuery);
                Response mailResponse = null;


                try
                {
                    Console.WriteLine("INFO : Attempting to send a test mail to " + Constants.TestEmail);
                    SendAlert(Constants.TestEmail, Constants.TestSearchLink, solrResponse, mailResponse).Wait();
                    Console.WriteLine("INFO : Test Alert Sent");
                    return true;
                }
                catch (Exception ex)
                {
                    // log exception
                    Console.WriteLine("Error : An error occured sending an alert for Test user " + Constants.TestEmail);
                    Console.WriteLine("\nError : Reason - " + ex.Message);
                    return false;

                }
               
            }
            else
            {
                // perform production option
                // Run SQL to pull schedule
                // Iterate through users and send emails
                Console.WriteLine("INFO : Mode - Production");
                var userAlerts = Helpers.GetUserAlerts();
                int userSentCount = 0;
                bool error = false;

                if (userAlerts != null && userAlerts.Count > 0)
                {
                    Console.WriteLine("INFO : Alerts need to be sent to  " + userAlerts.Count + " users.");
                    foreach (UserAlert ua in userAlerts)
                    {
                        error = false;
                        try
                        {
                            // Need a better split function here q=
                            JSONRootObject solrResponse = GetAlert(ua.source.Replace(Constants.SiteSearchLink, ""));
                            Response mailResponse = null;

                            if (solrResponse != null)
                            {
                                SendAlert(ua.email, ua.source, solrResponse, mailResponse).Wait();
                            }


                        }
                        catch (Exception ex)
                        {
                            // log exception
                            Console.WriteLine("Error : An error occured sending an alert for user " + ua.email);
                            Console.WriteLine("\nError : Reason - " + ex.Message);
                            error = true;
                        }

                        if (!error)
                        {
                            userSentCount++;
                            Helpers.UpdateUserSendDate(ua.user_id, DateTime.Now);


                        }

                    }
                }
                Console.WriteLine("INFO : Sent " + userSentCount + " alerts.");
                return true;
            }
            return true;
        }
    }
}
