namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Documents
{
    using System.Xml.Linq;

    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Interfaces;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.WebService;

    public class CalaisJsonDocument : ICalaisDocument
    {
        #region ICalaisDocument Members

        public string RawOutput { get; set; }

        public void ProcessResponse(string response)
        {
            this.RawOutput = response;

            //Check for web service error.
            if(response.Contains("</Exception></Error>"))
            {
                throw new IlluminationWebServiceException(XDocument.Parse(response));
            }
        }

        #endregion
    }
}