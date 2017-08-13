﻿using System;
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
        public string code_info { get; set; }

        public string reason_for_recall { get; set; }
        public DateTime recall_initiation_date { get; set; }
        public string recall_number { get; set; }
        public string recalling_firm { get; set; }
        public string voluntary_mandated { get; set; }

        public DateTime report_date { get; set; }
        public string status { get; set; }
		
		public DateTime date_last_indexed { get; set; }

        //IRefusal
        public string fei_number { get; set; }
        public string product_code { get; set; }
        public string product_code_description { get; set; }
        public DateTime refusal_date { get; set; }
        public string entry_number { get; set; }
        public string rfrnc_doc_id { get; set; }
        public string line_number { get; set; }
        public string line_sfx_id { get; set; }
        public string fda_sample_analysis { get; set; }
        public string private_lab_analysis { get; set; }
        public string refusal_charges { get; set; }

        //ICitations
        public string act_cfr { get; set; }
        public string program_area { get; set; }
        public string description_short { get; set; }
        public string description_long { get; set; }
        public DateTime end_date { get; set; }

        //IClassification
        public string district_decision { get; set; }
        public string district  { get; set; }
        public DateTime inspection_end_date { get; set; }
        public string center { get; set; }
        public string project_area { get; set; }
        public string legal_name { get; set; }

       
    }
}
