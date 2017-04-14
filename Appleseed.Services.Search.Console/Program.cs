namespace Appleseed.Services.Search.Console
{
    using Appleseed.Services.Base.Engine.Configuration;
    using Appleseed.Services.Base.Engine.Services.Impl;
    using Appleseed.Services.Core.Helpers;
    using Cassandra;
    using Common.Logging;
    using helpers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Diagnostics;
    using System.Collections;
    using System.Net;
    using System.Xml.Linq;
    using System.Xml;

    class Manager
    {
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        private static bool _stopService = false;
        private static bool reRunProcess = false;
        static Thread ServiceThread;
        static Thread WatcherThread;
        static ArrayList PreErrors = new ArrayList();
        static int solrStatus = 0;

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Service Process");
                /* PreCheck("IndexesSection");
                 CheckSolrStatus();
                 TimeWait();*/

                ServiceThread = new Thread(ExecuteEngineProcesses) { Name = "Run Service Thread" };
                WatcherThread = new Thread(ChangeWatcher) { Name = "Run Watcher Thread" };
                ServiceThread.Start();
                WatcherThread.Start();
                Console.WriteLine("Press any key to stop...");
                Console.ReadLine();
                _stopService = true;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);

            }


            ////RunIdentityIndexService();

            //RunWebContentIndexService();

            ////RunTaggingService();

            //RunFileContentIndexService();

            //RunRssContentIndexService();

            //RunNutshellIndexService();

            //RunDropboxIndexService();

            //RunCrunchBaseIndexService();

            //RunWebCrawlerIndexService();

            //Console.Read();

        }

        private static void ExecuteEngineProcesses()
        {
            var configSource = ConfigurationManager.AppSettings["ReadConfigFrom"];
            Engine config;
            if (configSource == "datastax" || configSource == "cassandra")
            {
                config = CassandraToClass.GetCassandraConfig();
            }
            else
            {
                string xml = File.ReadAllText(Constant.EngineFileName);
                config = new Engine(xml);
            }

            //Console.ReadKey();
            List<helpers.Process> processList = helpers.XMLtoClass.GetEngineConfiguration();
            //Configuration externalConfig = helpers.XMLtoClass.GetExternalConfig();

            foreach (helpers.Process process in processList)
            {
                if (!process.Enabled)
                    continue;

                MethodInvoke(process.Class, process.MethodName, config);
            }
            
            ExitProgram();
        }



        private static void ChangeWatcher()
        {
            string assemblyDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            NotifyFilters notifyFilters = NotifyFilters.LastWrite;

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher()
            {
                //Path = assemblyDirectory,
                Path = Environment.CurrentDirectory.ToString() + @"\config",
                NotifyFilter = notifyFilters,
                Filter = "*.xml"
            };
            fileSystemWatcher.Changed += OnChanged;
            fileSystemWatcher.EnableRaisingEvents = true;


            Console.WriteLine("Watching for changes...");
            while (!_stopService)
            {
                Thread.Sleep(5 * 1000);
                if (reRunProcess)
                {
                    reRunProcess = false;
                    ServiceThread.Abort();
                    //Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Re-run the process after update(s) on config/xml file(s)");
                    //Console.ForegroundColor = ConsoleColor.White;
                    ServiceThread = new Thread(ExecuteEngineProcesses) { Name = "Run Service Thread" };
                    ServiceThread.Start();

                }
            }
        }

        static void OnChanged(object source, FileSystemEventArgs e)
        {

            reRunProcess = true;
            Console.ForegroundColor = ConsoleColor.Red;
           // Console.WriteLine("Round " + ++i);
            Console.WriteLine("Change event handler invoked...");
            Console.ForegroundColor = ConsoleColor.White;

        }


        private static void MethodInvoke(string className, string methodName, Engine engine)
        {
            // get the type ( class ) from string.
            Type type = Type.GetType(className);

            if (type == null)
                return;

            // create instance of type ( class ).
            object instance = Activator.CreateInstance(type);

            if (instance == null)
                return;

            // get method from type ( class ).
            MethodInfo methodInfo = type.GetMethod(methodName);

            if (methodInfo == null)
                return;

            // run / execute method.
            methodInfo.Invoke(instance, new object[] { engine });
        }

        private static void TimeWait()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();


            while(PreErrors.Count > 0 || solrStatus != 0 )
            {
                Thread.Sleep(3000);
                Console.WriteLine("Re-trying...");
                PreCheck("IndexesSection");
                CheckSolrStatus();
            }
            sw.Stop();
        }

        private static void PreCheck(string section)
        {
            var name = "";
            string xml = File.ReadAllText(Constant.EngineFileName);
            Engine config = new Engine(xml);

            if(config.IndexesSection.Count > 0)
            {           
                int c = 0;
                for (var i=0; i < config.IndexesSection[0].Indexes.Count; i++)
                {
                    /* TODO: Uncomment related code in Base.EngineCfg\Configuration\Indexes.cs and below once Indexes.cs can access config/engine.map.xml
                    if (!Directory.Exists(config.IndexesSection[0].Indexes[c].Location.ToString()))
                    {
                        name = config.IndexesSection[0].Indexes[c].Name.ToString();
                        Console.WriteLine(name  + "'s Location not exist");
                        PreErrors.Add(name);
                    }

                    c++;
                    */
                }

            }

        }

        private static void CheckSolrStatus()
        {
            try
            {
                var url = ConfigurationManager.AppSettings["Solr_xml"];
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();

                using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    XDocument xmlDoc = new XDocument();
                    try
                    {
                        xmlDoc = XDocument.Parse(sr.ReadToEnd());
                        solrStatus = int.Parse(xmlDoc.Root.Element("lst").Element("int").Value);



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
          

            //solrStatus;
        }

        private static bool isLocked(string path)
        {
            
            try
            {
                FileStream fs = File.OpenWrite(path);

                fs.Close();
                return false;
            }
            catch(Exception)
            {
                return true;
            }

         
        }

        private static void RunDropboxIndexService()
        {
            var indexServiceConfig = ConfigurationManager.GetSection("dropboxCloudFileIndexService") as DropBoxCloudFileIndexServiceSection;

            if (indexServiceConfig != null)
            {
                if (indexServiceConfig.Websites.Count.Equals(0))
                {
                    Logger.Info("NO DROPBOX TO INDEX");
                    return;
                }

                Logger.Info("BEGIN DROPBOX INDEXING");
                foreach (DropBoxCloudFileToIndexElement source in indexServiceConfig.Websites)
                {
                    Logger.Info("SITE: " + source.Name);


                    var indexService = new DropBoxCloudFileIndexService(Logger, source);

                    try
                    {

                        indexService.Run();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

                Logger.Info("END DROPBOX INDEXING");
                return;
            }

            Logger.Info("NO DROPBOX TO INDEX");
        }

        private static void RunNutshellIndexService()
        {
            var indexServiceConfig = ConfigurationManager.GetSection("nutshellCRMApiCrawlerIndexService") as NutshellCRMApiCrawlerIndexServiceSection;

            if (indexServiceConfig != null)
            {
                if (indexServiceConfig.Websites.Count.Equals(0))
                {
                    Logger.Info("NO NUTSHELL TO INDEX");
                    return;
                }

                Logger.Info("BEGIN NUTSHELL INDEXING");
                foreach (NutshellCRMApiCrawlerToIndexElement source in indexServiceConfig.Websites)
                {
                    Logger.Info("SITE: " + source.Name);
                    var indexService = new NutshellCRMApiCrawlerIndexService(Logger, source);

                    try
                    {
                        indexService.Run();

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

                Logger.Info("END NUTSHELL INDEXING");
                return;
            }

            Logger.Info("NO NUTSHELL TO INDEX");
        }

        private static void RunCrunchBaseIndexService()
        {
            var indexServiceConfig = ConfigurationManager.GetSection("crunchbaseApiCrawlerIndexService") as CrunchBaseApiCrawlerIndexServiceSection;

            if (indexServiceConfig != null)
            {
                if (indexServiceConfig.Websites.Count.Equals(0))
                {
                    Logger.Info("NO CRUNCHBASE TO INDEX");
                    return;
                }

                Logger.Info("BEGIN CRUNCHBASE INDEXING");
                foreach (CrunchBaseApiCrawlerToIndexElement source in indexServiceConfig.Websites)
                {
                    Logger.Info("SITE: " + source.Name);
                    var indexService = new CrunchBaseApiCrawlerIndexService(Logger, source);

                    try
                    {
                        indexService.Run();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

                Logger.Info("END CRUCHBASE INDEXING");
                return;
            }

            Logger.Info("NO CRUNCHBASE TO INDEX");
        }

        private static void RunWebCrawlerIndexService()
        {
            var indexServiceConfig = ConfigurationManager.GetSection("webcrawlIndexService") as WebCrawlIndexServiceSection;

            if (indexServiceConfig != null)
            {
                if (indexServiceConfig.Websites.Count.Equals(0))
                {
                    Logger.Info("NO CRAWLER TO INDEX");
                    return;
                }

                Logger.Info("BEGIN CRAWLER AGGREGATE");
                foreach (WebCrawlToIndexElement source in indexServiceConfig.Websites)
                {
                    Logger.Info("SITE: " + source.Name);

                    var indexService = new WebCrawlerIndexService(Logger, source);
                    try
                    {
                        indexService.Run();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

                Logger.Info("END CRAWLER AGGREGATE");
                return;
            }

            Logger.Info("NO CRAWLER TO INDEX");
        }

        private static void RunRssContentIndexService()
        {
            var indexServiceConfig = ConfigurationManager.GetSection("rssIndexService") as rssIndexServiceSection;

            if (indexServiceConfig != null)
            {
                if (indexServiceConfig.Websites.Count.Equals(0))
                {
                    Logger.Info("NO RSS TO INDEX");
                    return;
                }

                Logger.Info("BEGIN RSS INDEXING");
                foreach (rssIndexElement source in indexServiceConfig.Websites)
                {
                    Logger.Info("SITE: " + source.Name);

                    List<string> targetUrls = Utilities.GenerateSequence_UrlParse(source.SiteMapUrl);
                    if (targetUrls.Count.Equals(0))
                    {
                        Logger.Info("NO RSS TO INDEX");
                        return;
                    }
                    foreach (string siteMap in targetUrls)
                    {
                        var indexService = new RssContentIndexService(Logger, source, siteMap);

                        try
                        {
                            indexService.Run();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                }

                Logger.Info("END RSS INDEXING");
                return;
            }

            Logger.Info("NO RSS TO INDEX");
        }

        private static void RunIdentityIndexService()
        {
            var indexService = new IdentityIndexService(Logger, ConfigurationManager.AppSettings["local_PathToLuceneIndex"]);
            indexService.Run();
        }

        private static void RunWebContentIndexService()
        {
            var indexServiceConfig = ConfigurationManager.GetSection("websiteIndexService") as WebsiteIndexServiceSection;

            if (indexServiceConfig != null)
            {
                if (indexServiceConfig.Websites.Count.Equals(0))
                {
                    Logger.Info("NO WEBSITES TO INDEX");
                    return;
                }

                Logger.Info("BEGIN WEBSITE INDEXING");
                foreach (WebsiteToIndexElement source in indexServiceConfig.Websites)
                {
                    Logger.Info("SITE: " + source.Name);


                    var indexService = new WebContentIndexService(Logger, source);

                    try
                    {
                        indexService.Run();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

                Logger.Info("END WEBSITE INDEXING");
                return;
            }

            Logger.Info("NO WEBSITES TO INDEX");
        }

        private static void RunFileContentIndexService()
        {
            var indexServiceConfig = (DbFileIndexServiceSection)ConfigurationManager.GetSection("dbFileIndexService");

            if (indexServiceConfig != null)
            {
                if (indexServiceConfig.Files.Count.Equals(0))
                {
                    Logger.Info("NO FILES TO INDEX");
                    return;
                }

                Logger.Info("BEGIN FILE INDEXING");
                foreach (var source in indexServiceConfig.Files)
                {
                    Logger.Info("FILE: " + source.Name);

                    var indexService = new FileContentIndexService(Logger, source);

                    try
                    {
                        indexService.Run();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

                Logger.Info("END FILE INDEXING");
                return;
            }

            Logger.Info("NO FILES TO INDEX");
        }

        private static void RunTaggingService()
        {
            var serviceConfig = ConfigurationManager.GetSection("taggingService") as TagServiceSection;

            if (serviceConfig != null)
            {
                if (serviceConfig.Sites.Count.Equals(0))
                {
                    Logger.Info("NO SITES TO TAG");
                    return;
                }

                Logger.Info("BEGIN TAGGING SERVICE");
                foreach (SiteToTagElement site in serviceConfig.Sites)
                {
                    try
                    {
                        Logger.Info("SITE: " + site.Name);
                        var tagger = new Appleseed.Services.Tagger.SiteAutoTagger(Logger, site.ConnectionString);
                        tagger.Tag(site.Url, site.PortalId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

                Logger.Info("END TAGGING SERVICE");
                return;
            }

            Logger.Info("NO SITES TO TAG");
        }

        private static void ExitProgram()
        {
            var isExitAfterRunEnabled = ConfigurationManager.AppSettings["ExitAfterRun"];
            Logger.Info("CHECKING CONFIG FOR AUTO EXIT AFTER RUN...");
            //Console.ReadKey();
            if (isExitAfterRunEnabled.ToLower() == "true" || isExitAfterRunEnabled.ToLower() == "t")
            {
                Logger.Info("AUTO EXIT AFTER RUN ENABLED. NOW EXITING...");
                Environment.Exit(0);
            }
            else {
                Logger.Info("AUTO EXIT AFTER RUN DISABLED. PRESS ANY KEY.");
            }
        }
    }
}