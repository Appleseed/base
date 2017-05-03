using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appleseed.Services.Search.Console.helpers
{
    public class Process
    {        
        public string Name { get; set; }
        public string Section { get; set; }
        public string ConfigClass { get; set; }
        public string Class { get;set;}
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }
        public string MethodName { get; set; }
    }
}
