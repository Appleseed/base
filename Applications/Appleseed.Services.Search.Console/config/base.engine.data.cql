

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'queue', 'Base.Queue',
	{ 'queue_aggregate' : {'connectionString' : '',
					   'queueName' : 'Aggregate',
					   'type' : 'InMemory'
				},
	  'queue_collect' : {'connectionString' : '',
					   'queueName' : 'Collect',
					   'type' : 'Redis'
				},
	  'queue_extract' : {'connectionString' : '',
					   'queueName' : 'Extract',
					   'type' : 'Lucene'
				},
	  'queue_illuminate' : {'connectionString' : '',
					   'queueName' : 'Illuminate',
					   'type' : 'Cassandra'
				},
	  'queue_organize' : {'connectionString' : '',
					   'queueName' : 'Organize',
					   'type' : 'RabbitMQ'
				},
	  'queue_index' : {'connectionString' : '',
					   'queueName' : 'Index',
					   'type' : 'MySQL'
				}
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'graph', 'Search.Organize',
	{ 'graphDB1' : {  'location' : 'C:\Projects\Anant\graphDB1', 
					  'type' : 'neo4j',
					  'url' : 'http://localhost:7474/db/data'
				},
	  'nosqlDB1' : {  'location' : 'C:\Projects\Anant\nosqlDB1', 
					  'type' : 'RavenDB',
					  'url' : 'http://localhost:27017'
				}
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'processor', 'Search.Illuminate',
	{ 'Alchemy1' : {  'type' : 'Alchemy', 
					  'apiKey' : 'XXXXXX'
				   }
				   ,
	  'OpeanCalais' : {  'type' : 'OpeanCalais', 
					  'apiKey' : 'XXXXXX',
					  'apiSecret' : 'XXXXXX'
				   },
	
	  'StanfordNLP' : {  'type' : 'StanfordNLP'
				    }
	}
);


insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'index', 'Search.Index',
	{ 'indexalias1' : {	'location' : 'C:\Services\Appleseed.Search\Data\public', 
						'type' : 'Lucene.NET',
						'collectionName' : ''
					  },
	  'indexalias2' : {	'location' : 'C:\Services\Appleseed.Search\Data\private', 
						'type' : 'Lucene.NET',
						'collectionName' : ''
					  },
      'indexalias3' : {	'location' : 'C:\Services\Appleseed.Search\Data\konotree.default.com', 
						'type' : 'Lucene.NET',
						'collectionName' : ''
					  },
      'indexalias4' : {	'location' : 'http://localhost:9200/collectionname', 
						'type' : 'ElasticSearch',
						'collectionName' : ''
					  },
	  'indexalias5' : {	'location' : 'http://localhost:8983/solr/appleseed-public', 
						'type' : 'Solr',
						'collectionName' : 'appleseed-public'
					  }
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'Web.Site.RSS.XML',
	{ 'Blog' : { 'siteMapUrl' : 'https://blog.domain.us/feed',
				 'indexPath' : '{indexalias5}'
			   }
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'Web.Site.Atom.XML',
	{ 'Blog' : { 'siteMapUrl' : 'https://blog.domain.us/feed',
				 'indexPath' : '{indexalias5}'
			   }
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'Web.Site.Crawl',
	{ 'Webcrawl' : { 'siteMapUrl' : 'http://domain.co', 
					 'crawlDepth' : '100',
					 'indexPath' : '{indexalias1}',
					 'connectionString' : 'server=localhost;Trusted_Connection=yes;database=DATABASE',
					 'tableName' : 'as_Cache_BaseItemCollection'
				   }
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'Web.Site.Sitemap.XML',
	{ 'My Site' : { 'siteMapUrl' : 'http://domain.us/sitemap.axd', 
				    'indexPath' : '{indexalias5}'
				  }
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'processor', 'Portal.Content.Tagger',
	{ 'Tags' : { 'url' : 'http://url.com', 
				 'portalId' : '0',
				 'connectionString' : 'server=localhost;Trusted_Connection=yes;database=DATABASE',
				 'tableName' : 'as_Cache_BaseItemCollection'
			   }
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'Web.API.Crawler.CrunchBase',
	{ 'CrunchBase' : {'apiUrl' : 'http://api.crunchbase.com/v/2/', 
					  'apiKey' : 'XXXXXX',
					  'indexPath' : '{indexalias1}',
					  'connectionString' : 'server=localhost;Trusted_Connection=yes;database=DATABASE',
					  'tableName' : 'as_Cache_BaseItemCollection',
					  'itemsCollected' : '33'
					 }
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'Data.CSV',
	{ 'Contacts' : {'indexPath' : '{indexalias1}', 
				 'location' : 'C:\data\locationof\contacts.csv'
				}
	}
);


insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'Data.XML',
	{ 'Contacts' : {'indexPath' : '{indexalias1}', 
				 'location' : 'C:\data\locationof\contacts.xml'
				}
	}
);


insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'Data.MSSQL',
	{ 'Contacts' : {'indexPath' : '{indexalias1}', 
				 'connectionString' : 'server=localhost;Trusted_Connection=yes;database=DATABASE',
				 'query' : 'Select * from Table'
				}
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'File.Local',
	{ 'Files' : {'indexPath' : '{indexalias1}', 
				 'connectionString' : 'server=localhost;Trusted_Connection=yes;database=DATABASE',
				 'tableName' : 'as_Cache_BaseItemCollection'
				}
	}
);

insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'File.Cloud.DropBox',
	{ 'DropBox' : {'appkey' : 'XXXXXX', 
				   'appSecret' : 'XXXXXX',
				   'token' : 'XXXXXX',
				   'tokenSecret' : 'XXXXXX',
				   'filesPath' : 'C:\Projects\Anant\KonoTree_Files',
				   'connectionString' : 'server=localhost;Trusted_Connection=yes;database=DATABASE',
				   'queueName' : '{queuealias1}',
				   'userAgent' : '{userAgent1}'
				  }
	}
);


insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'File.Cloud.AmazonS3',
	{ 'Amazon' : {'appkey' : 'XXXXXX', 
				   'appSecret' : 'XXXXXX',
				   'token' : 'XXXXXX',
				   'tokenSecret' : 'XXXXXX',
				   'filesPath' : 'C:\Projects\Anant\KonoTree_Files',
				   'connectionString' : 'server=localhost;Trusted_Connection=yes;database=DATABASE',
				   'queueName' : '{queuealias1}',
				   'userAgent' : '{userAgent1}'
				  }
	}
);



insert into appleseed_search_engines.config (config_type, config_name, config_values) values (
	'source', 'Web.API.CRM.Nutshell',
	{ 'Nutshell' : {'apiUrl' : 'https://app.nutshell.com/api/v1/json',
					'apiKey' : 'XXXXXX',
					'apiUserEmail' : 'EMAIL@EMAIL.COM',
					'indexPath' : '{indexalias1}',
					'connectionString' : 'server=.\SQLEXPRESS;Trusted_Connection=yes;database=DATABASE',
					'tableName' : 'as_Cache_BaseItemCollection'
				   }
	}
);