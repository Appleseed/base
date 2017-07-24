using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appleseed.Base.Alerts.Model;
using System.Data;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Appleseed.Base.Alerts.Providers
{
    class EmailAlertProvider : IAlert
    {
        static IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        static string SolrURL = System.Configuration.ConfigurationManager.AppSettings["SolrURL"];

        public List<UserAlert> GetUserAlertSchedules(string scheudle)
        {
            var userAlerts = db.Query<UserAlert>("GetPortalUserAlerts", new { alert_schedule = scheudle },
            commandType: CommandType.StoredProcedure).ToList<UserAlert>();

            return userAlerts;
        }

        public RootSolrObject GetSearchAlertViaSolr(string query)
        {
            // perform split function

            string url = SolrURL + WebUtility.HtmlDecode(query);
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
            getRequest.Method = "GET";

            using (var getResponse = (HttpWebResponse)getRequest.GetResponse())
            {
                Stream newStream = getResponse.GetResponseStream();
                StreamReader sr = new StreamReader(newStream);

                var result = sr.ReadToEnd();

                var searchResults = JsonConvert.DeserializeObject<RootSolrObject>(result);

                return searchResults;

            }

        }
    }
}
