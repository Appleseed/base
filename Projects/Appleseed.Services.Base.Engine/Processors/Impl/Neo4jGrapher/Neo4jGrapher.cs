using Appleseed.Services.Base.Model;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appleseed.Services.Base.Engine.Processors.Impl.Neo4jGrapher
{
    public class Neo4jGrapher
    {
        private Uri uri { get; set; }

        public Neo4jGrapher(Uri uri)
        {
            this.uri = uri;
        }

        public void WriteIntoNeo4j(IEnumerable<AppleseedModuleItemIndex> dataToWrite)
        {
            GraphClient neo4jClient = new GraphClient(this.uri);
            neo4jClient.Connect();
            HashSet<string> wordHash = new HashSet<string>();

            foreach (AppleseedModuleItemIndex itemIndex in dataToWrite)
            {
                Document newDocument = new Document { ItemKey = itemIndex.Key };

                neo4jClient.Cypher
                    .Create("(document:Document {newDocument})")
                    .WithParam("newDocument", newDocument)
                    .ExecuteWithoutResults();

                /// May not split with ' '
                // char[] delimiterChar = {',', ' '};
                char[] delimiterChar = { ',' };
                string[] keywords = itemIndex.SmartKeywords.Split(delimiterChar);

                foreach (string word in keywords)
                {
                    if (word.Trim() != "")
                    {
                        string wordLower = word.Trim().ToLower();

                        if (wordHash.Contains(wordLower))
                        {
                            neo4jClient.Cypher
                                .Match("(thisDoc:Document)")
                                .Where((Document thisDoc) => thisDoc.ItemKey == itemIndex.Key)
                                .Match("(thisKwd:KeyWord)")
                                .Where((KeyWord thisKwd) => thisKwd.KeyContent == wordLower)
                                .Create("thisDoc-[:HASKEYWORD]->thisKwd")
                                .ExecuteWithoutResults();
                        }
                        else
                        {
                            KeyWord newKeyWord = new KeyWord { KeyContent = wordLower };
                            neo4jClient.Cypher
                                .Match("(thisDoc:Document)")
                                .Where((Document thisDoc) => thisDoc.ItemKey == itemIndex.Key)
                                .Create("thisDoc-[:HASKEYWORD]->(keyWord:KeyWord {newKeyWord})")
                                .WithParam("newKeyWord", newKeyWord)
                                .ExecuteWithoutResults();
                        }

                        wordHash.Add(wordLower);

                    }
                }

            }
        }
    }
}
