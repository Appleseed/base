using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System;
using System.Configuration;

namespace Appleseed.Services.Search.Console.helpers
{
    public static class XMLtoClass
    {
        public static List<Process> GetEngineConfiguration()
        {
            XDocument xmlDoc = XDocument.Load(Constant.EngineProcessFileName);
            var process = (from template in xmlDoc.Descendants("Process") select new Process()
            {
                Class = template.Attribute("Class").Value,
                Enabled = Convert.ToBoolean(template.Attribute("Enabled").Value),
                MethodName = template.Attribute("MethodName").Value,
                Name = template.Attribute("Name").Value,
                Section = template.Attribute("Section").Value,
                SortOrder = Convert.ToInt32(template.Attribute("SortOrder").Value)
            }).ToList();

            return process.OrderBy(x => x.SortOrder).ToList();
        }

        public static Configuration GetExternalConfig()
        {
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = Constant.EngineFileName;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            return config;            
        }
    }
}
