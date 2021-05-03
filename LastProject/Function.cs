using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LastProject
{
    [Serializable]
    public class Jokes
    {
        public string id;
        public string type;
        public string setup;
        public string punchline;
    }

    public class Function
    {

        public static readonly HttpClient client = new HttpClient();
        private static AmazonDynamoDBClient newClient = new AmazonDynamoDBClient();
        private static string tableName = "LastProject";
        public async Task<ExpandoObject> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            //Jokes newJoke = JsonConvert.DeserializeObject<Jokes>(input.Body);
            string list = "";
            Dictionary<string, string> dict = (Dictionary<string, string>)input.QueryStringParameters;
            dict.TryGetValue("list", out list);

            HttpResponseMessage response = await client.GetAsync("https://official-joke-api.appspot.com/jokes/" + list);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();


           
            Document myDoc = Document.FromJson(responseBody);
            dynamic objX = JsonConvert.DeserializeObject<ExpandoObject>(myDoc.ToJson());

            
            Jokes newJoke = JsonConvert.DeserializeObject<Jokes>(myDoc.ToJson());
            Table jokes = Table.LoadTable(newClient, tableName);

            PutItemOperationConfig config = new PutItemOperationConfig();
            config.ReturnValues = ReturnValues.AllOldAttributes;
            Document result = await jokes.PutItemAsync(Document.FromJson(JsonConvert.SerializeObject(newJoke)), config);


            return objX;
        }
    }
}
