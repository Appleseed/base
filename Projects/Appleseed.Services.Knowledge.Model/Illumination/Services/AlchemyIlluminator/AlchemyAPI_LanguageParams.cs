namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System;
    using System.Web;

    public class AlchemyAPI_LanguageParams : AlchemyAPI_BaseParams
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

		private SourceTextMode sourceText;
		private string cQuery;
		private string xPath;
		
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
			if (this.cQuery != null) retString += "&cquery=" + HttpUtility.UrlEncode(this.cQuery);
			if (this.xPath != null) retString += "&xpath=" + HttpUtility.UrlEncode(this.xPath);

			return retString;
		}
	}
}
