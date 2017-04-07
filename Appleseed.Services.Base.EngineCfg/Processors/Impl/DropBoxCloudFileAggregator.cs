using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Appleseed.Base.Data.Service;
using Appleseed.Base.Data.Utility;

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

    using DropNet;
    using DropNet.Authenticators;
    using DropNet.Models;
    using DropNet.Extensions;
    using DropNet.Helpers;
    using DropNet.Exceptions;

    public class DropBoxCloudFileAggregator : IAggregateData
    {
        private readonly Common.Logging.ILog logger;
        private readonly string App_key;
        private readonly string App_secret;
        private readonly string filesPath;
        private readonly string token;
        private readonly string tokenSecret;
        private readonly string connectionString;
        private readonly string tableName;

        public DropBoxCloudFileAggregator(ILog logger, string App_key, string App_secret, string token, string tokenSecret, string filesPath, string connectionString, string tableName)
		    {
	            if (logger == null)
	            {
	                throw new ArgumentNullException("logger");
	            }

	            if (App_key == null)
	            {
	                throw new ArgumentNullException("App_key");
	            }

	            if( App_secret == null)
	            {

                    throw new ArgumentNullException("App_secret");
	            }

	            if (token == null)
	            {
	                throw new ArgumentNullException("token");
	            }

	            if (tokenSecret == null)
	            {
	                throw new ArgumentNullException("tokenSecret");
	            }

                if (tableName == null)
                {
                    throw new ArgumentNullException("tableName");
                }

                if (filesPath == null)
                {
                    throw new ArgumentNullException("filesPath");
                }

	            if (connectionString == null)
	            {
	                throw new ArgumentNullException("connectionString");
	            }

                if (tableName == null)
                {
                    throw new ArgumentNullException("tableName");
                }
	            
	            this.logger = logger;
	            this.App_key = App_key;
	            this.App_secret = App_secret;
	            this.tokenSecret = tokenSecret;
                this.filesPath = filesPath;
	            this.token = token;
	            this.connectionString = connectionString;
	            this.tableName = tableName;
	            this.logger.Info("----------------CONFIGURING DROPBOXCLOUDFILE--------------------------------");
	        }

        public void Aggregate()
        {
            this.logger.InfoFormat("----------------STARTING DROPBOXCLOUDFILE.AGGREGATE--------------------------- {0}", 1);

            //this.logger.InfoFormat("App_key:{0} App_secret:{1} token:{2} tokenSecret:{3}", App_key, App_secret, token, tokenSecret);
            
            // check if there is the path for downloading files
            if (!Directory.Exists(filesPath))
            {
                Directory.CreateDirectory(filesPath);
            }

            var _client = new DropNetClient(App_key, App_secret);

            _client.UserLogin = new UserLogin { Token = token, Secret = tokenSecret };

            var m = _client.GetMetaData("/", false, false);


            List<CollectionIndexItem> ls = itemsInFolder(m, _client);


            foreach (CollectionIndexItem jo in ls)
            {

                //string itemId = jo.CollectionItemID;
                int itemId = jo.ItemModuleId;
                //var itemName = jo.CollectionItemName;
                var itemName = jo.ItemName;
                var itemKey = "Dropbox/" + jo.ItemModuleId.ToString();

                

                var filesPath1 = filesPath.Replace(@"\", "/");
                    
                //var itemPath = filesPath1 + jo.CollectionItemFilePath;
                var itemPath = filesPath1 + jo.ItemPath;

                //TODO: Make a new folder for each folder in Dropbox. Right now all files are in one directory
                //var itemPath = filesPath + "\\" + jo.CollectionItemName;

                var itemType = "";

                //Download the file
                var fileBytes = _client.GetFile(jo.ItemPath);
                using (FileStream fs = new FileStream(itemPath, FileMode.Create))
                {
                    for (int i = 0; i < fileBytes.Length; i++)
                    {
                        fs.WriteByte(fileBytes[i]);
                    }
                    fs.Seek(0, SeekOrigin.Begin);
                    for (int i = 0; i < fileBytes.Length; i++)
                    {
                        if (fileBytes[i] != fs.ReadByte())
                        {
                            logger.InfoFormat("DROPBOX.CLOUD.AGGREGATE.FILE - Error Writing Data for {}", itemPath);

                            break;
                        }
                    }
                }


               /* DateTime publishDate;
                try
                {
                    publishDate = !string.IsNullOrEmpty(jo.ItemCreatedDate) ? Convert.ToDateTime(jo.ItemCreatedDate) : DateTime.Now;
                }
                catch (Exception)
                {
                    publishDate = DateTime.Now;
                }*/

                var newItem = new AppleseedModuleItem()
                {
                    Key = itemKey, //TODO: use a unique ID 
                    Path = itemPath,
                    ModuleID = itemId,
                    PageID = 0,
                    PortalID = 0,
                    ViewRoles = CollectionItemSecurityType.PUBLIC,
                    Type = CollectionItemType.FILE,
                    FileSize = 0,
                    //TODO PublishedDate = publishDate,
                    Name = itemName
                };


                //TODO: add this into the DB using AggregateItem()

                this.AggregateItem(newItem);
                this.logger.InfoFormat("DROPBOX.CLOUD.AGGREGATE.FILE Added {0} into Cache Collection", jo.ItemName.ToString());



            }
        }

        public List<CollectionIndexItem> itemsInFolder(MetaData m, DropNetClient _client)
        {

            List<MetaData> li = m.Contents;

            //List<CollectionItem> ls = new List<CollectionItem>();
            List<CollectionIndexItem> ls = new List<CollectionIndexItem>();

            foreach (MetaData mo in li)
            {
                if (mo.Is_Dir)
                {
                    var filesPath1 = filesPath.Replace(@"\", "/");

                    //this.logger.Info("The folder that should be created is: " + filesPath1 + mo.Path);

                    var newFolderPath = filesPath1 + mo.Path;

                    if (!Directory.Exists(newFolderPath))
                    {
                        Directory.CreateDirectory(newFolderPath);
                    }


                    var mot = _client.GetMetaData(mo.Path, false, false );

                    ls.AddRange(itemsInFolder(mot, _client));

                }

                else
                {

                   // CollectionItem c = new CollectionItem();
                    CollectionIndexItem item = new CollectionIndexItem();

                    item.ItemModuleId = mo.Rev.GetHashCode();
                    item.ItemName = mo.Name;
                    item.ItemPath = mo.Path;
                    item.ItemCreatedDate = mo.ModifiedDate.ToLongDateString();
                    item.ItemKey = mo.Path;

                    //c.CollectionItemID = mo.Rev;
                    //c.CollectionItemName = mo.Name;
                    //c.CollectionItemModifiedDate = mo.ModifiedDate.ToLongDateString();
                    //c.CollectionItemDescription = "Description";
                    //c.CollectionItemFilePath = mo.Path;
                    //c.CollectionItemTitle = "Title";
                    //c.CollectionItemSummary = "Summary";
                    //c.CollectionItemContents = "";
                    //c.CollectionItemMeta = "Metadata";
                    //c.CollectionItemViewRoles = "";
                    //c.CollectionItemSource = mo.Path;
                    //c.CollectionItemKey = mo.Path;

                    ls.Add(item);
                }

            }

            return ls;
        }

        //public void AggregateItem(AppleseedModuleItem item)
        //{
        //    Appleseed.Services.Core.Helpers.CollectionHelper.AggregateItem(item, this.connectionString, this.tableName);
        //}
        public void AggregateItem(AppleseedModuleItem item)
        {
            var queueSection = ConfigurationManager.GetSection("queue") as QueueSection;
            const string singleQueueName = "SqlServerQueue";

            if (queueSection != null)
            {
                var queue = queueSection.Queue.SingleOrDefault(x => String.Equals(x.Name, singleQueueName, StringComparison.CurrentCultureIgnoreCase));
                if (queue != null)
                {
                    var repository = RepositoryService.GetRepository(queue.ConnectionString, queue.QueueName);
                    //ProcessQueueCycle(repository, log);
                }
            }

            //Appleseed.Services.Core.Helpers.CollectionHelper.AggregateItem(item, this.connectionString, this.tableName);
        }


      
    }

 
  
}
