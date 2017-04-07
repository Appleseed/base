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

    using TikaOnDotNet;
    using Appleseed.Services.Core.Models;
    using System.Text;

    /// <summary>
    /// Extracts content from an HTML document
    /// </summary>
    public class WebContentExtractor : IExtractThings
    {
        // 2. Get data from the interwebs using HTML Agility Pack, NReadability, TikaDotNet and populate item collection

        private readonly Common.Logging.ILog logger;

        public WebContentExtractor(ILog logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
        }


        //// TODO: create / refactor/ override ExtractDataForIndexing that takes items , gets data, and puts them in to a queue
        // TODO : uses the helper method Queue_Transmit_ItemIndex 

        /// <summary>
        /// Get content from a URL and scrub it to remove excess HTML/CSS/JS
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<Model.AppleseedModuleItemIndex> ExtractDataForIndexing(IEnumerable<BaseItem> items)
        {
            //TODO revamp how we deal with previously extracted data / synching issues
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Data";
            var xmlFile = path + "\\ExtractedData.xml";
            if (File.Exists(xmlFile))
            {
                //string content = File.ReadAllText(xmlFile);

                byte[] byteContent =  File.ReadAllBytes(xmlFile);
                string content = Encoding.UTF8.GetString(byteContent);

                var result = Core.Serialization.XmlSerialization.Deserialize<List<Model.AppleseedModuleItemIndex>>(content);
                
                //determine if xml has content
                if (result.Count != 0)
                {
                    this.logger.Info("Data already extracted.");
                    this.logger.Info("result count is " + result.Count.ToString() + "-------------");
                    return result;
                }    
                
            }


            // build list of CollectionIndexItems with 
            // indexpath 
            // title
            // content async
            // summary 

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

            /*Task.WaitAll(createItemTasks.ToArray());*/

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
                    CreatedDate = collectedItem.PublishedDate == null ? DateTime.Today.ToString() : collectedItem.PublishedDate.ToString(),
                    ViewRoles = collectedItem.ViewRoles
                };

                this.ExtractContent(item);
                //item.ItemContent = done above via reference
                //item.ItemName = done above via reference;
                item.Summary = string.Empty; // used to display - TODO: run through a summarizer

                return item;
            }
            catch (Exception ex)
            {
                this.logger.Error("Failed to extract file: " + collectedItem.Path);
                this.logger.Error(ex);
                return null;
            }
        }

        public void ExtractContent(Model.AppleseedModuleItemIndex indexItem)
        {
            var extractedContent = string.Empty;
            var contentTitle = string.Empty;
            var contentFileExtension = string.Empty;
            var contentFileName = string.Empty;

            switch (indexItem.Type)
            {
                case CollectionItemType.PAGE:
                    extractedContent = this.ExtractContentPAGE(indexItem.Path, out contentTitle);
                    break;

                case CollectionItemType.FILE:
                    // download, run TikaDotNet, return extracted info
                    //TODO determine if remote or local 

                    //if remote 
                    if (IsFileSourceRemote(indexItem.Path))
                    {
                        var contentUri = new Uri(indexItem.Path);
                        var contentFileInfo = new FileInfo(contentUri.AbsolutePath);
                        contentFileExtension = contentFileInfo.Extension;
                        contentFileName = contentFileInfo.Name;
                        indexItem.FileSize = contentFileInfo.Length;
                    }
                    else
                    {
                        //if not remote double check to see if it's there
                        if (File.Exists(indexItem.Path))
                        {
                            var contentFileInfo = new FileInfo(indexItem.Path);
                            contentFileExtension = contentFileInfo.Extension;
                            contentFileName = contentFileInfo.Name;
                            indexItem.FileSize = contentFileInfo.Length;
                        }
                        else
                        {
                            this.logger.Info("FILE NOT FOUND: " + indexItem.Path);
                            return;
                        }
                    }

                    this.logger.Debug("contentFileExtension: " + contentFileExtension);

                    switch (contentFileExtension.ToLower())
                    {
                        //exclude images
                        case ".png":
                        case ".jpg":
                            break;

                        //include these file formats
                        case ".pdf":
                        case ".doc":
                        case ".docx":
                        case ".ppt":
                        case ".pptx":
                        case ".xls":
                        case ".xlsx":
                        case ".osd":
                        case ".wpd":
                        case ".txt":
                        case ".rtf":
                        case ".html":
                        case ".htm":
                            this.logger.Debug("indexItem.Path: " + indexItem.Path);
                            extractedContent = this.ExtractContentFILE(indexItem.Path, out contentTitle);
                            break;

                        default:
                            // For random files for NOVA - needs to be special case.. but this is the only way to handle for now.
                            if (contentFileExtension.Length == 4)
                            {
                                string extensionCharacters = contentFileExtension.Substring(1, 3);
                                if (int.Parse(extensionCharacters) > 0)
                                {
                                    extractedContent = this.ExtractContentFILE(indexItem.Path, out contentTitle);
                                    break;
                                }
                            }

                            extractedContent = this.ExtractContentFILE(indexItem.Path, out contentTitle);
                            break;
                    } 

                    break;

                case CollectionItemType.MODULE:
                    ExtractContentMODULE(indexItem);
                    break;

                default:
                    break;
            }

            indexItem.Name = contentTitle == "None" ? contentFileName : contentTitle;

            indexItem.Content = extractedContent;
        }

        private static bool IsFileSourceRemote(string contentUrl)
        {
            return contentUrl.Contains("http:");
        }

        public string ExtractContentFILE(string contentUrl, out string contentTitle)
        {
            if (string.IsNullOrEmpty(contentUrl))
            {
                throw new ArgumentNullException("contentUrl");
            }

            string localFile;
            var summary = string.Empty;
            var title = string.Empty;

            if (IsFileSourceRemote(contentUrl))
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["local_PathToFilesDownload"]))
                {
                    throw new Exception("'local_PathToFilesDownload' was not found in the app.config file");
                }

                var contentUri = new Uri(contentUrl);
                var contentFileInfo = new FileInfo(contentUri.AbsolutePath);
                var contentFileExtension = contentFileInfo.Extension;
                localFile = ConfigurationManager.AppSettings["local_PathToFilesDownload"] + "/file_" + contentUrl.GetHashCode() + contentFileExtension;

                // tika will work on files not URLS
                //1. Download Document to local file downloads  if its not already downloaded

                if (!File.Exists(localFile))
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        this.logger.Info("Downloading :" + contentUri.ToString());
                        //var client = new CookieAwareWebClient();
                        var client = new System.Net.WebClient();
                        client.DownloadFile(contentUri, localFile);
                    }).ContinueWith(t =>
                    {
                        this.ExtractContentFileTika(ref summary, ref title, localFile);
                    });
                }
                else
                {
                    this.ExtractContentFileTika(ref summary, ref title, localFile);
                }
            }
            else
            {
                this.logger.Debug("START TIKA: " + contentUrl);
                localFile = contentUrl;
                if (File.Exists(localFile))
                {
                    this.ExtractContentFileTika(ref summary, ref title, localFile);
                }
            }

            contentTitle = title ?? "None";

            //3. Return data 
            return summary;
        }

        private void ExtractContentFileTika(ref string summary, ref string title, string localFile)
        {
            var extractor = new TextExtractor();
            try
            {
                var extractionResults = extractor.Extract(localFile);
                extractionResults.Metadata.TryGetValue("title", out title);
                summary = extractionResults.Text;
            }
            catch (Exception e)
            {
                this.logger.Error(e);
            }
        }

        private void ExtractContentFilesDownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            this.logger.Info("Downloaded :" + args.UserState.ToString());
        }


        // Use tika to get HTML content extracted as text. 
        public string ExtractContentPAGE(string contentURL, out string contentTitle)
        {
            if (string.IsNullOrEmpty(contentURL))
            {
                throw new ArgumentNullException("contentURL", "No valid URL provided to Web Content Extrator service.");
            }

            var extractor = new TextExtractor();
            try
            {
                var extractionResults = extractor.Extract(new Uri(contentURL));
                extractionResults.Metadata.TryGetValue("title", out contentTitle);
                var summary = string.Empty;

                if (extractionResults != null)
                {
                    extractionResults.Metadata.TryGetValue("title", out contentTitle);
                    summary = extractionResults.Text;
                    return summary;
                }

                
            }
            catch (Exception e)
            {
                this.logger.Error(e);

                this.logger.Info("Extracting (2nd Attempt) - " + contentURL);
                // If Tika Fails, revert back to secondary method. 
                return ExtractContentPAGEREAD(contentURL, out contentTitle);
            }


            contentTitle = string.Empty;
            return string.Empty;
        }

        // Use ScrapySharp+HtmlAgility and Nreadability to get HTML content that's readable. 
        public string ExtractContentPAGEREAD(string contentURL, out string contentTitle)
        {
            if (string.IsNullOrEmpty(contentURL))
            {
                throw new ArgumentNullException("contentURL", "No valid URL provided to Web Content Extrator service.");
            }

            try
            {
                var extractor = new Core.Extractors.Impl.HtmlContentExtractor();
                var extractedPageContent = extractor.Extract(contentURL);
                var summary = string.Empty;
                
                if (extractedPageContent != null)
                {
                    //if(!string.IsNullOrEmpty(extractedPageContent.Title ))
                    contentTitle = extractedPageContent.Title;                  
                    summary = extractedPageContent.Content;
                    return summary;
                }
            }
            catch (Exception e)
            {
                this.logger.Error(e);
            }

            contentTitle = string.Empty;
            return string.Empty;
        }

        public static void ExtractContentMODULE(Model.AppleseedModuleItemIndex indexItem)
        {
            //TODO: 
            // 1. connect to correct DB 
            // use connection string from app.config 

            // 2. determine portal exists
            // run query to see if portal exists with indexItem.ItemPortalID

            // 3. determine module exists
            // run query to see if module exists with indexItem.ItemModuleID

            // 4. determine module item exists
            // run query to see if module item exists with indexItem.ItemModuleID + moduleRecord

            // 5. determine module item type
            // using ItemModuleID and moduleRecord 

            // 6. determine module item title
            indexItem.Name = "Module Title";

            // 7. determine module item content
            indexItem.Content = "Module Content ";
        }
    }
}

