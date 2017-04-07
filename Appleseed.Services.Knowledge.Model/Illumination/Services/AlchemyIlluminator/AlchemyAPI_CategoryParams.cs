namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System.Web;

    public class AlchemyAPI_CategoryParams : AlchemyAPI_BaseParams
	{
		public enum SourceTextMode
		{
			NONE,
			CLEANED_OR_RAW,
			CQUERY,
			XPATH
		}
	
		private SourceTextMode sourceText;
		private string cQuery;
		private string xPath;
		private string baseUrl;

		public SourceTextMode getSourceText()
		{
			return this.sourceText;
		}

		public void setSourceText(SourceTextMode sourceText)
		{
			this.sourceText = sourceText;
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

		public string getBaseUrl()
		{
			return this.baseUrl;
		}

		public void setBaseUrl(string baseUrl)
		{
			this.baseUrl = baseUrl;
		}
	
		override public string getParameterString()
		{
			string retstring = base.getParameterString();

			if (this.sourceText != SourceTextMode.NONE)
			{
				if( this.sourceText == SourceTextMode.CLEANED_OR_RAW )
					retstring += "&sourceText=cleaned_or_raw";
				else if( this.sourceText == SourceTextMode.CQUERY )
					 retstring += "&sourceText=cquery";
				else if( this.sourceText == SourceTextMode.CQUERY )
					 retstring += "&sourceText=xpath";
			}
			if (this.cQuery != null) retstring += "&cquery=" + HttpUtility.UrlEncode(this.cQuery);
			if (this.xPath != null) retstring += "&xpath=" + HttpUtility.UrlEncode(this.xPath);
			if (this.baseUrl != null) retstring += "&baseUrl=" + HttpUtility.UrlEncode(this.baseUrl);
		   
			return retstring;
		}
	}
}
