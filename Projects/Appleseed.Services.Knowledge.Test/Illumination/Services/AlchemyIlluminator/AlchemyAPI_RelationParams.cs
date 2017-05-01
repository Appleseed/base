namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System;
    using System.Web;

    public class AlchemyAPI_RelationParams : AlchemyAPI_BaseParams
	{
		public enum SourceTextMode
		{
			NONE,
			CLEANED_OR_RAW,
			CLEANED,
			RAW,
			CQUERY,
			XPATH
		}

		private enum TBOOL
		{
			NONE,
			FALSE,
			TRUE
		}

		private TBOOL disambiguate;
		private TBOOL linkedData;
		private TBOOL coreference;
		private TBOOL entities;
		private TBOOL sentimentExcludeEntities;
		private TBOOL requireEntities;
		private int maxRetrieve = -1;
		private SourceTextMode sourceText;
		private TBOOL showSourceText;
		private string cQuery;
		private string xPath;
		private string baseUrl;
		private TBOOL sentiment;

		public bool isDisambiguate()
		{
			if (TBOOL.TRUE == this.disambiguate)
				return true;
			return false;
		}

		public void setDisambiguate(bool disambiguate)
		{
			if (disambiguate)
				this.disambiguate = TBOOL.TRUE;
			else
				this.disambiguate = TBOOL.FALSE;
		}

		public bool isLinkedData()
		{
			if (TBOOL.TRUE == this.linkedData)
				return true;
			return false;
		}

		public void setLinkedData(bool linkedData)
		{
			if (linkedData)
				this.linkedData = TBOOL.TRUE;
			else
				this.linkedData = TBOOL.FALSE;
		}

		public bool isCoreference()
		{
			if (TBOOL.TRUE == this.coreference)
				return true;
			return false;
		}

		public void setCoreference(bool coreference)
		{
			if (coreference)
				this.coreference = TBOOL.TRUE;
			else
				this.coreference = TBOOL.FALSE;
		}
	
		public SourceTextMode getSourceText()
		{
			return this.sourceText;
		}
		
		public void setSourceText(SourceTextMode sourceText)
		{
			this.sourceText = sourceText;
		}
		
		public bool isShowSourceText()
		{
			if (TBOOL.TRUE == this.showSourceText)
				return true;
			return false;
		}
		
		public void setShowSourceText(bool showSourceText)
		{
			if (showSourceText)
				this.showSourceText = TBOOL.TRUE;
			else
				this.showSourceText = TBOOL.FALSE;
		}
		
		public bool isEntities()
		{
			if (TBOOL.TRUE == this.entities)
				return true;
			return false;
		}
		
		public void setEntities(bool entities)
		{
			if (entities)
				this.entities = TBOOL.TRUE;
			else
				this.entities = TBOOL.FALSE;
		}
		
		public bool isSentimentExcludeEntities()
		{
			if (TBOOL.TRUE == this.sentimentExcludeEntities)
				return true;
			return false;
		}
		
		public void setSentimentExcludeEntities(bool sentimentExcludeEntities)
		{
			if (sentimentExcludeEntities)
				this.sentimentExcludeEntities = TBOOL.TRUE;
			else
				this.sentimentExcludeEntities = TBOOL.FALSE;
		}
		
		public bool isRequireEntities()
		{
			if (TBOOL.TRUE == this.requireEntities)
				return true;
			return false;
		}
		
		public void setRequireEntities(bool requireEntities)
		{
			if (requireEntities)
				this.requireEntities = TBOOL.TRUE;
			else
				this.requireEntities = TBOOL.FALSE;
		}
		
		public string getCQuery()
		{
			return this.cQuery;
		}
		
		public void setCQuery(string cQuery)
		{
			this.cQuery = cQuery;
		}
		
		public string getXPath()
		{
			return this.xPath;
		}
		
		public void setXPath(string xPath)
		{
			this.xPath = xPath;
		}
		
		public int getMaxRetrieve()
		{
			return this.maxRetrieve;
		}
		
		public void setMaxRetrieve(int maxRetrieve)
		{
			this.maxRetrieve = maxRetrieve;
		}
		
		public bool isSentiment()
		{
			if (TBOOL.TRUE == this.sentiment)
				return true;
			return false;
		}
		
		public void setSentiment(bool sentiment)
		{
			if (sentiment)
				this.sentiment = TBOOL.TRUE;
			else
				this.sentiment = TBOOL.FALSE;
		}
		
		public string getBaseUrl()
		{
			return this.baseUrl;
		}
		
		public void setBaseUrl(string baseUrl)
		{
			this.baseUrl = baseUrl;
		}
		
		override public String getParameterString()
		{
			String retString = base.getParameterString();

			if (this.sourceText != SourceTextMode.NONE)
			{
				if (this.sourceText == SourceTextMode.CLEANED_OR_RAW)
					retString += "&sourceText=cleaned_or_raw";
				else if (this.sourceText == SourceTextMode.CLEANED)
					retString += "&sourceText=cleaned";
				else if (this.sourceText == SourceTextMode.RAW)
					retString += "&sourceText=raw";
				else if (this.sourceText == SourceTextMode.CQUERY)
					retString += "&sourceText=cquery";
				else if (this.sourceText == SourceTextMode.CQUERY)
					retString += "&sourceText=xpath";
			}
			if (this.showSourceText != TBOOL.NONE) retString += "&showSourceText=" + (this.showSourceText==TBOOL.TRUE ? "1" : "0");
			if (this.disambiguate != TBOOL.NONE) retString += "&disambiguate=" + (this.disambiguate == TBOOL.TRUE ? "1" : "0");
			if (this.linkedData != TBOOL.NONE) retString += "&linkedData=" + (this.linkedData == TBOOL.TRUE ? "1" : "0");
			if (this.coreference != TBOOL.NONE) retString += "&coreference=" + (this.coreference == TBOOL.TRUE ? "1" : "0");
			if (this.entities != TBOOL.NONE) retString += "&entities=" + (this.entities == TBOOL.TRUE ? "1" : "0");
			if (this.sentimentExcludeEntities != TBOOL.NONE) retString += "&sentimentExcludeEntities=" + (this.sentimentExcludeEntities == TBOOL.TRUE ? "1" : "0");
			if (this.requireEntities != TBOOL.NONE) retString += "&requireEntities=" + (this.requireEntities == TBOOL.TRUE ? "1" : "0");
			if (this.sentiment != TBOOL.NONE) retString += "&sentiment=" + (this.sentiment == TBOOL.TRUE ? "1" : "0");
			if (this.cQuery != null) retString += "&cquery=" + HttpUtility.UrlEncode(this.cQuery);
			if (this.xPath != null) retString += "&xpath=" + HttpUtility.UrlEncode(this.xPath);
			if (this.maxRetrieve>-1) retString+="&maxRetrieve="+this.maxRetrieve;
			if (this.baseUrl != null) retString += "&baseUrl=" + HttpUtility.UrlEncode(this.baseUrl);
		   
			return retString;
		}
	}
}
