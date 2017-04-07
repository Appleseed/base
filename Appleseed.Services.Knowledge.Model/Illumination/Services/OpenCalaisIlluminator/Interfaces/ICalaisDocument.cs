namespace Appleseed.Services.Knowledge.Model.Illumination.Services.OpenCalaisIlluminator.Interfaces
{
    public interface ICalaisDocument
    {
        string RawOutput { get; set; }

        void ProcessResponse(string response);
    }
}
