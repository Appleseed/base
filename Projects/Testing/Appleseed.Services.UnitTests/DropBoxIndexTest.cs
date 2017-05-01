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
    public class DropBoxIndexTest
    {
        [TestMethod]
        public void TestMethodNewDropBoxApi(Engine engine)
        {
            ILog logger = LogManager.GetCurrentClassLogger();

            var indexServiceConfig = engine.DropBoxCloudFileIndexServiceSection != null ? engine.DropBoxCloudFileIndexServiceSection[0] : null;

            if (indexServiceConfig != null)
            {
                if (indexServiceConfig.Websites.Count.Equals(0))
                {
                    logger.Info("NO DROPBOX TO INDEX");
                    return;
                }

                logger.Info("BEGIN DROPBOX INDEXING");
                foreach (DropBoxCloudFileToIndexElement source in indexServiceConfig.Websites)
                {
                    logger.Info("SITE: " + source.Name);


                    var indexService = new DropBoxCloudFileIndexServiceNeo(logger, source);

                    try
                    {

                        indexService.Run();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                logger.Info("END DROPBOX INDEXING");
                return;
            }

            logger.Info("NO DROPBOX TO INDEX");
        }

        //[TestMethod]
        //public void TestMethodOldDropBoxApi()
        //{
        //    ILog Logger = LogManager.GetCurrentClassLogger();

        //    var indexServiceConfig = ConfigurationManager.GetSection("dropboxCloudFileIndexService") as DropBoxCloudFileIndexServiceSection;

        //    if (indexServiceConfig != null)
        //    {
        //        if (indexServiceConfig.Websites.Count.Equals(0))
        //        {
        //            Logger.Info("NO DROPBOX TO INDEX");
        //            return;
        //        }

        //        Logger.Info("BEGIN DROPBOX INDEXING");
        //        foreach (DropBoxCloudFileToIndexElement source in indexServiceConfig.Websites)
        //        {
        //            Logger.Info("SITE: " + source.Name);


        //            var indexService = new DropBoxCloudFileIndexService(Logger, source);

        //            try
        //            {

        //                indexService.Run();
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.Error(ex);
        //            }
        //        }

        //        Logger.Info("END DROPBOX INDEXING");
        //        return;
        //    }

        //    Logger.Info("NO DROPBOX TO INDEX");
        //}
    }
}
