namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Text;

    using Appleseed.Services.Core.Helpers;

    /// <summary>
    /// Extract all conent from an Identity object
    /// </summary>
    public class IdentityExtractor : IExtractThings
    {
        private const string UpdateSql = "UPDATE [MemberContacts] SET HasBeenIndexed = 1 WHERE Id IN ({0});";

        public List<Model.AppleseedModuleItemIndex> ExtractDataForIndexing(IEnumerable<Model.BaseItem> dataToIndex)
        {
            var indexableData = new List<Model.AppleseedModuleItemIndex>();

            foreach (var baseItem in dataToIndex)
            {
                var identity = baseItem as Model.IdentityItem;
                if (identity != null)
                {
                    var indexableItem = new Model.AppleseedModuleItemIndex
                                            {
                                                Name = identity.Name,
                                                Path = identity.Path,
                                                Content = identity.Introduction,
                                                Summary = identity.Developer + " " + identity.Mobile + " " + identity.Topics,
                                                CreatedDate = DateTime.Today.ToString()
                                            };
                    indexableData.Add(indexableItem);
                }
            }

            this.Cleanup(dataToIndex);

            return indexableData;
        }

        private void Cleanup(IEnumerable<Model.BaseItem> items)
        {
            var connectionString = ConfigurationManager.AppSettings["AppleseedConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("'AppleseedConnectionString' was not found in the appSettings of the configuration file.");
            }

            var ids = new StringBuilder();
            foreach (var item in items)
            {
                var identityItem = item as Model.IdentityItem;
                ids.Append(identityItem.DatabaseId + ",");
            }

            var contactIds = ids.ToString();
            contactIds = contactIds.TrimEnd(',');

            SqlClientHelper.ExecuteNonQuery(connectionString, CommandType.Text, string.Format(UpdateSql, contactIds));
        }
    }
}

