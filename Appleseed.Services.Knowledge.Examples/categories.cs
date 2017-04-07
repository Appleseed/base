
using System;
using System.IO;
using Appleseed.Services.Knowledge.API;


    public class TestApp_Categories
    {
        static public void Main()
        {
            // Create an AlchemyAPI object.
            Appleseed.Services.Knowledge.API.AlchemyAPI alchemyObj = new Appleseed.Services.Knowledge.API.AlchemyAPI();


            // Load an API key from disk.
            alchemyObj.LoadAPIKey("api_key.txt");


            // Categorize a web URL by topic.
            string xml = alchemyObj.URLGetCategory("http://www.techcrunch.com/");
            Console.WriteLine(xml);


            // Categorize some text.
            xml = alchemyObj.TextGetCategory("Developing new or expanded client relationships is important for the long term success of all professional firms?");
            Console.WriteLine(xml);


            // Load a HTML document to analyze.
            StreamReader streamReader = new StreamReader("data/example.html");
            string htmlDoc = streamReader.ReadToEnd();
            streamReader.Close();


            // Categorize a HTML document by topic.
            xml = alchemyObj.HTMLGetCategory(htmlDoc, "http://www.test.com/");
            Console.WriteLine(xml);
        }

    }

