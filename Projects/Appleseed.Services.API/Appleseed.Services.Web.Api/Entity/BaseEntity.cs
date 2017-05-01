namespace Appleseed.Services.Web.Api.Entity
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class BaseEntity
    {
         [DataMember]
         public string Name
         { get; set; }

         [DataMember]
         public int ID
         { get; set; }

         [DataMember]
         public Guid UniqueGUID
         { get; set; }
    }
}