namespace Appleseed.Services.Core.Helpers
{
    using log4net.Repository.Hierarchy;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;


    public static class Utilities
    {
        private static readonly Regex HtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        static Utilities()
        {
        }

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string RemoveHtml(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            return Utilities.HtmlRegex.Replace(source, " ").Trim();
        }

        public static List<string> GenerateSequence_UrlParse(string rawUrl)
        {
            List<string> resultList = new List<string>();
            Regex pageRegex = new Regex(@"&\{(?<text>[a-zA-Z]+)=\[(?<start>[0-9]+):(?<end>[0-9]+);(?<inc>[0-9]+)\]\}");
            Match pageMatch = pageRegex.Match(rawUrl);

            if (pageMatch.Success)
            {
                string text = pageMatch.Groups["text"].Value;
                int start = Convert.ToInt32(pageMatch.Groups["start"].Value);
                int end = Convert.ToInt32(pageMatch.Groups["end"].Value);
                int inc = Convert.ToInt32(pageMatch.Groups["inc"].Value);

                string baseUrl = rawUrl.Replace(pageMatch.Value, "");
                int steps = (end - start + 1) / inc;
                if ((end - start + 1) % inc != 0)
                {
                    steps += 1;
                }

                for (int i = 1; i <= steps; i++)
                {
                    string ultimateUrl = baseUrl + "&" + text + "=" + ((i - 1) * inc + start).ToString();
                    resultList.Add(ultimateUrl);
                }
            }
            else
            {
                resultList.Add(rawUrl);
            }

            return resultList;
        }

        public static string GenerateItemSource(string ItemPath)
        {
            Regex sourceRegex = new Regex(@"https*://(?<text>[a-zA-Z1-9._]+)/");
            Match sourceMatch = sourceRegex.Match(ItemPath);
            string text = "";
            if (sourceMatch.Length != 0)
            {
                text = sourceMatch.Groups["text"].Value;
            }
            return text;
        }
    }
}
