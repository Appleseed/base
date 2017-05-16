using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appleseed.Services.Base.Engine.Web.API.Models
{
    public class Config
    {
        public List<ConfigItem> Engine { get; set; }
    }

    public class ConfigItem
    { 
        public string config_name { get; set;}
        public string config_type { get; set; }
        public SortedDictionary<string, IDictionary<string, string>> config_values { get; set; }
    }
}
