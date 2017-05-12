using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Cassandra;

namespace Appleseed.Services.Base.Engine.Web.API.Controllers
{ 
    [Produces("application/json")]
    [Route("api/Config")]
    public class ConfigController : Controller
    {
        private string appConfigSource = "127.0.0.1";

        private int appConfigSourcePort = 9042;

        // GET: api/Config
        [HttpGet]
        public List<string> Get()
        {
            //var appConfigSource = ConfigurationManager.AppSettings["CassandraUrl"];
            //var appConfigSourcePort = Convert.ToInt32(ConfigurationManager.AppSettings["CassandraPort"]);
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var engineItems = session.Execute("select json * from config");
            var engineJson = new List<string>();

            foreach (var item in engineItems)
            {
                engineJson.Add( item["[json]"].ToString());
            }
            return engineJson;
        }

        // GET: api/Config/source
        [HttpGet("{type}", Name = "Get Config Type")]
        public List<string> Get(string type)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var engineItems = session.Execute("select json * from config where config_type = '" + type + "'");
            var engineJson = new List<string>();

            foreach (var item in engineItems)
            {
                engineJson.Add(item["[json]"].ToString());
            }

            return engineJson;
        }

        // GET: api/Config/source/Data.XML
        [HttpGet("{type}/{name}", Name = "Get Config Type and Name")]
        public List<string> Get(string type, string name)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var engineItems = session.Execute("select json * from config where config_type = '" + type + "' and config_name = '" + name + "'");
            var engineJson = new List<string>();

            foreach (var item in engineItems)
            {
                engineJson.Add(item["[json]"].ToString());
            }

            return engineJson;
        }

        // POST: api/Config
        [HttpPost("{type}")]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Config/source
        [HttpPut("{type}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/source
        [HttpDelete("{type}")]
        public void Delete(int id)
        {
        }
    }
}
