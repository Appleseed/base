using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appleseed.Services.Base.Engine.Web.API.Models
{
    public class Config
    {
        public List<ConfigItem> ConfigItems { get; set; }
    }

    public class ConfigItem
    { 
        public string ConfigItemName { get; set;}
        public string ConfigItemType { get; set; }
        public SortedDictionary<string, IDictionary<string, string>> ConfigItemValues { get; set; }
    }
}
