namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Entities
{
    using System.Collections.Generic;

    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Enums;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Helpers;
    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Interfaces;

    public class CalaisRdfEntity : ICalaisRdfEntity
    {
        #region ICalaisRdfEntity Members

        public string Id { get; set; }
        public IEnumerable<CalaisRdfResourceInstance> Instances { get; set; }

        #endregion

        public CalaisRdfEntityType EntityType { get; set; }
        public CalaisRdfEntitySubType EntitySubType { get; set; }
        public string Value { get; set; }
        public string SubValue { get; set; }

        public CalaisRdfEntity()
        {
            this.EntitySubType = CalaisRdfEntitySubType.None;
            this.SubValue = string.Empty;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            if(string.IsNullOrEmpty(this.SubValue))
            {
                return this.EntityType + ": " + this.Value + "; ";    
            }

            return this.EntityType + ": " + this.Value + " ( " + StringEnum.GetString(this.EntitySubType) + " : " + this.SubValue.CapitalizeAll() + " )";
        }
    }
}