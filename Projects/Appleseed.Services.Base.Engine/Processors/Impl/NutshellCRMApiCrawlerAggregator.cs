
namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using Abot.Crawler;
    using Abot.Poco;
    using System.Net;
    using Common.Logging;

    using Appleseed.Services.Core.Helpers;
    using Appleseed.Services.Base.Model;
    using Newtonsoft.Json.Linq;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class NutshellCRMApiCrawlerAggregator : IAggregateData
    {
        private readonly Common.Logging.ILog logger;
        private readonly string api_key;
        private readonly string userEmail;
        private readonly string uri;
        private readonly string connectionString;
        private readonly string tableName;

        public NutshellCRMApiCrawlerAggregator(ILog logger, string api_key, string userEmail, string uri, string connectionString, string tableName)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            if (api_key == null)
            {
                throw new ArgumentNullException("api_key");
            }
            if (userEmail == null)
            {
                throw new ArgumentNullException("userEmail");
            }
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            
            this.logger = logger;
            this.api_key = api_key;
            this.userEmail = userEmail;
            this.uri = uri;
            this.connectionString = connectionString;
            this.tableName = tableName;
            this.logger.Info("----------------CONFIGURING NUTSHELLCRMAPICRAWLER--------------------------------");
        }
        public void Aggregate()
        {
            this.logger.InfoFormat("----------------STARTING NUTSHELLCRMAPICRAWLER.AGGREGATE--------------------------- {0}", 1);
            
            //Get Json Big String 
            string contacts = NutshellAggregateContacts(api_key, userEmail, uri);          
            string accounts = NutshellAggregateAccounts(api_key, userEmail, uri);

            // Change big String into JObject List
            List<JObject> contactsList = NutshellCollectContact(contacts);
            List<JObject> accountsList = NutshellCollectContact(accounts);
            
            // Change elements in list to Object, Object models need define?
            foreach (var contact in contactsList) {

               
                var itemId = contact["id"];
                var itemName = contact["name"];
                var itemKey = "contact/" + contact["id"].ToString();
                var itemPath = contact["htmlUrl"];
                var itemType = contact["entityType"];

                 // TODO: Insert into Database

                var newItem = new AppleseedModuleItem()
                {
                    Key = itemKey, //DONE: use a unique ID that Nutshell is giving
                    Path = itemPath.ToString(),
                    ModuleID = Int32.Parse(itemId.ToString()),
                    PageID = 0,
                    PortalID = 0,
                    ViewRoles = CollectionItemSecurityType.PUBLIC,
                    Type = CollectionItemType.NS_CON,
                    FileSize = 0,
                    Name = itemName.ToString()
                };


                //TODO: add this into the DB using AggregateItem() 

                this.AggregateItem(newItem);
                this.logger.InfoFormat("NUTSHELL.API.AGGREGATE.CONTACT Added {0} into Cache Collection", contact.ToString());

            }

            foreach (var account in accountsList)
            {
                logger.InfoFormat("NUTSHELL.API.AGGREGATE.ACCOUNT {0}", account.ToString());
                var itemId = account["id"];
                var itemName = account["name"];
                var itemKey = "account/" + account["id"].ToString();
                var itemPath = account["htmlUrl"];
                var itemType = account["entityType"];

                // TODO: Insert into Database

                var newItem = new AppleseedModuleItem()
                {
                    Key = itemKey, //DONE: use a unique ID that Nutshell is giving
                    Path = itemPath.ToString(),
                    ModuleID = Int32.Parse(itemId.ToString()),
                    PageID = 0,
                    PortalID = 0,
                    ViewRoles = CollectionItemSecurityType.PUBLIC,
                    Type = CollectionItemType.NS_ACC,
                    FileSize = 0,
                    Name = itemName.ToString()
                };

                this.AggregateItem(newItem);
                this.logger.InfoFormat("NUTSHELL.API.AGGREGATE.ACCOUNT Added {0} into Cache Collection", account.ToString());
            }

           
            
            
            this.logger.Info("----------------ENDING NUTSHELLCRMAPICRAWLER.AGGREGATE--------------------------------");     
        }

        public void AggregateItem(AppleseedModuleItem item)
        {
            //refactored to core helpers because it will be used everywhere 
            Appleseed.Services.Core.Helpers.CollectionHelper.AggregateItem(item, this.connectionString, this.tableName);
        }


        // Aggregate Contacts
        public string NutshellAggregateContacts(string api_key, string userEmail, string uri)
        {
            JObject accounts = new JObject();
            accounts["method"] = "findContacts";
            JObject accountsFields = new JObject();
            accountsFields["query"] = new JObject();
            accountsFields["orderBy"] = "id";
            accountsFields["orderDirection"] = "ASC";
            accountsFields["limit"] = 50;
            accountsFields["page"] = 1;
            accountsFields["stubResponses"] = false;
            accounts["params"] = accountsFields;
            accounts["id"] = "apeye";
            string json = accounts.ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(userEmail + ":" + api_key));
            httpWebRequest.Headers.Add(api_key, userEmail);
            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());

            return (streamReader.ReadToEnd());
        }

        // Aggregate Accounts
        public string NutshellAggregateAccounts(string api_key, string userEmail, string uri)
        {
            JObject accounts = new JObject();
            accounts["method"] = "findAccounts";
            JObject accountsFields = new JObject();
            accountsFields["query"] = new JObject();
            accountsFields["orderBy"] = "name";
            accountsFields["orderDirection"] = "ASC";
            accountsFields["limit"] = 50;
            accountsFields["page"] = 1;
            accountsFields["stubResponses"] = false;
            accounts["params"] = accountsFields;
            accounts["id"] = "apeye";
            string json = accounts.ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(userEmail + ":" + api_key));
            httpWebRequest.Headers.Add(api_key, userEmail);
            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            return (streamReader.ReadToEnd());
        }

        public List<JObject> NutshellCollectAccount(string AccountString)
        {
            List<JObject> returnList = new List<JObject>();
            //string AccountString = NutshellAggregateAccounts(api_key, userEmail, uri);
            JObject Accounts = JObject.Parse(AccountString);
            foreach (JObject account in Accounts["result"])
                returnList.Add(account);
            return returnList;
        }


        public List<JObject> NutshellCollectContact(string ContactString)
        {
            List<JObject> returnList = new List<JObject>();
            //string ContactString = NutshellAggregateContacts(api_key, userEmail, uri);
            JObject Contacts = JObject.Parse(ContactString);
            foreach (JObject contact in Contacts["result"])
                returnList.Add(contact);
            return returnList;
        }
    }
}
