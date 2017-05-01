namespace Appleseed.Services.Knowledge.Model.Illumination.Services
{
    using System.Collections.Generic;

    using Appleseed.Services.Knowledge.Model.Illumination.Models;

    public interface IIlluminationService
    {
        IEnumerable<IlluminationResult> Illuminate(string content);
    }
}
