namespace Appleseed.Services.Web.Api
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Security Header that currently just exposes an API Key.
    /// The API Key is the user's unique Guid.  The user must also be an admin.
    /// </summary>
    [DataContract]
    public class AuthHeader
    {
        [DataMember]
        public string APIKey { get; set; }

        public string APISecret { get; set; }
    }
}