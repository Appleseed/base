namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System;
    using System.Web;

    public class AlchemyAPI_ConceptParams : AlchemyAPI_BaseParams
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

		private int maxRetrieve = -1;
		private SourceTextMode sourceText;
		private TBOOL showSourceText;
		private TBOOL linkedData;
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
			if (this.linkedData != TBOOL.NONE) retString += "&linkedData=" + (this.linkedData == TBOOL.TRUE ? "1" : "0");
			if (this.cQuery != null) retString += "&cquery=" + HttpUtility.UrlEncode(this.cQuery);
			if (this.xPath != null) retString += "&xpath=" + HttpUtility.UrlEncode(this.xPath);
			if (this.maxRetrieve > -1) retString+="&maxRetrieve="+this.maxRetrieve;

			return retString;
		}
	}
}
