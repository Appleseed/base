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
using Appleseed.Base.Alerts.Model;
using Appleseed.Base.Alerts.Controller;

namespace Appleseed.Base.Alerts
{
  
    /// <summary>
    /// Main Program for Alert Notifications 
    /// </summary>
    class Program
    {
        #region App Config Values
        static string Mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];
        static string TestEmail = System.Configuration.ConfigurationManager.AppSettings["TestEmail"];
        static string TestSearchQuery = System.Configuration.ConfigurationManager.AppSettings["TestSearchQuery"];

        static string SolrURL = System.Configuration.ConfigurationManager.AppSettings["SolrURL"];
        static string APIKey = System.Configuration.ConfigurationManager.AppSettings["SendGridAPIKey"];

        static string MailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
        static string MailFromName = System.Configuration.ConfigurationManager.AppSettings["MailFromName"];
        static string MailSubject = System.Configuration.ConfigurationManager.AppSettings["MailSubject"];
        static string MailHeaderText = System.Configuration.ConfigurationManager.AppSettings["MailHeaderText"];
        static string MailSchedule = System.Configuration.ConfigurationManager.AppSettings["MailSchedule"];

        static string SiteSearchLink = System.Configuration.ConfigurationManager.AppSettings["SiteSearchLink"];
        static string SearchLinkText = System.Configuration.ConfigurationManager.AppSettings["SearchLinkText"];

        static IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        #endregion

        
        static void Main(string[] args)
        {
            CheckAlertSchedule(MailSchedule);
        }


        static void CheckAlertSchedule(string alert_schedule)
        {


            // Test Mode
            if (String.Compare(Mode, "production", true) != 0)
            {
                RootSolrObject response = GetSearchAlertViaSolr(TestSearchQuery);

                SendAlert(TestEmail, SearchLinkText, response).Wait();
            }
            else
            {
                // perform production option
                // Run SQL to pull schedule
                // Iterate through users and send emails

                var userAlerts = GetUserAlertSchedules(alert_schedule);

                if (userAlerts != null && userAlerts.Count > 0)
                {
                    foreach (UserAlert ua in userAlerts)
                    {
                        try
                        {
                            // Need a better split function here q=
                            RootSolrObject response = GetSearchAlertViaSolr(ua.source.Replace(SiteSearchLink, ""));
                            if (response != null)
                            {
                                SendAlert(ua.email, ua.source, response).Wait();
                            }


                        }
                        catch (Exception ex)
                        {
                            // log exception
                            Console.WriteLine("An error occured sending an email for " + ua.email);
                            Console.WriteLine("\nReason : " + ex.Message);
                        }
                    }
                }
            }

        }
        static List<UserAlert> GetUserAlertSchedules(string scheudle)
        {
              var userAlerts = db.Query<UserAlert>("GetPortalUserAlerts", new { alert_schedule = scheudle },
              commandType: CommandType.StoredProcedure).ToList<UserAlert>();

            return userAlerts;
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
                var from = new EmailAddress(MailFrom, MailFromName);
                var subject = MailSubject;
                var to = new EmailAddress(email, null);

                var plainTextContent = " ";

                StringBuilder sbHtmlContent = new StringBuilder();

                //Header
                sbHtmlContent.Append("<h1>" + MailHeaderText + "</h1>");
               

                for (int i = 0; i < results.response.docs.Count(); i++)
                {
                    sbHtmlContent.Append("<br/><br/><br/>");
					if (!String.IsNullOrEmpty(results.response.docs[i].item_type))
						sbHtmlContent.Append("<h2>" + UppercaseFirst(results.response.docs[i].item_type)+ "</h2>");
                   
				    if (!String.IsNullOrEmpty(results.response.docs[i].recall_number))
						sbHtmlContent.Append( "<h2>" + results.response.docs[i].recall_number  + "</h2>");
					
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
