using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Cassandra;
using Newtonsoft.Json;
using Appleseed.Services.Base.Engine.Web.API.Models;

namespace Appleseed.Services.Base.Engine.Web.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Config")]
    public class ConfigController : Controller
    {
        private string appConfigSource = "127.0.0.1";

        private int appConfigSourcePort = 9042;

        private Config GetConfig(RowSet rowSet)
        {
            var engineJson = new Config();
            var configList = new List<ConfigItem>();

            foreach (var itemRow in rowSet)
            {
                var configItem = new ConfigItem();
                configItem.config_name = (itemRow["config_name"] ?? "").ToString();
                configItem.config_type = (itemRow["config_type"] ?? "").ToString();
                configItem.config_values = (SortedDictionary<string, IDictionary<string, string>>)(itemRow["config_values"]);
                configList.Add(configItem);
            }
            engineJson.Engine = configList;

            return engineJson;
        }

        // GET: api/Config
        [HttpGet(Name = "Get Config")]
        public Config Get()
        {
            //var appConfigSource = ConfigurationManager.AppSettings["CassandraUrl"];
            //var appConfigSourcePort = Convert.ToInt32(ConfigurationManager.AppSettings["CassandraPort"]);
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var engineItems = session.Execute("select * from config");
            var engineJson = GetConfig(engineItems);
            return engineJson;
        }

        // GET: api/Config/source
        [HttpGet("{type}", Name = "Get Config Type")]
        public Config Get(string type)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var engineItems = session.Execute("select * from config where config_type = '" + type + "'");
            var engineJson = GetConfig(engineItems);

            return engineJson;
        }

        // GET: api/Config/source/Data.XML
        [HttpGet("{type}/{name}", Name = "Get Config Type and Name")]
        public Config Get(string type, string name)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var engineItems = session.Execute("select * from config where config_type = '" + type + "' and config_name = '" + name + "'");
            var engineJson = GetConfig(engineItems);

            return engineJson;
        }

        // POST: api/Config
        [HttpPost()]
        public IActionResult Post([FromBody] dynamic data)
        {
            if (data.config_type == null || data.config_name == null)
            {
                return BadRequest();
            }

            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            data = JsonConvert.SerializeObject(data);
            data = JsonConvert.DeserializeObject<ConfigItem>(data);

            var check = session.Prepare("select * from config where config_type = ? and config_name = ?");
            var checkStatement = check.Bind(data.config_type, data.config_name);
            var checkResults = session.Execute(checkStatement);

            if (checkResults.GetAvailableWithoutFetching() > 0)
            {
                return BadRequest();
            }

            var prep = session.Prepare("insert into config (config_type, config_name, config_values) values (?, ?, ?)");
            var statement = prep.Bind(data.config_type, data.config_name, data.config_values);
            session.Execute(statement);

            return CreatedAtRoute("Get Config", data);
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
