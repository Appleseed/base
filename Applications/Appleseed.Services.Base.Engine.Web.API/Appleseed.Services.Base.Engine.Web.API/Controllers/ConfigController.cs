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

            var check = session.Prepare("select * from config where config_type = ?");
            var checkStatement = check.Bind(type);
            var checkResults = session.Execute(checkStatement);
            var engineJson = GetConfig(checkResults);

            return engineJson;
        }

        // GET: api/Config/source/Data.XML
        [HttpGet("{type}/{name}", Name = "Get Config Type and Name")]
        public Config Get(string type, string name)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var check = session.Prepare("select * from config where config_type = ? and config_name = ?");
            var checkStatement = check.Bind(type, name);
            var checkResults = session.Execute(checkStatement);
            var engineJson = GetConfig(checkResults);

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

        // POST: api/Config/source/Data.XML
        [HttpPost("{type}/{name}")]
        public IActionResult Post([FromBody] dynamic data, string type, string name)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            data.config_type = type;
            data.config_name = name;
            data = JsonConvert.SerializeObject(data);
            data = JsonConvert.DeserializeObject<ConfigItem>(data);

            var check = session.Prepare("select * from config where config_type = ? and config_name = ?");
            var checkStatement = check.Bind(type, name);
            var checkResults = session.Execute(checkStatement);
            var results = checkResults.GetRows().ToList();

            if (results.Count() == 0)
            {
                return BadRequest();
            }

            var allValues = new SortedDictionary<string, IDictionary<string, string>>();

            foreach (var dataValues in data.config_values)
            {
                foreach (var itemRow in results)
                {
                    var itemValues = (SortedDictionary<string, IDictionary<string, string>>)(itemRow["config_values"]);
                    foreach (var itemValue in itemValues)
                        if (allValues.ContainsKey(itemValue.Key) == false)
                        {
                            allValues.Add(itemValue.Key, itemValue.Value);
                        }
                    if (itemValues.ContainsKey(dataValues.Key) == false)
                    {
                        allValues.Add(dataValues.Key, dataValues.Value);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            var prep = session.Prepare("insert into config (config_type, config_name, config_values) values (?, ?, ?)");
            var statement = prep.Bind(type, name, allValues);
            session.Execute(statement);

            return CreatedAtRoute("Get Config", data);
        }

        // POST: api/Config/source/Data.XML/Contacts
        [HttpPost("{type}/{name}/{item}")]
        public IActionResult Post([FromBody] dynamic data, string type, string name, string item)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var check = session.Prepare("select * from config where config_type = ? and config_name = ?");
            var checkStatement = check.Bind(type, name);
            var checkResults = session.Execute(checkStatement);
            var results = checkResults.GetRows().ToList();

            if (results.Count == 0)
            {
                return BadRequest();
            }

            var allValues = new SortedDictionary<string, IDictionary<string, string>>();

            foreach (var dataValues in data)
            {
                var itemValues = (SortedDictionary<string, IDictionary<string, string>>)(results.First()["config_values"]);
                foreach (var itemValue in itemValues)
                {
                    if (allValues.ContainsKey(itemValue.Key) == false)
                    {
                        allValues.Add(itemValue.Key, itemValue.Value);
                    }
                }
                if (itemValues.ContainsKey(item) == true)
                {
                    if (itemValues[item].ContainsKey(dataValues.Name) == false)
                    {
                        allValues[item].Add(dataValues.Name, dataValues.Value.ToString());
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }

            var prep = session.Prepare("insert into config (config_type, config_name, config_values) values (?, ?, ?)");
            var statement = prep.Bind(type, name, allValues);
            session.Execute(statement);

            return CreatedAtRoute("Get Config", data);
        }

        // PUT: api/Config/source/Data.XML
        [HttpPut("{type}/{name}")]
        public IActionResult Put([FromBody] dynamic data, string type, string name)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            data.config_type = type;
            data.config_name = name;
            data = JsonConvert.SerializeObject(data);
            data = JsonConvert.DeserializeObject<ConfigItem>(data);

            var check = session.Prepare("select * from config where config_type = ? and config_name = ?");
            var checkStatement = check.Bind(type, name);
            var checkResults = session.Execute(checkStatement);
            var results = checkResults.GetRows().ToList();

            if (results.Count() == 0)
            {
                return BadRequest();
            }
            var itemValues = (SortedDictionary<string, IDictionary<string, string>>)(results.First()["config_values"]);
            foreach (var dataValues in data.config_values)
            {
                foreach (var itemRow in results)
                {
                    foreach (var itemValue in itemValues.ToList())
                    {
                        if (itemValues.ContainsKey(dataValues.Key) == true)
                        {
                            itemValues[dataValues.Key] = dataValues.Value;
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                }
            }
            var prep = session.Prepare("insert into config (config_type, config_name, config_values) values (?, ?, ?)");
            var statement = prep.Bind(type, name, itemValues);
            session.Execute(statement);

            return CreatedAtRoute("Get Config Type and Name", data);
        }

        // PUT: api/Config/source/Data.XML/Contacts
        [HttpPut("{type}/{name}/{item}")]
        public IActionResult Put([FromBody] dynamic data, string type, string name, string item)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var check = session.Prepare("select * from config where config_type = ? and config_name = ?");
            var checkStatement = check.Bind(type, name);
            var checkResults = session.Execute(checkStatement);
            var results = checkResults.GetRows().ToList();

            if (results.Count() == 0)
            {
                return BadRequest();
            }

            var itemValues = (SortedDictionary<string, IDictionary<string, string>>)(results.First()["config_values"]);

            foreach (var dataValues in data)
            {
                foreach (var itemValue in itemValues)
                {
                    if (itemValues.ContainsKey(item) == true)
                    {
                        if (itemValues[item].ContainsKey(dataValues.Name) == true)
                        {
                            itemValues[item][dataValues.Name] = dataValues.Value.ToString();
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            
            var prep = session.Prepare("insert into config (config_type, config_name, config_values) values (?, ?, ?)");
            var statement = prep.Bind(type, name, itemValues);
            session.Execute(statement);

            return CreatedAtRoute("Get Config Type and Name", data);
        }

        // PUT: api/Config/source/Data.XML/Contacts/indexPath
        [HttpPut("{type}/{name}/{item}/{valueKey}")]
        public IActionResult Put([FromBody] dynamic data, string type, string name, string item, string valueKey)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var check = session.Prepare("select * from config where config_type = ? and config_name = ?");
            var checkStatement = check.Bind(type, name);
            var checkResults = session.Execute(checkStatement);
            var results = checkResults.GetRows().ToList();

            if (results.Count() == 0)
            {
                return BadRequest();
            }

            var itemValues = (SortedDictionary<string, IDictionary<string, string>>)(results.First()["config_values"]);
            foreach (var itemValue in itemValues)
            {
                if (itemValues.ContainsKey(item) == true)
                {
                    if (itemValues[item].ContainsKey(valueKey) == true)
                    {
                        itemValues[item][valueKey] = data.ToString();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }

            var prep = session.Prepare("insert into config (config_type, config_name, config_values) values (?, ?, ?)");
            var statement = prep.Bind(type, name, itemValues);
            session.Execute(statement);

            return CreatedAtRoute("Get Config Type and Name", data);
        }

        // DELETE: api/Config/source
        [HttpDelete("{type}")]
        public IActionResult Delete(string type)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var check = session.Prepare("select * from config where config_type = ?");
            var checkStatement = check.Bind(type);
            var checkResults = session.Execute(checkStatement);
            var results = checkResults.GetRows().ToList();

            if (results.Count() == 0)
            {
                return BadRequest();
            }

            var prep = session.Prepare("delete from config where config_type = ?");
            var statement = prep.Bind(type);
            session.Execute(statement);

            return Ok();
        }

        // DELETE: api/Config/source/Data.XML
        [HttpDelete("{type}/{name}")]
        public IActionResult Delete(string type, string name)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).Build();
            Cassandra.ISession session = cluster.Connect("appleseed_search_engines");

            var check = session.Prepare("select * from config where config_type = ? and config_name = ?");
            var checkStatement = check.Bind(type, name);
            var checkResults = session.Execute(checkStatement);
            var results = checkResults.GetRows().ToList();

            if (results.Count() == 0)
            {
                return BadRequest();
            }

            var prep = session.Prepare("delete from config where config_type = ? and config_name = ?");
            var statement = prep.Bind(type, name);
            session.Execute(statement);

            return Ok();
        }

        }
    }
}
