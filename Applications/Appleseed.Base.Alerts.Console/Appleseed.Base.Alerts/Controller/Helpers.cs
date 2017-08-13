using Appleseed.Base.Alerts.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace Appleseed.Base.Alerts.Controller
{
    class Helpers
    {
        #region App Config Values


        static IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        #endregion
        #region helpers
        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static List<UserAlert> GetUserAlertSchedules(string scheudle)
        {
            var userAlerts = db.Query<UserAlert>("GetPortalUserAlerts", new { alert_schedule = scheudle },
            commandType: CommandType.StoredProcedure).ToList<UserAlert>();


            return userAlerts;
        }
        public static bool UpdateUserSendDate(Guid userID, DateTime date)
        {
            int rowsAffected = db.Execute("");

            if (rowsAffected > 0)
            {
                return true;
            }
            return false;

        }
        #endregion
    }
}
