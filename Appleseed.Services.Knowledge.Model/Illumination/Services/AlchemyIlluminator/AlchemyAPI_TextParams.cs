namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System;

    public class AlchemyAPI_TextParams : AlchemyAPI_BaseParams
	{
		private enum TBOOL
		{
			NONE,
			FALSE,
			TRUE
		}

		private TBOOL useMetadata;
		private TBOOL extractLinks;
	
		public bool isUseMetadata()
		{
			if (TBOOL.TRUE == this.useMetadata)
				return true;
			return false;
		}

		public void setUseMetadata(bool useMetadata)
		{
			if (useMetadata)
				this.useMetadata = TBOOL.TRUE;
			else
				this.useMetadata = TBOOL.FALSE;
		}

		public bool isExtractLinks()
		{
			if (TBOOL.TRUE == this.extractLinks)
				return true;
			return false;
		}

		public void setExtractLinks(bool extractLinks)
		{
			if (extractLinks)
				this.extractLinks = TBOOL.TRUE;
			else
				this.extractLinks = TBOOL.FALSE;
		}

		override public String getParameterString()
		{
			String retString = base.getParameterString();
   
			if (this.useMetadata != TBOOL.NONE) retString += "&useMetadata=" + (this.useMetadata==TBOOL.TRUE ? "1" : "0");
			if (this.extractLinks != TBOOL.NONE) retString += "&extractLinks=" + (this.extractLinks == TBOOL.TRUE ? "1" : "0");

			return retString;
		}
	}
}
