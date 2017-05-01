namespace Appleseed.Services.Base.Model
{
    using System;

    
    public abstract class BaseItem
    {
        public string Key { get; set; }

        // Item Identity Fields
        public string Path { get; set; }

        public string Name { get; set; }

        public DateTime PublishedDate { get; set; }

    }
}
