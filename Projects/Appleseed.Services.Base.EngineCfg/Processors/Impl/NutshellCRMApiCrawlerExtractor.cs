
namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Appleseed.Services.Base.Model;

    using Common.Logging;

    //using NReadability;

    using TikaOnDotNet.TextExtraction;
    using Appleseed.Services.Core.Models;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using System.Net;
    public class NutshellCRMApiCrawlerExtractor : IExtractThings
    {

        private readonly Common.Logging.ILog logger;
        private readonly string api_key;
        private readonly string userEmail;
        private readonly string uri;

        public NutshellCRMApiCrawlerExtractor(ILog logger, string api_key, string userEmail, string uri)
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

            this.logger = logger;
            this.api_key = api_key;
            this.userEmail = userEmail;
            this.uri = uri;
        }


        public List<AppleseedModuleItemIndex> ExtractDataForIndexing(IEnumerable<BaseItem> items) 
        {

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Data";
            var xmlFile = path + "\\ExtractedData.xml";

            if (File.Exists(xmlFile))
            {
                //string content = File.ReadAllText(xmlFile);

                byte[] byteContent = File.ReadAllBytes(xmlFile);
                string content = Encoding.UTF8.GetString(byteContent);

                var result = Core.Serialization.XmlSerialization.Deserialize<List<Model.AppleseedModuleItemIndex>>(content);

                //determine if xml has content
                if (result.Count != 0)
                {
                    this.logger.Info("Data already extracted.");
                    this.logger.Info("Result count is " + result.Count.ToString() + "-------------");
                    return result;
                }

            }

            var itemCollection = new List<Model.AppleseedModuleItemIndex>();

            // TODO: implement custom limited concurrency level task scheduler set to do 2 threads
            //Scheduler lcts = new Scheduler(1);
            List<Task> createItemTasks = new List<Task>();
            //CancellationTokenSource cts = new CancellationTokenSource();

            var i = 1;
            var count = items.Count();
            var successCount = 0;
            var failCount = 0;

            foreach (var baseItem in items)
            {
                string url = baseItem.Path;

                //createItemTasks.Add(Task.Factory.StartNew(() =>
                //        {
                this.logger.Info(this + ". Async Extract Start: " + url);

                this.logger.Info(i + " of " + count + " " + url);
                var item = this.CreateCollectionIndexItem((Model.AppleseedModuleItem)baseItem);
                if (item != null)
                {
                    if (string.IsNullOrEmpty(item.Content).Equals(false))
                    {
                        itemCollection.Add(item);
                        successCount++;
                    }
                    else
                    {
                        this.logger.Info("Failed to extract page: " + url);
                        failCount++;
                    }
                }
                else
                {
                    failCount++;
                }
                //        }, TaskCreationOptions.LongRunning).ContinueWith(t =>
                //{
                //    if (t.Exception != null)
                //    {
                //        logger.Error(t.Exception);
                //    }
                //    else
                //    {
                this.logger.Info("Async Extract End  : " + url);
                //    }
                //}));
                i++;
            }

            Task.WaitAll(createItemTasks.ToArray());

            this.logger.Info("-----------EXTRACTION SUMMARY-----------");
            this.logger.Info("Total Pages: " + count);
            this.logger.Info("Success Count: " + successCount);
            this.logger.Info("Fail Count: " + failCount);
            this.logger.Info("-----------EXTRACTION SUMMARY-----------");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var xml = Core.Serialization.XmlSerialization.Serialize(itemCollection);
            File.WriteAllText(xmlFile, xml);

            return itemCollection;
            
        }

        private Model.AppleseedModuleItemIndex CreateCollectionIndexItem(Model.AppleseedModuleItem collectedItem)
        {
            try
            {
                var item = new Model.AppleseedModuleItemIndex()
                {
                    Key = collectedItem.Key,
                    PageID = collectedItem.PageID,
                    PortalID = collectedItem.PortalID,
                    ModuleID = 1,
                    Path = collectedItem.Path,
                    Type = collectedItem.Type,
                    CreatedDate = DateTime.Today.ToString(),
                    ViewRoles = collectedItem.ViewRoles,
                    //add Name
                    Name = collectedItem.Name
                };

                this.ExtractContent(item, this.api_key, this.userEmail, this.uri);
                //item.ItemContent = done above via reference
                //item.ItemName = done above via reference;
                item.Summary = string.Empty; // used to display - TODO: run through a summarizer
                // hashcode the key
                item.Key = String.Format("{0:X}", collectedItem.Key.GetHashCode());
                return item;
            }
            catch (Exception ex)
            {
                this.logger.Error("Failed to extract file: " + collectedItem.Path);
                this.logger.Error(ex);
                return null;
            }
        }

        public void ExtractContent(Model.AppleseedModuleItemIndex indexItem, string api_key, string userEmail, string uri)
        {
            // how to set other values but extractedContent
            var extractedContent = string.Empty;
            var contentTitle = string.Empty;
            var contentFileExtension = string.Empty;
            var contentFileName = string.Empty;

            switch (indexItem.Type)
            {
                case CollectionItemType.NS_ACC:
                    extractedContent = NutshellGetAccount(api_key, userEmail, uri, indexItem.Key);
                    break;
                case CollectionItemType.NS_CON:
                    extractedContent = NutshellGetContact(api_key, userEmail, uri, indexItem.Key);
                    break;
                default:
                    break;
            
            }

            
            indexItem.Content = extractedContent;
        
        }



        public string NutshellGetAccount(string api_key, string userEmail, string uri, string AccountID)
        {
            JObject accounts = new JObject();

            accounts["method"] = "getAccount";

            JObject accountsFields = new JObject();

            accountsFields["accountId"] = AccountID;
            accountsFields["rev"] = null;

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


        public string NutshellGetContact(string api_key, string userEmail, string uri, string AccountID)
        {
            JObject accounts = new JObject();

            accounts["method"] = "getContact";

            JObject accountsFields = new JObject();

            accountsFields["contactId"] = AccountID;
            accountsFields["rev"] = null;

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
    }
}
