namespace Appleseed.Services.Base.Engine.Processors
{
    using System.Collections.Generic;

    using Appleseed.Services.Base.Model;

    /// <summary>
    /// Interface used to organize Appleseed Module Items
    /// </summary>
    public interface IOrganizeThings
    {
        void Organize(List<AppleseedModuleItemIndex> itemsToOrganize);
    }
}
