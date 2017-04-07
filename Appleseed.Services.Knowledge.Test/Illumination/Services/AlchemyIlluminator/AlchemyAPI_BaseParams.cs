namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System;
    using System.Web;

    public class AlchemyAPI_BaseParams
	{
		public enum OutputMode
		{
			NONE,
			XML,
			RDF,
            JSON
		};

		private String url;
		private String html;
		private String text;
        private OutputMode outputMode = OutputMode.XML;
		private String customParameters;
	
		public String getUrl()
		{
			return this.url;
		}

		public void setUrl(String url)
		{
			this.url = url;
		}

		public String getHtml()
		{
			return this.html;
		}

		public void setHtml(String html)
		{
			this.html = html;
		}

		public String getText()
		{
			return this.text;
	   	}

		public void setText(String text)
		{
			this.text = text;
		}

		public OutputMode getOutputMode()
		{
			return this.outputMode;
		}

		public void setOutputMode(OutputMode outputMode)
		{
			this.outputMode = outputMode;
		}

		public String getCustomParameters()
		{
			return this.customParameters;
		}

		public void setCustomParameters(params object[] argsRest)
		{
			string returnString = "";

			for (int i = 0; i < argsRest.Length; ++i)
			{
				returnString = returnString + '&' + argsRest[i];
				if (++i < argsRest.Length)
					returnString = returnString + '=' + HttpUtility.UrlEncode((string)argsRest[i]);
			}

			this.customParameters = returnString;
		}

		public void resetBaseParams()
		{
			this.url = null;
			this.html = null;
			this.text = null;
		}

		virtual public String getParameterString()
		{
			String retString = "";

			if (this.url != null) retString += "&url=" + HttpUtility.UrlEncode(this.url);
			if (this.html != null) retString += "&html=" + HttpUtility.UrlEncode(this.html);
			if (this.text != null) retString += "&text=" + HttpUtility.UrlEncode(this.text);
			if (this.customParameters!=null) retString+=this.customParameters;
			if (this.outputMode != OutputMode.NONE)
			{
				if (this.outputMode == OutputMode.XML)
					retString += "&outputMode=xml";
				else if (this.outputMode == OutputMode.RDF)
					retString += "&outputMode=rdf";
                //else if (outputMode == OutputMode.JSON)
                  //  retString += "&outputMode=json";
			} 

			return retString;
		}
	}
}
