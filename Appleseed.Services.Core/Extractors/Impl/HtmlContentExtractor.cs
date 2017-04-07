namespace Appleseed.Services.Core.Extractors.Impl
{
    using System;
    using System.Linq;
    using System.Net;

    using Appleseed.Services.Core.Models;

    using HtmlAgilityPack;

    using NReadability;
    

    public class HtmlContentExtractor : IUrlContentExtractor
    {
        public ExtractedWebContent Extract(string url)
        {
            try
            {
                var extractedWebPage = new ExtractedWebContent();
                string content; 
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; abot v@ABOTASSEMBLYVERSION@ http://code.google.com/p/abot)");
                    content = wc.DownloadString(url);
                }

                bool success;

                //var nReadabilityTranscoder = new NReadabilityTranscoder(ReadingStyle.Terminal, ReadingMargin.XNarrow, ReadingSize.Medium);
                var nReadabilityTranscoder = new NReadabilityTranscoder();
                var nReadabilityWebTranscoder = new NReadabilityWebTranscoder();

                TranscodingInput transcodingInput = new TranscodingInput(content);
                /*WebTranscodingInput transcodingInput = new WebTranscodingInput(url)*/;

                    transcodingInput.DomSerializationParams.DontIncludeContentTypeMetaElement = true;
                    transcodingInput.DomSerializationParams.DontIncludeDocTypeMetaElement = true;
                    transcodingInput.DomSerializationParams.DontIncludeMobileSpecificMetaElements = true;
                    transcodingInput.DomSerializationParams.PrettyPrint = true;
                
                //Console.WriteLine(nReadabilityTranscoder.Transcode(content, out success));
                //var transcodingResult = new NReadabilityWebTranscoder().Transcode(webTranscodingInput);

                try
                {
                    TranscodingResult transcodingResult = nReadabilityTranscoder.Transcode(transcodingInput);
                    //WebTranscodingResult transcodingResult = nReadabilityWebTranscoder.Transcode(transcodingInput);
                    
                    // TODO: THIS IS TEMPORARY.  This method of getting page title from URL should not be used for any
                    // site other than an Appleseed site.

                    // Attempt to get the page name from the URL; otherwise get it from the Title tag instead
                    var urlSegments = url.Split('/');
                    if (!urlSegments.Last().EndsWith(".aspx"))
                    {
                        extractedWebPage.Title = urlSegments.Last().Replace('-', ' ');
                    } 
                    else if (transcodingResult.TitleExtracted)
                    {
                        extractedWebPage.Title = transcodingResult.ExtractedTitle;
                    }

                    if (transcodingResult.ContentExtracted)
                    {
                        var htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(transcodingResult.ExtractedContent);
                        try
                        {
                            extractedWebPage.Content = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='readInner']").InnerHtml;
                        }
                        catch (Exception ex)
                        {
                            extractedWebPage.Content = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HtmlContentExtractor - Readable Content - Exception :" + ex.Message);
                    
                    //USE HtmlAgilityPack to get information regardless of whether NReadability cleaned it up

                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(content);
                        try
                        {
                            extractedWebPage.Content = content;
                            extractedWebPage.Title = htmlDocument.DocumentNode.SelectSingleNode("//title").InnerText;
                        }
                        catch (Exception htmlAgilityEx)
                        {
                            extractedWebPage.Content = string.Empty;
                        }

                }

                return extractedWebPage;
            }
            catch (Exception ex)
            {

                Console.WriteLine("HtmlContentExtractor - Exception :" + ex.Message);
            }

            return (ExtractedWebContent)null;
        }
    }
}
