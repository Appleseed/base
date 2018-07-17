using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appleseed.Base.Alerts.Model
{
    class UserAlert
    {
        public Guid user_id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string source { get; set; }
    }
}
