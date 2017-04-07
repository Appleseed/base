
using System;
using System.IO;
using Appleseed.Services.AlchemyAPI;

namespace Appleseed.Services.AlchemyAPI
{
    public class TestApp_Keywords
    {
        static public void Main()
        {
            // Create an AlchemyAPI object.
            Appleseed.Services.Knowledge.API.AlchemyAPI alchemyObj = new Appleseed.Services.Knowledge.API.AlchemyAPI();


            // Load an API key from disk.
            alchemyObj.LoadAPIKey("api_key.txt");


            // Extract topic keywords for a web URL.
            string xml = alchemyObj.URLGetRankedKeywords("http://www.techcrunch.com/");
            Console.WriteLine(xml);


            // Extract topic keywords for a text string.
            xml = alchemyObj.TextGetRankedKeywords("Developing new or expanded client relationships is important for the long term success of all professional firms?");
            //xml = alchemyObj.TextGetRankedKeywords("Hello there, my name is Bob Jones.  I live in the United States of America.  Where do you live, Fred?");
            Console.WriteLine(xml);


            // Load a HTML document to analyze.
            StreamReader streamReader = new StreamReader("data/example.html");
            string htmlDoc = streamReader.ReadToEnd();
            streamReader.Close();


            // Extract topic keywords for a HTML document.
            xml = alchemyObj.HTMLGetRankedKeywords(htmlDoc, "http://www.test.com/");
            Console.WriteLine(xml);
        }

    }

}