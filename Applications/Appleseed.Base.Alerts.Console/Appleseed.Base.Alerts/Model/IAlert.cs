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
        JSONRootObject GetAlert(string query);
        Task SendAlert(string email, string link, JSONRootObject results, object mailResponse);
        bool UpdateUserSendDate(Guid userID, DateTime date);
    }
}
