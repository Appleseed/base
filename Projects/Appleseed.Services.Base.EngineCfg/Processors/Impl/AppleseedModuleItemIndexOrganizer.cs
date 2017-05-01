namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using Appleseed.Services.Base.Engine.Configuration;
    using Appleseed.Services.Base.Engine.Processors;
    using Appleseed.Services.Base.Model;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator;
    using Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Uses Alchemy API to enlighten the content with key words
    /// </summary>
    public class AppleseedModuleItemIndexOrganizer : BaseOrganizer, IOrganizeThings
    {
        private readonly Common.Logging.ILog logger;
        private readonly string sourceName;
        private Boolean illuminate = false;


        public AppleseedModuleItemIndexOrganizer(ILog logger, string sourceName)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (string.IsNullOrEmpty(sourceName))
            {
                throw new ArgumentNullException("sourceName");
            }

            this.logger = logger;
            this.sourceName = sourceName;

            this.IlluminateService = new HashSet<string>();

            var illuminatorsConfig = ConfigurationManager.GetSection("illuminators") as IlluminatorsSection;

            //DONE: if no illumination, set illuminate to false : by RXS - 10/5/16 
            if (illuminatorsConfig == null)
            {
                illuminate = false;
                return;
            } else
            {
                illuminate = true;
            }

            foreach (IlluminatorsElement illuminator in illuminatorsConfig.illuminators)
            {
                if (illuminator.Type == "Alchemy")
                {
                    var _apiKey = illuminator.ApiKey;
                    this.AppleseedAlchemyApi = new AlchemyAPI();
                    this.AppleseedAlchemyApi.SetAPIKey(_apiKey);
                    this.IlluminateService.Add("A");
                }

                if (illuminator.Type == "OpenCalais")
                {
                    var apiKey = illuminator.ApiKey;
                    this.AppleseedCalaisApi = new CalaisIlluminationService(apiKey);
                    this.IlluminateService.Add("O");
                }
            }

        }

        private AlchemyAPI AppleseedAlchemyApi { get; set; }

        private CalaisIlluminationService AppleseedCalaisApi { get; set; }

        private HashSet<string> IlluminateService { get; set; }

        public void Organize(List<AppleseedModuleItemIndex> itemsToOrganize)
        {
            //DONE: if no illumination, just skip : by RXS - 10/5/16 
            if (illuminate)
            {
                return;
            } else  {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Data";
                var xmlFile = path + "\\OrganizedData.xml";
                if (File.Exists(xmlFile))
                {
                    var content = File.ReadAllText(xmlFile);
                    var result = Core.Serialization.XmlSerialization.Deserialize<List<Model.AppleseedModuleItemIndex>>(content);
                    itemsToOrganize.Clear();
                    itemsToOrganize.AddRange(result);
                    this.logger.Info("Data already organized.");
                    return;
                }

                //List<CollectionIndexItem> SmartItemCollection = new List<CollectionIndexItem>();

                // custom limited concurrency level task scheduler set to do 2 threads
                // Scheduler lcts = new Scheduler(2);

                //List<Task> createItemTasks = new List<Task>();
                //TaskFactory factory = new TaskFactory(lcts);
                //CancellationTokenSource cts = new CancellationTokenSource();

                var index = 0;

                //foreach(string url in urlstoIndex){

                bool isSuccess = true;

                foreach (var item in itemsToOrganize)
                {
                    string url = item.Path;
                    var content = item.Content + " " + item.Summary;
                    this.logger.Info(index + ". Content Length: " + content.Length);

                    /*
                                    createItemTasks.Add(
                                        factory.StartNew(() =>
                                            {
                                                this.logger.Info(this + ". Async Extract Start: " + url);
                                                */
                    //1. Extract Author(s)
                    //2. Extract Categories
                    //3. Extract Concepts
                    //5. Extract Entities
                    //4. Extract Keywords


                    try
                    {
                        this.logger.Info(index + ". Organize Start: " + url);

                        //adding sourceName here -- all data should have a source 
                        item.Source = this.sourceName;

                        if (this.IlluminateService.Contains("A"))
                        {
                            item.SmartKeywords = this.GetKeyWords(content);
                        }

                        if (this.IlluminateService.Contains("O"))
                        {
                            // TO DO: add OpenCalais Illuminator
                            // AppleseedCalaisApi.Illuminate(content);
                        }

                        this.logger.Info(index + ". Found Keywords : " + item.SmartKeywords);
                        this.logger.Info(index + ". Organize End  : " + url);
                    }
                    catch (Exception e)
                    {
                        item.SmartKeywords = string.Empty;
                        item.Source = this.sourceName;
                        isSuccess = false;
                        this.logger.Info(index + ". Error: " + e.Message);
                    }

                    /*
                            }, TaskCreationOptions.LongRunning).ContinueWith(t =>
                            {
                                this.logger.Info(a + ". Async Extract End  : " + "" + url); //+ t.Status 
                            })
                    );       */

                    index++;
                }

                ///////////// GraphDB, close it at this moment
                //try
                //{
                //    if (isSuccess == true)
                //    {
                //        var graphDBsConfig = ConfigurationManager.GetSection("graphDBs") as GraphDBsSection;

                //        foreach (GraphDBsElement graphDB in graphDBsConfig.graphDBs)
                //        {
                //            if (graphDB.Type == "Neo4j")
                //            {
                //                this.logger.Info("GraphDB Storage for " + graphDB.Name + " using Neo4j");
                //                var uri = new Uri(graphDB.Uri);
                //                Neo4jGrapher.Neo4jGrapher grapher = new Neo4jGrapher.Neo4jGrapher(uri);
                //                grapher.WriteIntoNeo4j(itemsToOrganize);
                //                this.logger.Info("GraphDB Storage for " + graphDB.Name + " Succeed");
                //            }
                //            if (graphDB.Type == "RavenDB")
                //            {
                //                // TO DO: RavenDB Graph DB
                //            }
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    this.logger.Info(index + ". Error: " + e.Message);
                //}



                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var xml = Core.Serialization.XmlSerialization.Serialize(itemsToOrganize);
                File.WriteAllText(xmlFile, xml);

                //Task.WaitAll(createItemTasks.ToArray());
            }

        }


        private string GetKeyWords(string input)
        {
            var contentKeywords = new StringBuilder(); //, content_categories, content_concepts, content_entities, content_authors;
            var alchemyXmlKeywords = this.AppleseedAlchemyApi.TextGetRankedKeywords(GetFirstCountKB(input, 150));
            var doc = new XmlDocument();
            doc.LoadXml(alchemyXmlKeywords);
            var root = doc.DocumentElement;
            var nodes = root.SelectNodes("/results/keywords/keyword/text");

            foreach (XmlNode node in nodes)
            {
                contentKeywords.Append(node.InnerText + ",");
            }

            return contentKeywords.ToString();
        }
    }
}
