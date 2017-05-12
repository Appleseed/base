using System;
using System.Collections.Generic;
using System.Configuration;
using Appleseed.Services.Base.Engine.Configuration;
using Appleseed.Services.Base.Engine.Services.Impl;
using Appleseed.Services.Core.Helpers;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appleseed.Services.UnitTests
{
    [TestClass]
    public class RssIndexTest
    {
        [TestMethod]
        public void TestMethod1(Engine engine)
        {

             ILog Logger = LogManager.GetCurrentClassLogger();


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
        }
    }
}
