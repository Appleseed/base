namespace Appleseed.Services.Base.Engine.Processors
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface used to index Appleseed Module Items
    /// </summary>
    public interface IIndexThings
    {
        void Build(IEnumerable<Model.AppleseedModuleItemIndex> indexableData);
    }
}
