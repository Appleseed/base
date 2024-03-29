﻿using System;
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
                Console.WriteLine("Error : Reason - " + ex.Message + "\n");
                return null;
            }

        }

        public async Task SendAlert(UserAlert ua, string link, JSONRootObject results, object mailResponse)
        {

            if (results != null && results.response != null && results.response.docs != null && results.response.docs.Count() > 0)
            {
                var client = new SendGridClient(Constants.APIKey);
                var from = new EmailAddress(Constants.MailFrom, Constants.MailFromName);
                var subject = Constants.MailSubject;
                var to = new EmailAddress(ua.email, null);

                var plainTextContent = " ";
                var stringLimit = Int32.Parse(Constants.StringLimit);

                if (!String.IsNullOrEmpty(ua.name))
                    subject = subject + " - " + ua.name;

                StringBuilder sbHtmlContent = new StringBuilder();

                //Header
                sbHtmlContent.Append("<h1>" + Constants.MailHeaderText + "</h1>");


                for (int i = 0; i < results.response.docs.Count(); i++)
                {
                    sbHtmlContent.Append("<br/><br/><br/>");

                    if (!String.IsNullOrEmpty(results.response.docs[i].item_type))
                        sbHtmlContent.Append("<h2>" + Helpers.UppercaseFirst(results.response.docs[i].item_type) + "</h2>");

                    if (!String.IsNullOrEmpty(results.response.docs[i].name))
                        sbHtmlContent.Append("<h2>" + Helpers.UppercaseFirst(results.response.docs[i].name) + "</h2>");

                    if (!String.IsNullOrEmpty(results.response.docs[i].recall_number))
                        sbHtmlContent.Append("<h2>" + results.response.docs[i].recall_number + "</h2>");
                    // IRE
                    if (!String.IsNullOrEmpty(results.response.docs[i].status))
                        sbHtmlContent.Append("<strong>Status: </strong>" + results.response.docs[i].status + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].classification))
                        sbHtmlContent.Append("<strong>Classification: </strong>" + results.response.docs[i].classification + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].product_description))
                        sbHtmlContent.Append("<strong>Product Description: </strong>" + results.response.docs[i].product_description + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].code_info))
                        sbHtmlContent.Append("<strong>Code Info: </strong>" + results.response.docs[i].code_info + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].reason_for_recall))
                        sbHtmlContent.Append("<strong>Recall Reason: </strong> " + results.response.docs[i].reason_for_recall + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].voluntary_mandated))
                        sbHtmlContent.Append("<strong>Voluntary Mandated: </strong> " + results.response.docs[i].voluntary_mandated + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].product_quantity))
                        sbHtmlContent.Append("<strong>Product Quantity: </strong> " + results.response.docs[i].product_quantity + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].recalling_firm))
                        sbHtmlContent.Append("<strong>Recalling Firm: </strong> " + results.response.docs[i].recalling_firm + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].address_1))
                        sbHtmlContent.Append("<strong>Recalling Firm Address: </strong> " + results.response.docs[i].address_1 + "<br/>");
                    //IRefusal
                    if (!String.IsNullOrEmpty(results.response.docs[i].fei_number))
                        sbHtmlContent.Append("<strong>Fei Number: </strong> " + results.response.docs[i].fei_number + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].product_code))
                        sbHtmlContent.Append("<strong>Product Code: </strong> " + results.response.docs[i].product_code + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].product_code_description))
                        sbHtmlContent.Append("<strong>Product Code Description: </strong> " + results.response.docs[i].product_code_description + "<br/>");
                    if (results.response.docs[i].refusal_date != null && results.response.docs[i].refusal_date > DateTime.Now.AddYears(-15))
                        sbHtmlContent.Append("<strong>Refusal Date: </strong> " + results.response.docs[i].refusal_date + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].entry_number))
                        sbHtmlContent.Append("<strong>Entry Number: </strong> " + results.response.docs[i].entry_number + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].rfrnc_doc_id))
                        sbHtmlContent.Append("<strong>Refrence Doc ID: </strong> " + results.response.docs[i].rfrnc_doc_id + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].line_number))
                        sbHtmlContent.Append("<strong>Line Number: </strong> " + results.response.docs[i].line_number + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].line_sfx_id))
                        sbHtmlContent.Append("<strong>Line Suffix ID: </strong> " + results.response.docs[i].line_sfx_id + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].fda_sample_analysis))
                        sbHtmlContent.Append("<strong>FDA Sample Analysis: </strong> " + results.response.docs[i].fda_sample_analysis + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].private_lab_analysis))
                        sbHtmlContent.Append("<strong>Private Lab Analysis: </strong> " + results.response.docs[i].private_lab_analysis + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].refusal_charges))
                        sbHtmlContent.Append("<strong>Refusal Charges: </strong> " + results.response.docs[i].refusal_charges + "<br/>");
                    //ICitation
                    if (!String.IsNullOrEmpty(results.response.docs[i].act_cfr))
                        sbHtmlContent.Append("<strong>Act CFR: </strong> " + results.response.docs[i].act_cfr + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].program_area))
                        sbHtmlContent.Append("<strong>Program Area: </strong> " + results.response.docs[i].program_area + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].description_short))
                        sbHtmlContent.Append("<strong>Short Description: </strong> " + results.response.docs[i].description_short + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].description_long))
                        sbHtmlContent.Append("<strong>Description: </strong> " + results.response.docs[i].description_long + "<br/>");
                    if (results.response.docs[i].end_date != null && results.response.docs[i].end_date > DateTime.Now.AddYears(-15))
                        sbHtmlContent.Append("<strong>End Date: </strong> " + results.response.docs[i].end_date + "<br/>");
                    //IClassification
                    if (!String.IsNullOrEmpty(results.response.docs[i].district_decision))
                        sbHtmlContent.Append("<strong>District Decision: </strong> " + results.response.docs[i].district_decision + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].district))
                        sbHtmlContent.Append("<strong>District: </strong> " + results.response.docs[i].district + "<br/>");
                    if (results.response.docs[i].inspection_end_date != null && results.response.docs[i].inspection_end_date > DateTime.Now.AddYears(-15))
                        sbHtmlContent.Append("<strong>Inspection End Date: </strong> " + results.response.docs[i].inspection_end_date + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].center))
                        sbHtmlContent.Append("<strong>Center: </strong> " + results.response.docs[i].center + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].project_area))
                        sbHtmlContent.Append("<strong>Project Area: </strong> " + results.response.docs[i].project_area + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].legal_name))
                        sbHtmlContent.Append("<strong>Legal Name: </strong> " + results.response.docs[i].legal_name + "<br/>");


                    //IA Event

                    if ( results.response.docs[i].reactions!=null && results.response.docs[i].reactions.Count()>0)
                    {
                        sbHtmlContent.Append("<strong>Reactions: </strong> " + string.Join(",", results.response.docs[i].reactions) + "<br/>");
                    }
                    if (results.response.docs[i].outcomes != null && results.response.docs[i].outcomes.Count() > 0)
                    {
                        sbHtmlContent.Append("<strong>Outcomes: </strong> " + string.Join(",", results.response.docs[i].outcomes) + "<br/>");
                    }


                    if (!String.IsNullOrEmpty(results.response.docs[i].report_number))
                        sbHtmlContent.Append("<strong>Report Number: </strong> " + results.response.docs[i].report_number + "<br/>");

                    if (!String.IsNullOrEmpty(results.response.docs[i].consumer_gender))
                        sbHtmlContent.Append("<strong>Consumer Gender: </strong> " + results.response.docs[i].consumer_gender + "<br/>");
                    if (!String.IsNullOrEmpty(results.response.docs[i].consumer_age))
                        sbHtmlContent.Append("<strong>Consumer Age: </strong> " + results.response.docs[i].consumer_age + "<br/>");
                    // Content
                    if (!String.IsNullOrEmpty(results.response.docs[i].content) && results.response.docs[i].content.Length >= stringLimit)
                        sbHtmlContent.Append("<strong>Main Content: </strong> " + results.response.docs[i].content.Substring(0, stringLimit) + "<br/>");
                    // Links
                    if (!String.IsNullOrEmpty(results.response.docs[i].path))
                        sbHtmlContent.Append("<strong>Link: </strong> " + results.response.docs[i].path + "<br/>");
                    if (results.response.docs[i].date_page_last_updated != null && results.response.docs[i].date_page_last_updated > DateTime.Now.AddYears(-15))
                        sbHtmlContent.Append("<strong>Last Updated: </strong> " + results.response.docs[i].date_page_last_updated + "<br/>");
                    if (results.response.docs[i].date_published != null && results.response.docs[i].date_published > DateTime.Now.AddYears(-15))
                        sbHtmlContent.Append("<strong>Publish Date: </strong> " + results.response.docs[i].date_published + "<br/>");



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
            bool error = false;

            // Test Mode
            if (String.Compare(Constants.Mode, "production", true) != 0)
            {
                Console.WriteLine("INFO : Mode - Test" + "\n");
                JSONRootObject solrResponse = GetAlert(Constants.TestSearchQuery);
                Response mailResponse = null;


                try
                {
                    UserAlert ua = new UserAlert();
                    ua.email = Constants.TestEmail;
                    ua.name = "My Alert";
                   

                    Console.WriteLine("INFO : Attempting to send a test mail to " + Constants.TestEmail + "\n");
                    SendAlert(ua, Constants.TestSearchLink, solrResponse, mailResponse).Wait();
                    Console.WriteLine("INFO : Test Alert Sent" + "\n");
                    error = false;
                }
                catch (Exception ex)
                {
                    // log exception
                    Console.WriteLine("Error : An error occured sending an alert for Test user " + Constants.TestEmail + "\n");
                    Console.WriteLine("\nError : Reason - " + ex.Message + "\n");
                    error = true;

                }
                if (error)
                {
                    return false;
                }
                return true;

            }
            else
            {
                // perform production option
                // Run SQL to pull schedule
                // Iterate through users and send emails
                Console.WriteLine("INFO : Mode - Production" + "\n");
                var userAlerts = Helpers.GetUserAlerts();
                int userSentCount = 0;
               

                if (userAlerts != null && userAlerts.Count > 0)
                {
                    Console.WriteLine("INFO : Alerts need to be sent to  " + userAlerts.Count + " users." + "\n");
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
                                SendAlert(ua, ua.source, solrResponse, mailResponse).Wait();
                            }


                        }
                        catch (Exception ex)
                        {
                            // log exception
                            Console.WriteLine("Error : An error occured sending an alert for user " + ua.email +"\n");
                            Console.WriteLine("\nError : Reason - " + ex.Message + "\n");
                            error = true;
                        }

                        if (!error)
                        {
                            userSentCount++;
                            Helpers.UpdateUserSendDate(ua.user_id, DateTime.Now);


                        }

                    }
                }
                if (userSentCount > 0)
                {
                    Console.WriteLine("INFO : Sent " + userSentCount + " alerts." + "\n");
                    return true;
                }
                return false;
            }
             
        }
    }
}
