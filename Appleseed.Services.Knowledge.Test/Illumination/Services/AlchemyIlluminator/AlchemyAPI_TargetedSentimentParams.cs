namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System;
    using System.Web;

    public class AlchemyAPI_TargetedSentimentParams : AlchemyAPI_BaseParams
	{
		
		private enum TBOOL
		{
			NONE,
			FALSE,
			TRUE
		}
		
		private TBOOL showSourceText;
		private string target;
	
		public bool isshowSourceText()
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
		
		public string getTarget()
		{
			return this.target;
		}
		
		public void setTarget(string target)
		{
			this.target = target;
		}
		
		override public String getParameterString()
		{
			String retString = base.getParameterString();
   
			if (this.showSourceText != TBOOL.NONE) retString += "&showSourceText=" + (this.showSourceText ==TBOOL.TRUE ? "1" : "0");
			if (this.target != null) retString += "&target=" + HttpUtility.UrlEncode(this.target);

			return retString;
		}
	}
}

