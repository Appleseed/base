namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;

    public class AlchemyAPI
{
	private string _apiKey;
	private string _requestUri;

	public AlchemyAPI()
	{
		this._requestUri = "http://access.alchemyapi.com/calls/";
	}

	public void SetAPIHost(string apiHost)
	{
		if (apiHost.Length < 2)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Error setting API host.");

			throw ex;
		}

		this._requestUri = "http://" + apiHost  +  ".alchemyapi.com/calls/";
	}

	public void SetAPIKey(string apiKey)
	{
		this._apiKey = apiKey;

		if (this._apiKey.Length < 5)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Error setting API key.");

			throw ex;
		}
	}

	public void LoadAPIKey(string filename)
	{
		StreamReader reader;

		reader = File.OpenText(filename);

		string line = reader.ReadLine();

		reader.Close();

		this._apiKey = line.Trim();

		if (this._apiKey.Length < 5)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Error loading API key.");

			throw ex;
		}
	}
		
	public string URLGetAuthor(string url)
	{
		this.CheckURL(url);
			
		return this.URLGetAuthor(url, new AlchemyAPI_BaseParams());
	}
		
	public string URLGetAuthor(string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);
			
		return this.GET("URLGetAuthor", "url", parameters);
	}			
		
	public string HTMLGetAuthor(string html,string url)
	{
		this.CheckHTML(html, url);
			
		return this.HTMLGetAuthor(html, url, new AlchemyAPI_BaseParams());
	}
		
	public string HTMLGetAuthor(string html, string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);
			
		return this.POST("HTMLGetAuthor", "html", parameters);
	}	

	public string URLGetRankedNamedEntities(string url)
	{
		this.CheckURL(url);

		return this.URLGetRankedNamedEntities(url, new AlchemyAPI_EntityParams());
	}

	public string URLGetRankedNamedEntities(string url, AlchemyAPI_EntityParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);
	
		return this.GET("URLGetRankedNamedEntities", "url", parameters);
	}

	public string HTMLGetRankedNamedEntities(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetRankedNamedEntities(html, url, new AlchemyAPI_EntityParams());
	}

	
	public string HTMLGetRankedNamedEntities(string html, string url, AlchemyAPI_EntityParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetRankedNamedEntities", "html", parameters);
	}

	public string TextGetRankedNamedEntities(string text)
	{
		this.CheckText(text);

		return this.TextGetRankedNamedEntities(text,new AlchemyAPI_EntityParams());
	}

	public string TextGetRankedNamedEntities(string text, AlchemyAPI_EntityParams parameters)
	{
		this.CheckText(text);
		parameters.setText(text);

		return this.POST("TextGetRankedNamedEntities", "text", parameters);
	}
 
	public string URLGetRankedConcepts(string url)
	{
		this.CheckURL(url);

		return this.URLGetRankedConcepts(url, new AlchemyAPI_ConceptParams());
	}

	public string URLGetRankedConcepts(string url, AlchemyAPI_ConceptParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetRankedConcepts", "url", parameters);
	}

	public string HTMLGetRankedConcepts(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetRankedConcepts(html, url, new AlchemyAPI_ConceptParams());
	}

	public string HTMLGetRankedConcepts(string html, string url, AlchemyAPI_ConceptParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetRankedConcepts", "html", parameters);
	}

	public string TextGetRankedConcepts(string text)
	{
		this.CheckText(text);

		return this.TextGetRankedConcepts(text,new AlchemyAPI_ConceptParams());
	}

	public string TextGetRankedConcepts(string text, AlchemyAPI_ConceptParams parameters)
	{
		this.CheckText(text);
		parameters.setText(text);

		return this.POST("TextGetRankedConcepts", "text", parameters);
	}

	public string URLGetRankedKeywords(string url)
	{
		this.CheckURL(url);

		return this.URLGetRankedKeywords(url, new AlchemyAPI_KeywordParams());
	}

	public string URLGetRankedKeywords(string url, AlchemyAPI_KeywordParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetRankedKeywords", "url", parameters);
	}

	public string HTMLGetRankedKeywords(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetRankedKeywords(html, url, new AlchemyAPI_KeywordParams());
	}

	public string HTMLGetRankedKeywords(string html, string url, AlchemyAPI_KeywordParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetRankedKeywords", "html", parameters);
	}

	public string TextGetRankedKeywords(string text)
	{
		this.CheckText(text);

		return this.TextGetRankedKeywords(text,new AlchemyAPI_KeywordParams());
	}

	public string TextGetRankedKeywords(string text, AlchemyAPI_KeywordParams parameters)
	{
		this.CheckText(text);
		parameters.setText(text);

		return this.POST("TextGetRankedKeywords", "text", parameters);
	}

	public string URLGetLanguage(string url)
	{
		this.CheckURL(url);

		return this.URLGetLanguage(url,new AlchemyAPI_LanguageParams());
	}

	public string URLGetLanguage(string url, AlchemyAPI_LanguageParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetLanguage", "url", parameters);
	}

	public string HTMLGetLanguage(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetLanguage(html,url,new AlchemyAPI_LanguageParams());
	}

	public string HTMLGetLanguage(string html, string url, AlchemyAPI_LanguageParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetLanguage", "html", parameters);
	}

	public string TextGetLanguage(string text)
	{
		this.CheckText(text);

		return this.TextGetLanguage(text, new AlchemyAPI_LanguageParams());
	}

	public string TextGetLanguage(string text, AlchemyAPI_LanguageParams parameters)
	{
		this.CheckText(text);
		parameters.setText(text);

		return this.POST("TextGetLanguage", "text", parameters);
	}
	
	public string URLGetCategory(string url)
	{
		this.CheckURL(url);

		return this.URLGetCategory(url, new AlchemyAPI_CategoryParams() );
	}

	public string URLGetCategory(string url, AlchemyAPI_CategoryParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetCategory", "url", parameters);
	}

	public string HTMLGetCategory(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetCategory(html, url, new AlchemyAPI_CategoryParams());
	}

	public string HTMLGetCategory(string html, string url, AlchemyAPI_CategoryParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetCategory", "html", parameters);
	}

	public string TextGetCategory(string text)
	{
		this.CheckText(text);

		return this.TextGetCategory(text, new AlchemyAPI_CategoryParams());
	}

	public string TextGetCategory(string text, AlchemyAPI_CategoryParams parameters)
	{
		this.CheckText(text);
		parameters.setText(text);

		return this.POST("TextGetCategory", "text", parameters);
	}

	public string URLGetText(string url)
	{
		this.CheckURL(url);

		return this.URLGetText(url, new AlchemyAPI_TextParams());
	}

	public string URLGetText(string url, AlchemyAPI_TextParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetText", "url", parameters);
	}

	public string HTMLGetText(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetText(html,url, new AlchemyAPI_TextParams());
	}

	public string HTMLGetText(string html, string url,AlchemyAPI_TextParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetText", "html", parameters);
	}

	public string URLGetRawText(string url)
	{
		this.CheckURL(url);

		return this.URLGetRawText(url, new AlchemyAPI_BaseParams());
	}

	public string URLGetRawText(string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetRawText", "url", parameters);
	}

	public string HTMLGetRawText(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetRawText(html, url, new AlchemyAPI_BaseParams());
	}

	public string HTMLGetRawText(string html, string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetRawText", "html", parameters);
	}

	public string URLGetTitle(string url)
	{
		this.CheckURL(url);

		return this.URLGetTitle(url, new AlchemyAPI_BaseParams());
	}

	public string URLGetTitle(string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetTitle", "url", parameters);
	}

	public string HTMLGetTitle(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetTitle(html, url, new AlchemyAPI_BaseParams());
	}

	public string HTMLGetTitle(string html, string url, AlchemyAPI_BaseParams parameters)
	
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetTitle", "html", parameters);
	}
	
	public string URLGetFeedLinks(string url)
	{
		this.CheckURL(url);

		return this.URLGetFeedLinks(url, new AlchemyAPI_BaseParams());
	}

	public string URLGetFeedLinks(string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetFeedLinks", "url", parameters);
	}

	public string HTMLGetFeedLinks(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetFeedLinks(html,url, new AlchemyAPI_BaseParams());
	}

	public string HTMLGetFeedLinks(string html, string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetFeedLinks", "html", parameters);
	}
	
	public string URLGetMicroformats(string url)
	{
		this.CheckURL(url);

		return this.URLGetMicroformats(url, new AlchemyAPI_BaseParams());
	}

	public string URLGetMicroformats(string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetMicroformatData", "url", parameters);
	}

	public string HTMLGetMicroformats(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetMicroformats(html,url, new AlchemyAPI_BaseParams());
	}

	public string HTMLGetMicroformats(string html, string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetMicroformatData", "html", parameters);
	}

	public string URLGetConstraintQuery(string url, string query)
	{
		this.CheckURL(url);
		if (query.Length < 2)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Invalid constraint query specified.");

			throw ex;
		}

		AlchemyAPI_ConstraintQueryParams cqParams = new AlchemyAPI_ConstraintQueryParams();
		cqParams.setCQuery(query);

		return this.URLGetConstraintQuery(url,cqParams);
	}

	public string URLGetConstraintQuery(string url, AlchemyAPI_ConstraintQueryParams parameters)
	{
		this.CheckURL(url);
		if (parameters.getCQuery().Length < 2)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Invalid constraint query specified.");

			throw ex;
		}
	
		parameters.setUrl(url);

		return this.POST("URLGetConstraintQuery", "url", parameters);
	}

	public string HTMLGetConstraintQuery(string html, string url, string query)
	{
		this.CheckHTML(html, url);
		if (query.Length < 2)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Invalid constraint query specified.");

			throw ex;
		}

		AlchemyAPI_ConstraintQueryParams cqParams = new AlchemyAPI_ConstraintQueryParams();
		cqParams.setCQuery(query);

		return this.HTMLGetConstraintQuery(html, url, cqParams);
	}

	public string HTMLGetConstraintQuery(string html, string url, AlchemyAPI_ConstraintQueryParams parameters)
	{
		this.CheckHTML(html, url);
		if (parameters.getCQuery().Length < 2)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Invalid constraint query specified.");

			throw ex;
		}

		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetConstraintQuery", "html", parameters);
	}
	
	public string URLGetTextSentiment(string url)
	{
		this.CheckURL(url);

		return this.URLGetTextSentiment(url, new AlchemyAPI_BaseParams());
	}

	public string URLGetTextSentiment(string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetTextSentiment", "url", parameters);
	}

	public string HTMLGetTextSentiment(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetTextSentiment(html, url, new AlchemyAPI_BaseParams());
	}

	public string HTMLGetTextSentiment(string html, string url, AlchemyAPI_BaseParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetTextSentiment", "html", parameters);
	}

	public string TextGetTextSentiment(string text)
	{
		this.CheckText(text);

		return this.TextGetTextSentiment(text,new AlchemyAPI_BaseParams());
	}

	public string TextGetTextSentiment(string text, AlchemyAPI_BaseParams parameters)
	{
		this.CheckText(text);
		parameters.setText(text);

		return this.POST("TextGetTextSentiment", "text", parameters);
	}
	//------
	public string URLGetTargetedSentiment(string url, string target)
	{
		this.CheckURL(url);
		this.CheckText(target);
			
		return this.URLGetTargetedSentiment(url, target, new AlchemyAPI_TargetedSentimentParams());
	}
		
	public string URLGetTargetedSentiment(string url, string target, AlchemyAPI_TargetedSentimentParams parameters)
	{
		this.CheckURL(url);
		this.CheckText(target);
			
		parameters.setUrl(url);
		parameters.setTarget(target);
			
		return this.GET("URLGetTargetedSentiment", "url", parameters);
	}
		
	public string HTMLGetTargetedSentiment(string html, string url, string target)
	{
		this.CheckHTML(html, url);
		this.CheckText(target);
			
		return this.HTMLGetTargetedSentiment(html, url, target, new AlchemyAPI_TargetedSentimentParams());
	}
		
	public string HTMLGetTargetedSentiment(string html, string url, string target, AlchemyAPI_TargetedSentimentParams parameters)
	{
		
		this.CheckHTML(html, url);
		this.CheckText(target);
			
		parameters.setHtml(html);
		parameters.setUrl(url);
		parameters.setTarget(target);
			
		return this.POST("HTMLGetTargetedSentiment", "html", parameters);
	}
	
	public string TextGetTargetedSentiment(string text, string target)
	{
		this.CheckText(text);
		this.CheckText(target);
			
		return this.TextGetTargetedSentiment(text, target, new AlchemyAPI_TargetedSentimentParams());
	}
		
	public string TextGetTargetedSentiment(string text, string target, AlchemyAPI_TargetedSentimentParams parameters)
	{
		this.CheckText(text);
		this.CheckText(target);
			
		parameters.setText(text);
		parameters.setTarget(target);
			
		return this.POST("TextGetTargetedSentiment", "text", parameters);
	}
	//------

	public string URLGetRelations(string url)
	{
		this.CheckURL(url);

		return this.URLGetRelations(url, new AlchemyAPI_RelationParams());
	}

	public string URLGetRelations(string url, AlchemyAPI_RelationParams parameters)
	{
		this.CheckURL(url);
		parameters.setUrl(url);

		return this.GET("URLGetRelations", "url", parameters);
	}

	public string HTMLGetRelations(string html, string url)
	{
		this.CheckHTML(html, url);

		return this.HTMLGetRelations(html, url, new AlchemyAPI_RelationParams());
	}

	public string HTMLGetRelations(string html, string url, AlchemyAPI_RelationParams parameters)
	{
		this.CheckHTML(html, url);
		parameters.setHtml(html);
		parameters.setUrl(url);

		return this.POST("HTMLGetRelations", "html", parameters);
	}

	public string TextGetRelations(string text)
	{
		this.CheckText(text);

		return this.TextGetRelations(text,new AlchemyAPI_RelationParams());
	}

	public string TextGetRelations(string text, AlchemyAPI_RelationParams parameters)
	{
		this.CheckText(text);
		parameters.setText(text);

		return this.POST("TextGetRelations", "text", parameters);
	}

	private void CheckHTML(string html, string url)
	{
		if (html.Length < 10)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Enter a HTML document to analyze.");

			throw ex;
		}

		if (url.Length < 10)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Enter a web URL to analyze.");

			throw ex;
		}
	}

	private void CheckText(string text)
	{
		if (text.Length < 5)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Enter some text to analyze.");

			throw ex;
		}
	}

	private void CheckURL(string url)
	{
		if (url.Length < 10)
		{
			System.ApplicationException ex =
				new System.ApplicationException("Enter a web URL to analyze.");

			throw ex;
		}
	}

	private string GET(string callName, string callPrefix, AlchemyAPI_BaseParams parameters)
	{ // callMethod, callPrefix, ... params
		StringBuilder uri = new StringBuilder();
		uri.Append(this._requestUri).Append(callPrefix).Append("/").Append(callName);
		uri.Append("?apikey=").Append(this._apiKey).Append(parameters.getParameterString());

		parameters.resetBaseParams();

		Uri address = new Uri(uri.ToString());
		HttpWebRequest wreq = WebRequest.Create(address) as HttpWebRequest;
		wreq.Proxy = GlobalProxySelection.GetEmptyWebProxy();
		wreq.Method = "GET";

		return this.DoRequest(wreq, parameters.getOutputMode());
	}

	private string POST(string callName, string callPrefix, AlchemyAPI_BaseParams parameters)
	{ // callMethod, callPrefix, ... params
		Uri address = new Uri(this._requestUri + callPrefix + "/" + callName);

		HttpWebRequest wreq = WebRequest.Create(address) as HttpWebRequest;
		wreq.Proxy = GlobalProxySelection.GetEmptyWebProxy();
		wreq.Method = "POST";
		wreq.ContentType = "application/x-www-form-urlencoded";

		StringBuilder d = new StringBuilder();
		d.Append("apikey=").Append(this._apiKey).Append(parameters.getParameterString());

		parameters.resetBaseParams();

		byte[] bd = UTF8Encoding.UTF8.GetBytes(d.ToString());

		wreq.ContentLength = bd.Length;
		using (Stream ps = wreq.GetRequestStream()) { ps.Write(bd, 0, bd.Length); }

		return this.DoRequest(wreq, parameters.getOutputMode());
	}

	private string DoRequest(HttpWebRequest wreq, AlchemyAPI_BaseParams.OutputMode outputMode)
	{
		using (HttpWebResponse wres = wreq.GetResponse() as HttpWebResponse)
		{
			StreamReader r = new StreamReader(wres.GetResponseStream());

			string xml = r.ReadToEnd();

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);

			XmlElement root = xmlDoc.DocumentElement;

			if (AlchemyAPI_BaseParams.OutputMode.XML == outputMode)
			{
				XmlNode status = root.SelectSingleNode("/results/status");

				if (status.InnerText != "OK")
				{
                    var statusInfo = root.SelectSingleNode("/results/statusInfo");
                    throw new System.ApplicationException("Error making API call: " + statusInfo.InnerText);
				}
			}
			else if (AlchemyAPI_BaseParams.OutputMode.RDF == outputMode)
			{
				XmlNamespaceManager nm = new XmlNamespaceManager(xmlDoc.NameTable);
				nm.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
				nm.AddNamespace("aapi", "http://rdf.alchemyapi.com/rdf/v1/s/aapi-schema#");
				XmlNode status = root.SelectSingleNode("/rdf:RDF/rdf:Description/aapi:ResultStatus",nm);

				if (status.InnerText != "OK")
				{
                    var statusInfo = root.SelectSingleNode("/results/statusInfo");
                    throw new System.ApplicationException("Error making API call: " + statusInfo.InnerText);
				}
			}

			return xml;
		
		}
	}
}

}

