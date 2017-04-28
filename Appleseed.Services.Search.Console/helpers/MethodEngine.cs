using Appleseed.Services.Base.Engine.Services.Impl;
using Appleseed.Services.Base.Engine.Configuration;
using Appleseed.Services.Core.Helpers;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Appleseed.Services.Search.Console
{
    public class MethodEngine
    {
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        public static void RunDropboxIndexService(Engine engine)
        {
            //var indexServiceConfig = ConfigurationManager.GetSection(sectionName) as DropBoxCloudFileIndexServiceSection;
            var indexServiceConfig = engine.DropBoxCloudFileIndexServiceSection != null ? engine.DropBoxCloudFileIndexServiceSection[0] : null;

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


                    var indexService = new Appleseed.Services.Base.Engine.Services.Impl.DropBoxCloudFileIndexService(Logger, source);

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

        public static void RunNutshellIndexService(Engine engine)
        {
            //var indexServiceConfig = ConfigurationManager.GetSection(sectionName) as NutshellCRMApiCrawlerIndexServiceSection;
            //var indexServiceConfig = config.Sections[sectionName] as NutshellCRMApiCrawlerIndexServiceSection; // ConfigurationManager.GetSection("nutshellCRMApiCrawlerIndexService") as NutshellCRMApiCrawlerIndexServiceSection;
            var indexServiceConfig = engine.NutshellCRMApiCrawlerIndexServiceSection != null ? engine.NutshellCRMApiCrawlerIndexServiceSection[0] : null;

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

        public static void RunCrunchBaseIndexService(Engine engine)
        {
            //var indexServiceConfig = ConfigurationManager.GetSection(sectionName) as CrunchBaseApiCrawlerIndexServiceSection;
            //var indexServiceConfig = config.Sections[sectionName] as CrunchBaseApiCrawlerIndexServiceSection; //ConfigurationManager.GetSection("crunchbaseApiCrawlerIndexService") as CrunchBaseApiCrawlerIndexServiceSection;
            var indexServiceConfig = engine.CrunchBaseApiCrawlerIndexServiceSection != null ? engine.CrunchBaseApiCrawlerIndexServiceSection[0] : null;

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

        public static void RunWebCrawlerIndexService(Engine engine)
        {
            //var indexServiceConfig = ConfigurationManager.GetSection(sectionName) as WebCrawlIndexServiceSection;
            //var indexServiceConfig = config.Sections[sectionName] as WebCrawlIndexServiceSection; //ConfigurationManager.GetSection("webcrawlIndexService") as WebCrawlIndexServiceSection;
            var indexServiceConfig = engine.WebCrawlIndexServiceSection != null ? engine.WebCrawlIndexServiceSection[0] : null;

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

        public static void RunRssContentIndexService(Engine engine)
        {
            //var indexServiceConfig = ConfigurationManager.GetSection(sectionName) as WebsiteIndexServiceSection;
            //var indexServiceConfig = config.Sections[sectionName] as WebsiteIndexServiceSection; // ConfigurationManager.GetSection("rssIndexService") as WebsiteIndexServiceSection;
            var indexServiceConfig = engine.rssIndexServiceSection != null ? engine.rssIndexServiceSection[0] : null;

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
                        var indexService = new RssContentIndexService(Logger, source, siteMap, engine);

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

        public static void RunIdentityIndexService()
        {
            var indexService = new IdentityIndexService(Logger, ConfigurationManager.AppSettings[helpers.Constant.LocalPathToLuceneIndex]);
            indexService.Run();
        }

        public static void RunWebContentIndexService(Engine engine)
        {
            //var indexServiceConfig = ConfigurationManager.GetSection(sectionName) as WebsiteIndexServiceSection;
            //var indexServiceConfig = config.Sections[sectionName] as WebsiteIndexServiceSection; // ConfigurationManager.GetSection("websiteIndexService") as WebsiteIndexServiceSection;
            var indexServiceConfig = engine.WebsiteIndexServiceSection != null ? engine.WebsiteIndexServiceSection[0] : null;

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


                    var indexService = new WebContentIndexService(Logger, source, engine);

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

        public static void RunFileContentIndexService(Engine engine)
        {
            //var indexServiceConfig = ConfigurationManager.GetSection(sectionName) as DbFileIndexServiceSection;
            //var indexServiceConfig = config.Sections[sectionName] as DbFileIndexServiceSection; //)ConfigurationManager.GetSection("dbFileIndexService");
            var indexServiceConfig = engine.DbFileIndexServiceSection != null ? engine.DbFileIndexServiceSection[0] : null;

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

        public static void RunTaggingService(Engine engine)
        {
            //var serviceConfig = ConfigurationManager.GetSection(sectionName) as TagServiceSection;
            //var serviceConfig = config.Sections[sectionName] as TagServiceSection; // ConfigurationManager.GetSection("taggingService") as TagServiceSection;
            var serviceConfig = engine.TagServiceSection != null ? engine.TagServiceSection[0] : null;

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
    }
}