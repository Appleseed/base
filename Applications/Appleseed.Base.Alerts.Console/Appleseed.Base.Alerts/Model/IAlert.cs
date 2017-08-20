using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appleseed.Base.Alerts.Model
{
    interface IAlert
    {

        List<UserAlert> GetUserAlertSchedules(string scheudle);
        SolrRootObject GetSearchAlertViaSolr(string query);
        Task SendAlert(string email, string link, SolrRootObject results, object mailResponse);
        bool UpdateUserSendDate(Guid userID, DateTime date);
    }
}
