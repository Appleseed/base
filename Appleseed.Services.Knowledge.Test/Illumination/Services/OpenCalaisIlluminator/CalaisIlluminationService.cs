namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator
{
    using System;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Linq;

    public class CalaisIlluminationService : IIlluminationService
    {
        private readonly string apiKey;

        public CalaisIlluminationService()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("OpenCalaisApiKey"))
            this.apiKey = ConfigurationManager.AppSettings["OpenCalaisApiKey"];

            if (string.IsNullOrEmpty(this.apiKey) || this.apiKey.Length != 24)
            {
                throw new Exception("No valid API Key was found in the configuration for the Open Calais Illumination Service.");
            }
        }

        public CalaisIlluminationService(string apiKey)
        {
            this.apiKey = apiKey;

            if (string.IsNullOrEmpty(this.apiKey) || this.apiKey.Length != 24)
            {
                throw new ArgumentNullException("apiKey", "No valid API Key was provided to the Open Calais Illumination Service.");
            }
        }


        public IEnumerable<Models.IlluminationResult> Illuminate(string content)
        {
            var openCalais = new CalaisDotNet(this.apiKey, content);
            var doc = openCalais.GetSimpleDocument();

            var results = new List<Models.IlluminationResult>();

            results.AddRange((from e in doc.Entities
                              select new Models.IlluminationResult()
                              {
                                  Value = e.Value,
                                  Relevance = e.Relevance
                              }).ToList());

            results.AddRange((from e in doc.Events
                              select new Models.IlluminationResult()
                              {
                                  Value = e
                              }).ToList());

            results.AddRange((from e in doc.SocialTags
                              select new Models.IlluminationResult()
                              {
                                  Value = e.Value,
                                  Relevance = e.Importance
                              }).ToList());

            results.AddRange((from e in doc.Topics
                              select new Models.IlluminationResult()
                              {
                                  Value = e.Value,
                                  Relevance = e.Score
                              }).ToList());

            return results;
        }
    }
}
