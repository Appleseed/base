namespace Appleseed.Services.Base.Engine.Processors
{
    using System.Collections.Generic;

    using Appleseed.Services.Base.Model;

    /// <summary>
    /// Interface used to collect Items
    /// </summary>
    public interface ICollectThings
    {
        List<BaseItem> CollectItems();

        void CollectItems(List<BaseItem> items);
    }
}
