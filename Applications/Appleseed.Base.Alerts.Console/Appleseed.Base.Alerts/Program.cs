using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Dapper;

namespace Appleseed.Base.Alerts
{
  
    class Program
    {

        class UserAlert
        {
            public Guid user_id { get; set; }
            public string email { get; set; }
            public string name { get; set; }
            public string source { get; set; }
        }
        class RootSolrObject
        {

            public SolrResponse response { get; set; }
        }
        class SolrResponse
        {

            public SolrResponseItem[] docs { get; set; }
        }

        class SolrResponseItem

        {
            public string id { get; set; }
            public string[] item_type { get; set; }

            public string[] address_1 { get; set; }
           


        }
        static void Main(string[] args)
        {
            
        }

        

        #region helpers
        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        #endregion
    }
}
