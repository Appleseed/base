namespace Appleseed.Services.Base.Engine.Processors
{
    using System.Collections.Generic;
    
    using Appleseed.Services.Base.Model;

    /// <summary>
    /// Interface used to extract content to Appleseed Module Items
    /// </summary>
    public interface IExtractThings
    {
        List<AppleseedModuleItemIndex> ExtractDataForIndexing(IEnumerable<BaseItem> items);
    }
}
