using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Appleseed.Base.Data.Queue;
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

    using Dropbox.Api;
    using System.Net.Http;
    using Dropbox.Api.Files;
    using System.Threading;
    using System.Threading.Tasks;
    //using DropNet;
    //using DropNet.Authenticators;
    //using DropNet.Models;
    //using DropNet.Extensions;
    //using DropNet.Helpers;
    //using DropNet.Exceptions;

    public class DropBoxCloudFileAggregatorNeo : IAggregateData
    {
        private readonly ILog logger;
        private readonly string App_key;
        private readonly string App_secret;
        private readonly string filesPath;
        private readonly string token;
        private readonly string tokenSecret;
        private readonly string connectionString;
        private readonly string _queueType;

        private readonly string userAgent;

        private DropboxClient client;

        public DropBoxCloudFileAggregatorNeo(ILog logger, string App_key, string userAgent, string token, string tokenSecret, string filesPath, string connectionString, string queueType)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (App_key == null)
            {
                throw new ArgumentNullException("App_key");
            }

            //if( App_secret == null)
            //{

            //    throw new ArgumentNullException("App_secret");
            //}

            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            //if (tokenSecret == null)
            //{
            //    throw new ArgumentNullException("tokenSecret");
            //}

            //if (tableName == null)
            //{
            //    throw new ArgumentNullException("tableName");
            //}

            if (filesPath == null)
            {
                throw new ArgumentNullException("filesPath");
            }

            //if (connectionString == null)
            //{
            //    throw new ArgumentNullException("connectionString");
            //}

            //if (tableName == null)
            //{
            //    throw new ArgumentNullException("tableName");
            //}

            if (userAgent == null)
            {
                throw new ArgumentNullException("userAgent");
            }

            this.logger = logger;
            this.App_key = App_key;
            //this.App_secret = App_secret;
            //this.tokenSecret = tokenSecret;
            this.filesPath = filesPath;
            this.token = token;
            this.connectionString = connectionString;
            this._queueType = queueType;
            this.userAgent = userAgent;
            this.logger.Info("----------------CONFIGURING DROPBOXCLOUDFILE--------------------------------");
        }

        public void Aggregate()
        {
            this.logger.InfoFormat("----------------STARTING DROPBOXCLOUDFILE.AGGREGATE--------------------------- {0}", 1);

            InitializeCertPinning();

            //this.logger.InfoFormat("App_key:{0} App_secret:{1} token:{2} tokenSecret:{3}", App_key, App_secret, token, tokenSecret);

            // check if there is the path for downloading files
            if (!Directory.Exists(filesPath))
            {
                Directory.CreateDirectory(filesPath);
            }

            // Specify socket level timeout which decides maximum waiting time when on bytes are
            // received by the socket.
            var httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
            {
                // Specify request level timeout which decides maximum time taht can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            };

            //this.client = new DropboxClient(token, userAgent: userAgent, httpClient: httpClient);
            this.client = new DropboxClient(token, userAgent: userAgent);
            //_client.UserLogin = new UserLogin { Token = token, Secret = tokenSecret };

            //try
            //{
            //    //await GetCurrentAccount();

            var path = "/DotNetApi/Help";
            //var path = "";
            var list = ListFolder(path).Result;



            List<CollectionIndexItem> ls = new List<CollectionIndexItem>();


            AggregateItems(list, path, ls);
        }

        private void AggregateItems(ListFolderResult list, string path, List<CollectionIndexItem> ls)
        {
            foreach (var mData in list.Entries)
            {
                if (mData.IsFile)
                {
                    var itemName = mData.Name;
                    var filesPath1 = filesPath.Replace(@"\", "/");

                    //var itemPath = filesPath1 + jo.CollectionItemFilePath;
                    var itemPath = filesPath1 + mData.PathLower;
                    var itemKey = "Dropbox/" + mData.PathLower.ToString();
                    //var moduleId = 

                    //using (var response = this.client.Files.DownloadAsync(path + "/" + itemName))
                    using (var response = this.client.Files.DownloadAsync(mData.PathLower))
                    {
                        //Download the file
                        var fileBytes = response.Result.GetContentAsByteArrayAsync();



                        if (!Directory.Exists(itemPath))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(itemPath));
                        }


                        using (FileStream fs = new FileStream(itemPath, FileMode.Create))
                        {
                            for (int i = 0; i < fileBytes.Result.Length; i++)
                            {
                                fs.WriteByte(fileBytes.Result[i]);
                            }
                            fs.Seek(0, SeekOrigin.Begin);
                            for (int i = 0; i < fileBytes.Result.Length; i++)
                            {
                                if (fileBytes.Result[i] != fs.ReadByte())
                                {
                                    logger.InfoFormat("DROPBOX.CLOUD.AGGREGATE.FILE - Error Writing Data for {}", itemPath);

                                    break;
                                }
                            }
                        }
                    }

                    var newItem = new AppleseedModuleItem()
                    {
                        Key = itemKey, //TODO: use a unique ID 
                        Path = itemPath,
                        ModuleID = 0,
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
                    this.logger.InfoFormat("DROPBOX.CLOUD.AGGREGATE.FILE Added {0} into Cache Collection", itemName);
                }
                else if (mData.IsFolder)
                {
                    var l = ListFolder(mData.PathLower).Result;
                    AggregateItems(l, path, new List<CollectionIndexItem>());

                }


                //}
                //catch (HttpException e)
                //{
                //    Console.WriteLine("Exception reported from RPC layer");
                //    Console.WriteLine("    Status code: {0}", e.StatusCode);
                //    Console.WriteLine("    Message    : {0}", e.Message);
                //    if (e.RequestUri != null)
                //    {
                //        Console.WriteLine("    Request uri: {0}", e.RequestUri);
                //    }
                //}


                //var m = _client.GetMetaData("/", false, false);


                //List<CollectionIndexItem> ls = itemsInFolder(m, _client);


                //foreach (CollectionIndexItem jo in ls)
                //{

                //    //string itemId = jo.CollectionItemID;
                //    int itemId = jo.ItemModuleId;
                //    //var itemName = jo.CollectionItemName;
                //    var itemName = jo.ItemName;
                //    var itemKey = "Dropbox/" + jo.ItemModuleId.ToString();


                //    var filesPath1 = filesPath.Replace(@"\", "/");

                //    //var itemPath = filesPath1 + jo.CollectionItemFilePath;
                //    var itemPath = filesPath1 + jo.ItemPath;

                //    //TODO: Make a new folder for each folder in Dropbox. Right now all files are in one directory
                //    //var itemPath = filesPath + "\\" + jo.CollectionItemName;

                //    var itemType = "";

                //    //Download the file
                //    var fileBytes = _client.GetFile(jo.ItemPath);
                //    using (FileStream fs = new FileStream(itemPath, FileMode.Create))
                //    {
                //        for (int i = 0; i < fileBytes.Length; i++)
                //        {
                //            fs.WriteByte(fileBytes[i]);
                //        }
                //        fs.Seek(0, SeekOrigin.Begin);
                //        for (int i = 0; i < fileBytes.Length; i++)
                //        {
                //            if (fileBytes[i] != fs.ReadByte())
                //            {
                //                logger.InfoFormat("DROPBOX.CLOUD.AGGREGATE.FILE - Error Writing Data for {}", itemPath);

                //                break;
                //            }
                //        }
                //    }


                /* DateTime publishDate;
                 try
                 {
                     publishDate = !string.IsNullOrEmpty(jo.ItemCreatedDate) ? Convert.ToDateTime(jo.ItemCreatedDate) : DateTime.Now;
                 }
                 catch (Exception)
                 {
                     publishDate = DateTime.Now;
                 }*/
            }
        }

        //public List<CollectionIndexItem> itemsInFolder(MetaData m, DropNetClient _client)
        //{

        //    List<MetaData> li = m.Contents;

        //    //List<CollectionItem> ls = new List<CollectionItem>();
        //    List<CollectionIndexItem> ls = new List<CollectionIndexItem>();

        //    foreach (MetaData mo in li)
        //    {
        //        if (mo.Is_Dir)
        //        {
        //            var filesPath1 = filesPath.Replace(@"\", "/");

        //            //this.logger.Info("The folder that should be created is: " + filesPath1 + mo.Path);

        //            var newFolderPath = filesPath1 + mo.Path;

        //            if (!Directory.Exists(newFolderPath))
        //            {
        //                Directory.CreateDirectory(newFolderPath);
        //            }


        //            var mot = _client.GetMetaData(mo.Path, false, false );

        //            ls.AddRange(itemsInFolder(mot, _client));

        //        }

        //        else
        //        {

        //           // CollectionItem c = new CollectionItem();
        //            CollectionIndexItem item = new CollectionIndexItem();

        //            item.ItemModuleId = mo.Rev.GetHashCode();
        //            item.ItemName = mo.Name;
        //            item.ItemPath = mo.Path;
        //            item.ItemCreatedDate = mo.ModifiedDate.ToLongDateString();
        //            item.ItemKey = mo.Path;

        //            //c.CollectionItemID = mo.Rev;
        //            //c.CollectionItemName = mo.Name;
        //            //c.CollectionItemModifiedDate = mo.ModifiedDate.ToLongDateString();
        //            //c.CollectionItemDescription = "Description";
        //            //c.CollectionItemFilePath = mo.Path;
        //            //c.CollectionItemTitle = "Title";
        //            //c.CollectionItemSummary = "Summary";
        //            //c.CollectionItemContents = "";
        //            //c.CollectionItemMeta = "Metadata";
        //            //c.CollectionItemViewRoles = "";
        //            //c.CollectionItemSource = mo.Path;
        //            //c.CollectionItemKey = mo.Path;

        //            ls.Add(item);
        //        }

        //    }

        //    return ls;
        //}


        /// <summary>
        /// Lists the items within a folder.
        /// </summary>
        /// <remarks>This is a demonstrates calling an rpc style api in the Files namespace.</remarks>
        /// <param name="path">The path to list.</param>
        /// <returns>The result from the ListFolderAsync call.</returns>
        private Task<ListFolderResult> ListFolder(string path)
        {
            //Console.WriteLine("--- Files ---");
            var list = this.client.Files.ListFolderAsync(path);

            // show folders then files
            //foreach (var item in list.Entries.Where(i => i.IsFolder))
            //{
            //    Console.WriteLine("D  {0}/", item.Name);
            //}

            //foreach (var item in list.Entries.Where(i => i.IsFile))
            //{
            //    var file = item.AsFile;

            //    Console.WriteLine("F{0,8} {1}",
            //        file.Size,
            //        item.Name);
            //}

            //if (list.HasMore)
            //{
            //    Console.WriteLine("   ...");
            //}
            return list;
        }

        //public void AggregateItem(AppleseedModuleItem item)
        //{
        //    Appleseed.Services.Core.Helpers.CollectionHelper.AggregateItem(item, this.connectionString, this.tableName);
        //}

        public void AggregateItem(AppleseedModuleItem item)
        {
            /*var queueSection = ConfigurationManager.GetSection("queues") as QueueSection;

            if (queueSection != null)
            {
                foreach (var queue in queueSection.Queue)
                {*/
                    //var queue = queueSection.Queue.SingleOrDefault(x => String.Equals(x.Name, singleQueueName, StringComparison.CurrentCultureIgnoreCase));

                    var repository = RepositoryService.GetRepository(this.connectionString, this._queueType);

                    var itemAggregateQueue = new BaseCollectionItemQueue(repository, this.logger, AggregationTypeEnum.Dropbox.ToString(), "Aggregate");

                    Appleseed.Base.Data.Model.BaseCollectionItem baseCollectionItem = new Appleseed.Base.Data.Model.BaseCollectionItem();

                    baseCollectionItem.Data.PortalID = item.PortalID;
                    baseCollectionItem.Data.Key = item.Key;
                    baseCollectionItem.Data.Type = item.Type;
                    baseCollectionItem.Data.Path = item.Path;
                    baseCollectionItem.Data.Name = item.Name;
                    baseCollectionItem.Data.ItemCreatedDate = DateTime.Now;
                    baseCollectionItem.Data.ViewRoles = item.ViewRoles;
                    baseCollectionItem.Data.FileSize = item.FileSize;

                    //baseCollectionItem.da

                    itemAggregateQueue.Enqueue(baseCollectionItem);

                    //ProcessQueueCycle(repository, log);
            //    }
            //}

            //Appleseed.Services.Core.Helpers.CollectionHelper.AggregateItem(item, this.connectionString, this.tableName);
        }

        /// <summary>
        /// Initializes ssl certificate pinning.
        /// </summary>
        private void InitializeCertPinning()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                var root = chain.ChainElements[chain.ChainElements.Count - 1];
                var publicKey = root.Certificate.GetPublicKeyString();

                return DropboxCertHelper.IsKnownRootCertPublicKey(publicKey);
            };
        }



    }



}
