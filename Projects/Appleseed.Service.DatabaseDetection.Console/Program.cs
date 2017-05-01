using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appleseed.Service.DatabaseDetection.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //DB connection
            string connectionString = @"server=localhost\SQLEXPRESS;Trusted_Connection=yes";

            FileInfo file = new FileInfo(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\test.sql");
            //Console.WriteLine(file.FullName);

            string script = file.OpenText().ReadToEnd();
            SqlConnection con = new SqlConnection(connectionString);
            Server server = new Server(new ServerConnection(con));
            //modify
            //<startup useLegacyV2RuntimeActivationPolicy="true"> 
            server.ConnectionContext.ExecuteNonQuery(script);

            
            
        }
    }
}
