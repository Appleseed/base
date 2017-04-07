namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.WebService
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    [Serializable]
    public class IlluminationWebServiceException : Exception
    {
        private XDocument _doc;
        private XElement _elem;
        public string _message;

        public IlluminationWebServiceException(XDocument doc)
        {
            this._doc = doc;
            base.Source = this._doc.ToString();
            base.HelpLink = "http://www.openIllumination.com/";
        }

        public override string Message
        {
            get
            {
                if (this._doc.Descendants("Exception") != null)
                {
                    this._elem = this._doc.Descendants("Exception").First();
                }
                else
                {
                    return "The OpenIllumination web service raised an unknown exception.";
                }

                return string.Format(" The OpenIllumination web service raised an exception: {0}.", this._elem.Value);    
            }
        }   

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            if(this._doc.Descendants("Exception") != null)
            {
                this._elem = this._doc.Descendants("Exception").First();
            }

            sb.AppendFormat(" The OpenIllumination web service raised an exception: {0} ", this._elem.Value);

            if (this.InnerException != null)
            {
                sb.AppendFormat(" ---> {0} <---", base.InnerException.ToString());
            }

            if (this.StackTrace != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append(base.StackTrace);
            }

            return sb.ToString();
        }
    }
}