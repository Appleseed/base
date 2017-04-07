namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Documents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Entities;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Helpers;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Interfaces;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.WebService;

    public class CalaisRdfDocument : ICalaisDocument
    {
        #region Private Variables

        //Key namespaces needed for processing.
        readonly XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        readonly XNamespace c = "http://s.openIllumination.com/1/pred/";

        //URIs for various types.
        private const string _relationshipUri = "http://s.openIllumination.com/1/type/em/r/";
        private const string _entityUri = "http://s.openIllumination.com/1/type/em/e/";
        private const string _docInfoUri = "http://s.openIllumination.com/1/type/sys/DocInfo";
        private const string _docMetaUri = "http://s.openIllumination.com/1/type/sys/DocInfoMeta";
        private const string _instanceUri = "http://s.openIllumination.com/1/type/sys/InstanceInfo";

        #endregion

        #region Public Variables

        #endregion

        #region Public Fields

        public IEnumerable<CalaisRdfEntity> Entities { get; set; }
        public CalaisRdfDocumentDescription Description { get; set; }
        public IEnumerable<CalaisRdfRelationship> Relationships { get; set; }

        #endregion

        #region ICalaisDocument Members

        public string RawOutput { get; set; }

        /// <summary>
        /// Processes the response from the server.
        /// </summary>
        /// <param name="response">The response.</param>
        public void ProcessResponse(string response)
        {
            //Contract
            this.Require(item => response != null, new ArgumentNullException("response"));

            this.RawOutput = response;
            var doc = XDocument.Parse(response);

            this.Ensure(item => doc != null, new Exception("Unable to process response!"));
            this.Ensure(item => doc.Element("Error") == null, new IlluminationWebServiceException(doc));

            //Process each part of the document in order.
            this.Description = this.ProcessRdfDescription(doc);
            this.Entities = this.ProcessRdfEntities(doc);
            this.Relationships = this.ProcessRdfRelationships(doc);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes any RDF relationships.
        /// </summary>
        /// <param name="doc">Parsed response from the server.</param>
        /// <returns>A collection of RDF relationships.</returns>
        private IEnumerable<CalaisRdfRelationship> ProcessRdfRelationships(XDocument doc)
        {
            //Contract
            this.Require(x => doc != null, new ArgumentNullException("doc"));

            //Select elements that conatain a relationship URI
            var results = from item in doc.Root.Descendants(this.rdf + "Description")
                          where item.Element(this.rdf + "type").Attribute(this.rdf + "resource").Value.Contains(_relationshipUri)
                          select item;

            foreach (var result in results)
            {
                var relationship = new CalaisRdfRelationship
                                       {
                                           Id = result.Attribute(this.rdf + "about").Value,
                                           RelationshipType = ((CalaisRdfRelationshipType) Enum.Parse(typeof (CalaisRdfRelationshipType), result.Element(this.rdf + "type").Attribute(this.rdf + "resource").Value.Replace(_relationshipUri, string.Empty))),
                                           RelationshipDetails = this.ProcessRdfRelationshipDetails(result.Elements().Where(item => item.Name.Namespace == this.c)).ToDictionary(item => item.Key, item => item.Value),
                                           Instances = this.ProcessRdfInstances(result.Attribute(this.rdf + "about").Value, doc)
                                       };

                //Check that for each relationship there is at least one corresponding instance (otherwise something is seriously broken!)
                this.Ensure(item => relationship.Instances.Count() > 0, new ApplicationException("No instances of entity found in the document"));

                yield return relationship;
            }
        }

        /// <summary>
        /// Processes the RDF relationship details into key/value pairs so they can be added to an IDictionary
        /// </summary>
        /// <param name="elements">Collection of elements conatining details.</param>
        /// <returns>A collection of keyvalue pairs containing property name (key) and associated value.</returns>
        private IEnumerable<KeyValuePair<string, string>> ProcessRdfRelationshipDetails(IEnumerable<XElement> elements)
        {
            foreach (var element in elements)
            {
                if (element.Attribute(this.rdf + "resource") != null)
                {
                    yield return new KeyValuePair<string, string>(element.Name.LocalName, this.ResolveRdfEntity(element.Attribute(this.rdf + "resource").Value));
                }
                else
                {
                    yield return new KeyValuePair<string, string>(element.Name.LocalName, element.Value);
                }
            }
        }

        /// <summary>
        /// Resolves the RDF entity from the existsing collection of entities.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns></returns>
        private string ResolveRdfEntity(string resourceId)
        {
            var result = from item in this.Entities
                         where item.Id == resourceId
                         select item.Value;

            if (result.Count() > 0)
            {
                return result.Single();
            }

            return string.Empty;
        }

        /// <summary>
        /// Takes the DocumentInfo and DocumentMetadata and combines them into one description object
        /// </summary>
        /// <param name="doc">Parsed response from the server.</param>
        /// <returns>RDF Description object</returns>
        private CalaisRdfDocumentDescription ProcessRdfDescription(XDocument doc)
        {
            var docInfo = (from item in doc.Root.Descendants(this.rdf + "Description")
                           where item.Element(this.rdf + "type").Attribute(this.rdf + "resource").Value == _docInfoUri
                           select item).Single();

            var meta = (from item in doc.Root.Descendants(this.rdf + "Description")
                        where item.Element(this.rdf + "type").Attribute(this.rdf + "resource").Value == _docMetaUri
                        select item).Single();

            var description = new CalaisRdfDocumentDescription
                                  {
                                      Id = docInfo.Attribute(this.c + "id").Value,
                                      About = docInfo.Attribute(this.rdf + "about").Value,
                                      AllowDistribution = bool.Parse(docInfo.Attribute(this.c + "allowDistribution").Value),
                                      AllowSearch = bool.Parse(docInfo.Attribute(this.c + "allowSearch").Value),
                                      ExternalId = docInfo.Attribute(this.c + "externalID").Value,
                                      Document = docInfo.Element(this.c + "document").Value,
                                      Submitter = docInfo.Element(this.c + "submitter").Value,
                                      ContentType = meta.Attribute(this.c + "contentType").Value,
                                      EmVer = meta.Attribute(this.c + "emVer").Value,
                                      Language = meta.Attribute(this.c + "language").Value,
                                      LanguageIdVer = meta.Attribute(this.c + "langIdVer").Value,
                                      ProcessingVersion = meta.Attribute(this.c + "processingVer").Value,
                                      SubmitionDate = meta.Attribute(this.c + "submissionDate").Value,
                                      SubmitterCode = meta.Element(this.c + "submitterCode").Value,
                                      Signature = meta.Element(this.c + "signature").Value
                                  };

            return description;
        }

        /// <summary>
        /// Processes the RDF entities in the response docuement.
        /// </summary>
        /// <param name="doc">Parsed response from the server.</param>
        /// <returns>A collection of RDF entities.</returns>
        private IEnumerable<CalaisRdfEntity> ProcessRdfEntities(XDocument doc)
        {
            //Contract
            this.Require(x => doc != null, new ArgumentNullException("doc"));

            //Select elements that conatin an entity URI
            var results = from item in doc.Root.Descendants(this.rdf + "Description")
                          where item.Element(this.rdf + "type").Attribute(this.rdf + "resource").Value.Contains(_entityUri)
                          select item;

            foreach (var result in results)
            {
                // Annoyingly some entities now have subtypes. e.g. Person + PersonType
                // The design now allows for one subtype per entity
                // (if this changes this will need to be re-written to work like relationship details)
                var subElements = result.Elements().Where(item => item.Name.Namespace == this.c).ToList();

                //Check that each element has (at most) one subtype
                this.Ensure(item => subElements.Count >= 1 || subElements.Count < 3, new ApplicationException("Unknown Illumination Entity format .. bailing out! Count=" + subElements.Count));

                var entity = new CalaisRdfEntity
                                 {
                                     Id = result.Attribute(this.rdf + "about").Value,
                                     EntityType = ((CalaisRdfEntityType) Enum.Parse(typeof (CalaisRdfEntityType), result.Element(this.rdf + "type").Attribute(this.rdf + "resource").Value.Replace(_entityUri, string.Empty))),
                                     Value = (subElements[0].Value ?? string.Empty),
                                     Instances = this.ProcessRdfInstances(result.Attribute(this.rdf + "about").Value, doc)
                                 };

                if (subElements.Count == 2)
                {
                    entity.EntitySubType = (CalaisRdfEntitySubType)Enum.Parse(typeof(CalaisRdfEntitySubType), subElements[1].Name.LocalName, true);
                    entity.SubValue = subElements[1].Value ?? string.Empty;
                }

                //Check that for each entity there is at least one corresponding instance (otherwise something is seriously broken!)
                this.Ensure(item => entity.Instances.Count() > 0, new ApplicationException("No instances of entity found in the document"));

                yield return entity;
            }
        }

        /// <summary>
        /// Processes all instances of a certain entity or relationship (based on its id)
        /// </summary>
        /// <param name="id">The id of the element.</param>
        /// <param name="doc">Parsed response from the server.</param>
        /// <returns></returns>
        private IEnumerable<CalaisRdfResourceInstance> ProcessRdfInstances(string id, XDocument doc)
        {
            var results = from item in doc.Root.Descendants(this.rdf + "Description")
                          where item.Element(this.rdf + "type").Attribute(this.rdf + "resource").Value == _instanceUri && item.Element(this.c + "subject").Attribute(this.rdf + "resource").Value == id
                          select new CalaisRdfResourceInstance
                                     {
                                         Detection = item.Element(this.c + "detection").Value,
                                         Length = int.Parse(item.Element(this.c + "length").Value),
                                         Offset = int.Parse(item.Element(this.c + "offset").Value)
                                     };
            return results;
        }

        #endregion
    }
}