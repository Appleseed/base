namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using Abot.Crawler;
    using Abot.Poco;
    using System.Net;
    using Common.Logging;
	using Newtonsoft.Json.Linq;
	using System.Collections.Generic;

    using Appleseed.Services.Core.Helpers;
    using Appleseed.Services.Base.Model;


	// Input URI should be "http://api.crunchbase.com/v/2/"
    public class CrunchBaseApiCrawlerAggregator : IAggregateData
    {
		private readonly Common.Logging.ILog logger;
		private readonly string api_key;
		private readonly string uri;
		private readonly string connectionString;
		private readonly string tableName;
        private readonly Int32 itemsCollected;

		public CrunchBaseApiCrawlerAggregator(ILog logger,string api_key,string uri,string connectionString,string tableName, string itemsCollected)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
			if (api_key == null)
			{
				throw new ArgumentNullException("api_key");
			}
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (connectionString == null)
			{
				throw new ArgumentNullException("connectionString");
			}
            if(itemsCollected == null)
            {
                throw new ArgumentNullException("itemsCollected");
            }
			this.logger = logger;
			this.api_key = api_key;
			this.uri = uri;
			this.connectionString = connectionString;
			this.tableName = tableName;
            this.itemsCollected = Int32.Parse(itemsCollected);
            this.logger.Info("----------------CONFIGURING CRUNCHBASEAPICRAWLER--------------------------------");
        }

        public void Aggregate()
        {
            this.logger.InfoFormat("----------------STARTING CRUNCHBASEAPICRAWLER.AGGREGATE--------------------------- {0}", 1);
            
			//Get Json Big String 
			string organizations = CrunchBaseGetOrganizations(api_key,uri);     
			string people = CrunchBaseGetPeople(api_key, uri);     
			string products = CrunchBaseGetOrganizations(api_key, uri);  

			// Change big String into JObject List
			List<JObject> organizationsList = CrunchBaseCollectOrganization(organizations);
			List<JObject> peopleList = CrunchBaseCollectPeople(organizations);
			List<JObject> productList = CrunchBaseCollectProduct(organizations);

			// Change elements in list to Object, Object models need define?
            int index = 1;
			foreach (var organization in organizationsList) {
                if (itemsCollected != 0 && index > itemsCollected) { Console.WriteLine("break"); break; }

                var itemId = index;
                index+=1;
				var itemName = organization["name"];
                var itemKey = organization["path"];
				var itemPath = uri+organization["path"]+"?user_key="+api_key;
				var itemType = organization["type"];

				// TODO: Insert into Database

				var newItem = new AppleseedModuleItem()
				{
					Key = itemKey.ToString(), 
					Path = itemPath.ToString(),
					ModuleID = Int32.Parse(itemId.ToString()),
					PageID = 0,
					PortalID = 0,
					ViewRoles = CollectionItemSecurityType.PUBLIC,
					Type = CollectionItemType.CB_ORG,
                    //TODO
					FileSize = 0,
					Name = itemName.ToString()
				};


				//TODO: add this into the DB using AggregateItem() 

				this.AggregateItem(newItem);
				this.logger.InfoFormat("CrunchBase.API.AGGREGATE.organization Added {0} into Cache Collection", organization.ToString());
			}

            index = 1;

			foreach (var person in peopleList) {

                if (itemsCollected != 0 && index > itemsCollected) { Console.WriteLine("break"); break; }

                var itemId = index;
                index += 1;
				var itemName = person["name"];
                var itemKey = person["path"];
                var itemPath = uri + person["path"] + "?user_key=" + api_key;
				var itemType = person["type"];

				// TODO: Insert into Database

				var newItem = new AppleseedModuleItem()
				{
                    Key = itemKey.ToString(), 
					Path = itemPath.ToString(),
					ModuleID = Int32.Parse(itemId.ToString()),
					PageID = 0,
					PortalID = 0,
					ViewRoles = CollectionItemSecurityType.PUBLIC,
					Type = CollectionItemType.CB_PER,
					FileSize = 0,
					Name = itemName.ToString()
				};


				//TODO: add this into the DB using AggregateItem() 

				this.AggregateItem(newItem);
				this.logger.InfoFormat("CrunchBase.API.AGGREGATE.person Added {0} into Cache Collection", person.ToString());

			}

            index = 1;
			foreach (var product in productList) {

                if (itemsCollected != 0 && index > itemsCollected) { Console.WriteLine("break"); break; }

                var itemId = index;
                index += 1;
				var itemName = product["name"];
                var itemKey = product["path"];
                var itemPath = uri + product["path"] + "?user_key=" + api_key;
				var itemType = product["type"];

				// TODO: Insert into Database

				var newItem = new AppleseedModuleItem()
				{
                    Key = itemKey.ToString(), 
					Path = itemPath.ToString(),
					ModuleID = Int32.Parse(itemId.ToString()),
					PageID = 0,
					PortalID = 0,
					ViewRoles = CollectionItemSecurityType.PUBLIC,
					Type = CollectionItemType.CB_PRO,
					FileSize = 0,
					Name = itemName.ToString()
				};


				//TODO: add this into the DB using AggregateItem() 

				this.AggregateItem(newItem);
				this.logger.InfoFormat("CrunchBase.API.AGGREGATE.product Added {0} into Cache Collection", product.ToString());

			}

            this.logger.Info("----------------ENDING CRUNCHBASEAPICRAWLER.AGGREGATE--------------------------------");
        }

		public void AggregateItem(AppleseedModuleItem item)
		{
			//refactored to core helpers because it will be used everywhere 
			Appleseed.Services.Core.Helpers.CollectionHelper.AggregateItem(item, this.connectionString, this.tableName);
		}
			
		public string CrunchBaseGetOrganizations(string user_key,string uri){
			var url = uri+"organizations?user_key=" + user_key;


			using (var wb = new WebClient())
				return wb.DownloadString(url).ToString();
		}

		public string CrunchBaseGetPeople(string user_key,string uri){
			var url = uri+"people?user_key=" + user_key;


			using (var wb = new WebClient())
				return wb.DownloadString(url).ToString();
		}

		public string CrunchBaseGetProducts(string user_key,string uri){
			var url = uri+"products?user_key=" + user_key;

			using (var wb = new WebClient())
				return wb.DownloadString(url).ToString();
		}

		public List<JObject> CrunchBaseCollectOrganization(string organizationString){
			List<JObject> returnList=new List<JObject>();
			JObject organizations=JObject.Parse(organizationString);
			foreach(JObject org in organizations["data"]["items"])
				returnList.Add(org);
			return returnList;	
		}

        public List<JObject> CrunchBaseCollectPeople(string peopleString)
        {
			List<JObject> returnList=new List<JObject>();
			JObject people=JObject.Parse(peopleString);
			foreach(JObject person in people["data"]["items"])
				returnList.Add(person);
			return returnList;	
		}

        public List<JObject> CrunchBaseCollectProduct(string productString)
        {
			List<JObject> returnList=new List<JObject>();
			JObject products=JObject.Parse(productString);
			foreach(JObject prod in products["data"]["items"])
				returnList.Add(prod);
			return returnList;	
		}
    }
}
