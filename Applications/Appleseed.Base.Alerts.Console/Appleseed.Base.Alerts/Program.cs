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
using Appleseed.Base.Alerts.Model;
using Appleseed.Base.Alerts.Controller;
using Appleseed.Base.Alerts.Providers;

namespace Appleseed.Base.Alerts
{
  
    /// <summary>
    /// Main Program for Alert Notifications 
    /// </summary>
    class Program
    {
        #region App Config Values
     

        
        #endregion

        
        static void Main(string[] args)
        {
            Console.WriteLine("INFO : Starting Alert Engine.");

            CheckAlertsProvider();

            Console.WriteLine("INFO : Ending Alert Engine.");
        }

        static void CheckAlertsProvider()
        {
            IAlert aAlertProvider = new EmailAlertProvider();

            aAlertProvider.Run();
        }
        
     
   }

}
