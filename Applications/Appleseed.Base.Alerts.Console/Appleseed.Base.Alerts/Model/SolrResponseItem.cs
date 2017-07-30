using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appleseed.Base.Alerts.Model
{
    class SolrResponseItem
    {
        public string id { get; set; }
        public string item_type { get; set; }

        public string address_1 { get; set; }

        public string city { get; set; }

        public string state { get; set; }
        public string classification { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }

        public string product_description { get; set; }
        public string product_quantity { get; set; }
        public string product_type { get; set; }
        public string[] code_info { get; set; }

        public string[] reason_for_recall { get; set; }
        public DateTime recall_initiation_date { get; set; }
        public string[] recall_number { get; set; }
        public string recalling_firm { get; set; }
        public string[] voluntary_mandated { get; set; }

        public DateTime report_date { get; set; }
        public string[] status { get; set; }
    }
}
