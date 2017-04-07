namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Interfaces
{
    using System.Collections.Generic;

    using Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Entities;

    public interface ICalaisRdfEntity
    {
        string Id { get; set; }

        IEnumerable<CalaisRdfResourceInstance> Instances { get; set; }
    }
}