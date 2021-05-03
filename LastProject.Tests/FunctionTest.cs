
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using LastProject;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using System.Dynamic;
using Amazon.DynamoDBv2.Model;
using Amazon;
using Amazon.Runtime;

namespace LastProject.Tests
{
    public class FunctionTest

    {
        //public static readonly HttpClient client = new HttpClient();
        public static readonly AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        public static string tableName = "LastProject";

        [Fact]
        public async Task TestToGet()
        {


            string id = "290";

            GetItemResponse res = await client.GetItemAsync(tableName, new Dictionary<string, AttributeValue>
            {
                {
                "id" , new AttributeValue { S = id}
                }
            });

            Document myDoc = Document.FromAttributeMap(res.Item);
            Jokes joke = JsonConvert.DeserializeObject<Jokes>(myDoc.ToJson());

            string punch = "He’s only got little legs.";

            Assert.Equal(punch, joke.punchline);
        }
    }
}
