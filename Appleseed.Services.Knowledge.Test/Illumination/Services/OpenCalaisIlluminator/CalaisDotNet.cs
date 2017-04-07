using System.Web;

namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator
{
    using System;
    using System.Net;
    using System.Xml.Linq;

    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Documents;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Helpers;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Interfaces;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.WebService;

    /// <summary>
    /// A proxy and adapter to the Illumination web service
    /// http://openIllumination.com/
    /// </summary>
    public class CalaisDotNet
    {
        #region Private Fields

        private readonly IlluminationServiceProxy _webServiceProxy;

        #endregion

        #region Public Fields

        public string ApiKey;
        public CalaisInputFormat InputFormat;
        public string Content;
        public CalaisOutputFormat OutputFormat;
        public bool AllowDistribution;
        public bool AllowSearch;
        public string ExternalId;
        public string Submitter;
        public string BaseUrl;
        public string EnableMetadataType;
        public bool CalculateRelevanceScore;
        public bool DiscardMetadata;
        public string DiscardMetadataString;

        /// <summary>
        /// Web proxy to go through when calling the Illumination web service
        /// </summary>
        public IWebProxy Proxy
        {
            // assign web proxy to the web service proxy
            set { this._webServiceProxy.Proxy = value; }
        }

        #endregion

        #region Constructors

        public CalaisDotNet(string apiKey, string content) : this(apiKey, content, DetectInputFormat(content), string.Empty, false, false, string.Empty, string.Empty, true, true, false, string.Empty) { }

        public CalaisDotNet(string apiKey, string content, CalaisInputFormat inputFormat) : this(apiKey, content, inputFormat, string.Empty, false, false, string.Empty, string.Empty, true, true, false, string.Empty) { }

        public CalaisDotNet(string apiKey, string content, CalaisInputFormat inputFormat, string baseUrl) : this(apiKey, content, inputFormat, baseUrl, false, false, string.Empty, string.Empty, true, true, false, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalaisDotNet"/> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="content">The content.</param>
        /// <param name="inputFormat">Format of the input content.</param>
        /// <param name="baseUrl">Base URL to be put in rel-tag microformats.</param>
        /// <param name="allowDistribution">if set to <c>true</c> Indicates whether the extracted metadata can be distributed.</param>
        /// <param name="allowSearch">if set to <c>true</c> Indicates whether future searches can be performed on the extracted metadata.</param>
        /// <param name="externalId">User-generated ID for the submission.</param>
        /// <param name="submitter">Identifier for the content submitter.</param>
        /// <param name="enableMetadataType">if set to <c>true</c> Indicates whether the output (RDF only) will include Generic Relation extractions.</param>
        /// <param name="calculateRelevanceScore">if set to <c>true</c> Indicates whether the extracted metadata will include relevance score for each unique entity.</param>
        /// <param name="discardMetadata">if set to <c>true</c> </param> Indicates whether the output will exclude Entity Disambiguation results</param>
        /// <param name="discardMetadataString">entities to discard - if empty will use "er/Company;er/Geo" (all current disambiguated types)</param>
        public CalaisDotNet(string apiKey, string content, CalaisInputFormat inputFormat, string baseUrl, bool allowDistribution, bool allowSearch, string externalId, string submitter, bool enableMetadataType, bool calculateRelevanceScore, bool discardMetadata, string discardMetadataString)
        {
            //Contract
            this.Require(x => apiKey.Length == 24, new ArgumentException("API Key must be 24 characters long (yours was" + apiKey.Length + ")"));
            this.Require(x => !string.IsNullOrEmpty(content), new ArgumentException("Content cannot be empty"));

            if (string.IsNullOrEmpty(externalId))
            {
                externalId = "0980980808";
            }

            if (string.IsNullOrEmpty(submitter))
            {
                submitter = "me";
            }

            // initialise inputs required to call web service
            this.ApiKey = apiKey;
            this.Content = content;
            this.InputFormat = inputFormat;
            this.OutputFormat = CalaisOutputFormat.Simple;
            this.BaseUrl = baseUrl;
            this.AllowDistribution = allowDistribution;
            this.AllowSearch = allowSearch;
            this.ExternalId = externalId;
            this.Submitter = submitter;
            this.CalculateRelevanceScore = calculateRelevanceScore;
            this.EnableMetadataType = string.Empty;
            this.DiscardMetadata = discardMetadata;
            this.DiscardMetadataString = discardMetadataString;

            if (this.DiscardMetadata)
            {
                if (string.IsNullOrEmpty(this.DiscardMetadataString))
                {
                    this.DiscardMetadataString = "";
                }
            }

            if (enableMetadataType)
            {
                this.EnableMetadataType = "GenericRelations,SocialTags";
            }

            // create a new web service proxy to Illumination
            this._webServiceProxy = new IlluminationServiceProxy();
        }

        #endregion

        public static T Parse<T>(string xmlString) where T : ICalaisDocument
        {
            T document;

            //Check response is not empty
            if (string.IsNullOrEmpty(xmlString))
            {
                throw new ArgumentException("xmlString cannot be empty");
            }

            switch (typeof(T).Name)
            {
                case "CalaisRdfDocument":
                {
                    document = ObjectFactory.Create<T>("Illumination.CalaisRdfDocument");
                    break;
                }

                case "CalaisMicroFormatsDocument":
                {
                    document = ObjectFactory.Create<T>("Illumination.CalaisMicroFormatsDocument");
                    break;
                }

                case "CalaisSimpleDocument":
                {
                    document = ObjectFactory.Create<T>("Illumination.CalaisSimpleDocument");
                    break;
                }

                case "CalaisJsonDocument":
                {
                    document = ObjectFactory.Create<T>("Illumination.CalaisJsonDocument");
                    break;
                }

                default:
                {
                    throw new ArgumentException("Unknown type");
                }
            }

            ((ICalaisDocument)document).ProcessResponse(xmlString);

            return document;
        }

        public T Call<T>() where T : ICalaisDocument
        {
            T document;

            // Switch output type to version based on the object being returned
            // Call object factory to create concrete implimentation

            switch (typeof(T).Name)
            {
                case "CalaisRdfDocument":
                {
                    this.OutputFormat = CalaisOutputFormat.Rdf;
                    document = ObjectFactory.Create<T>("Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Documents.CalaisRdfDocument");
                    break;
                }

                case "CalaisMicroFormatsDocument":
                {
                    this.OutputFormat = CalaisOutputFormat.MicroFormats;
                    document = ObjectFactory.Create<T>("Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Documents.CalaisMicroFormatsDocument");
                    break;
                }

                case "CalaisSimpleDocument":
                {
                    this.OutputFormat = CalaisOutputFormat.Simple;
                    document = ObjectFactory.Create<T>("Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Documents.CalaisSimpleDocument");
                    break;
                }

                default:
                {
                    throw new ArgumentException("Unknown type");
                }
            }

            // Get correctly formatted input XML
            string paramsXml = this.BuildInputParamsXml();

            //Check XML was built correctly
            this.Ensure(x => !string.IsNullOrEmpty(paramsXml), new ApplicationException("Building parameters XML failed!"));

            var tempContent = HttpUtility.UrlEncode(this.Content);

            // call web service to get response
            string response = this._webServiceProxy.Enlighten(this.ApiKey, tempContent, paramsXml);

            //System.IO.File.WriteAllText(@"c:\Temp\caliasResponseRaw.xml", response);

            //Check response is not empty
            this.Ensure(x => !string.IsNullOrEmpty(response), new ApplicationException("Server response is empty!"));

            //Check for error message
            this.Ensure(x => !response.Contains("<Error>"), new ApplicationException("Server reported an error"));

            //TODO: Process <Error> message here !

            ((ICalaisDocument)document).ProcessResponse(response);

            return document;
        }

        public CalaisSimpleDocument GetSimpleDocument()
        {
            return this.Call<CalaisSimpleDocument>();
        }

        #region Helpers

        /// <summary>
        /// Builds XML input content expected by web service
        /// </summary>
        /// <remarks>
        /// Input Paramaters Reference
        /// http://opencalais.mashery.com/page/documentation#inputparameters
        /// </remarks>
        private string BuildInputParamsXml()
        {
            XNamespace c = "http://s.opencalais.com/1/pred/";
            var xdoc = new XDocument(
                new XElement(
                    c + "params",
                    new XAttribute(XNamespace.Xmlns + "c", "http://s.opencalais.com/1/pred/"),
                    new XElement(c + "processingDirectives",
                        new XAttribute(c + "contentType", StringEnum.GetString(this.InputFormat)),
                        new XAttribute(c + "outputFormat", StringEnum.GetString(this.OutputFormat)),
                        new XAttribute(c + "reltagBaseURL", this.BaseUrl),
                        new XAttribute(c + "enableMetadataType", this.EnableMetadataType),
                        new XAttribute(c + "calculateRelevanceScore", this.CalculateRelevanceScore),
                        new XAttribute(c + "discardMetadata", this.DiscardMetadataString)
                    ),
                    new XElement(c + "userDirectives",
                        new XAttribute(c + "allowDistribution", this.AllowDistribution),
                        new XAttribute(c + "allowSearch", this.AllowSearch),
                        new XAttribute(c + "externalID", this.ExternalId),
                        new XAttribute(c + "submitter", this.Submitter)
                    ),
                    new XElement(c + "externalMetadata")
                ));

            return xdoc.ToString();
        }

        /// <summary>
        /// Checks input string to see if it is HTML / XML or plain text, defaults to RawText
        /// </summary>
        /// <param name="content">Content string to be analysed</param>
        /// <returns>Input format enum</returns>
        /// <remarks>This is a little crude and could do with reworking.</remarks>
        private static CalaisInputFormat DetectInputFormat(string content)
        {
            if (content.Contains("<html"))
            {
                return CalaisInputFormat.Html;
            }

            if (content.StartsWith("<?xml") && !content.Contains("<html"))
            {
                return CalaisInputFormat.Xml;
            }

            return CalaisInputFormat.Text;
        }

        #endregion
    }
}

