namespace Appleseed.Services.Knowledge.Model.Illumination.Services.AlchemyIlluminator
{
    using System.Web;

    public class AlchemyAPI_ConstraintQueryParams : AlchemyAPI_BaseParams
	{
		private string cQuery;

		public string getCQuery()
		{
			return this.cQuery;
		}
		public void setCQuery(string cQuery)
		{
			this.cQuery = cQuery;
		}
		override public string getParameterString()
		{
			string retstring = base.getParameterString();
			if (this.cQuery != null) retstring += "&cquery=" + HttpUtility.UrlEncode(this.cQuery);
			
			return retstring;
		} 
	}
}
