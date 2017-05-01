namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System;
    using System.Web;

    public class AlchemyAPI_KeywordParams : AlchemyAPI_BaseParams
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

		public enum KeywordExtractMode
		{
			NONE,
			NORMAL,
			STRICT
		}

		private int maxRetrieve = -1;
		private SourceTextMode sourceText;
		private TBOOL showSourceText;
		private string cQuery;
		private string xPath;
		private string baseUrl;
		private KeywordExtractMode keywordExtractMode;
		private TBOOL sentiment;

		public KeywordExtractMode getKeywordExtractMode()
		{
			return this.keywordExtractMode;
		}

		public void setKeywordExtractMode(KeywordExtractMode keywordExtractMode)
		{
			this.keywordExtractMode = keywordExtractMode;
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
			if (this.sentiment != TBOOL.NONE) retString += "&sentiment=" + (this.sentiment==TBOOL.TRUE ? "1" : "0");
			if (this.cQuery != null) retString += "&cquery=" + HttpUtility.UrlEncode(this.cQuery);
			if (this.xPath != null) retString += "&xpath=" + HttpUtility.UrlEncode(this.xPath);
			if (this.maxRetrieve > -1) retString+="&maxRetrieve="+this.maxRetrieve;
			if (this.baseUrl != null) retString += "&baseUrl=" + HttpUtility.UrlEncode(this.baseUrl);
			if (this.keywordExtractMode != KeywordExtractMode.NONE) retString += "&keywordExtractMode=" + (KeywordExtractMode.STRICT==this.keywordExtractMode ? "strict" : "normal");

			return retString;
		}
	}
}
