namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Entities
{
    using System.Collections.Generic;
    using System.Text;

    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Helpers;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Interfaces;

    public class CalaisRdfRelationship : ICalaisRdfEntity
    {
        #region ICalaisRdfEntity Members

        public string Id { get; set; }
        public IEnumerable<CalaisRdfResourceInstance> Instances { get; set; }

        #endregion

        public CalaisRdfRelationshipType RelationshipType { get; set; }
        public IDictionary<string, string> RelationshipDetails { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            
            foreach (var detail in this.RelationshipDetails)
            {
                sb.Append(detail.Key.CapitalizeAll() + ": " + detail.Value + "; ");    
            }

            return sb.ToString();
        }
    }
}