namespace Appleseed.Services.Base.Engine.Processors.Impl
{
    public abstract class BaseOrganizer
    {
        /// <summary>
        /// Some API calls don't want more than a certain amount of KB per request. This gets the first Nth kilobytes. 
        /// Strings take up  20+(String.Length/2)*4 bytes
        /// </summary>
        /// <param name="content">The full content string.</param>
        /// <param name="countkb">Number of Kilo-Bytes. E.g. 150 (kb)</param>
        /// <returns></returns>
        protected static string GetFirstCountKB(string content, int countkb)
        {
            int numberBytes = 20 + ((content.Length / 2) * 4);
            int numberKBytes = numberBytes / 1024;
            string returnContent = content;

            if (numberKBytes > countkb)
            {
                return content.Substring(0, (countkb * 1024 / 2) - 20);
            }

            return returnContent;
        }
    }
}
