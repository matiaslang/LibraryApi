using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using LibraryApi.Models;

namespace LibraryApi.DynamoDB {
    public static class DynamoClient {
        private static readonly string tableName = "bookTable";
        private static readonly string Ip = "localhost";
        private static readonly int Port = 8000;
        private static readonly string EndpointUrl = "http://" + Ip + ":" + Port;
        public static AmazonDynamoDBClient Client;

        private static bool IsPortInUse() {
            var isAvailable = true;
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
            foreach (var endpoint in tcpConnInfoArray)
                if (endpoint.Port == Port) {
                    isAvailable = false;
                    break;
                }

            return isAvailable;
        }

        public static bool CreateClient(bool useDynamoDbLocal) {
            if (useDynamoDbLocal) {
                // First, check to see whether anyone is listening on the DynamoDB local port
                // (by default, this is port 8000, so if you are using a different port, modify this accordingly)
                var portUsed = IsPortInUse();
                if (portUsed) {
                    Console.WriteLine("The local version of DynamoDB is NOT running.");
                    return false;
                }

                // DynamoDB-Local is running, so create a client
                Console.WriteLine("  -- Setting up a DynamoDB-Local client (DynamoDB Local seems to be running)");
                var ddbConfig = new AmazonDynamoDBConfig();
                ddbConfig.ServiceURL = EndpointUrl;
                try {
                    Client = new AmazonDynamoDBClient(ddbConfig);
                }
                catch (Exception ex) {
                    Console.WriteLine("     FAILED to create a DynamoDBLocal client; " + ex.Message);
                    return false;
                }
            }
            else {
                Client = new AmazonDynamoDBClient();
            }

            return true;
        }

        public static PutItemRequest CreatePutItemRequest(BookModel book) {
            LambdaLogger.Log($"Starting to process book {book.title}");
            var attributes = new Dictionary<string, AttributeValue> {
                ["id"] = new AttributeValue {S = string.IsNullOrWhiteSpace(book.id) ? Guid.NewGuid().ToString() : book.id},
                ["title"] = new AttributeValue {S = book.title},
                ["author"] = new AttributeValue {S = book.author},
                ["description"] = new AttributeValue {S = book.description}
            };
            var request = new PutItemRequest {
                TableName = tableName,
                Item = attributes
            };
            return request;
        }

        public static async Task<DeleteItemResponse> DeleteItem(string id) {
            LambdaLogger.Log($"Starting to delete item with id: {id}");
            var client = new AmazonDynamoDBClient();
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = id} },
            };
            DeleteItemRequest request = new DeleteItemRequest
            {
                TableName = tableName,
                Key = key,
                ReturnValues = ReturnValue.ALL_OLD
            };

            return await client.DeleteItemAsync(request);
        }

        public static async Task CreateTableIfNotExisting() {
            try {
                await Client.DescribeTableAsync(tableName);
            }
            catch (ResourceNotFoundException) {
                var createResult = await CreateTable();
                Console.WriteLine(createResult);
            }
        }

        public static async Task<CreateTableResponse> CreateTable() {
            var request = new CreateTableRequest {
                TableName = tableName,
                KeySchema = new List<KeySchemaElement> {
                    new KeySchemaElement {KeyType = KeyType.HASH, AttributeName = "id"}
                },
                AttributeDefinitions = new List<AttributeDefinition>
                    {new AttributeDefinition {AttributeName = "id", AttributeType = "S"}},
                ProvisionedThroughput = new ProvisionedThroughput {ReadCapacityUnits = 1, WriteCapacityUnits = 1}
            };
            return await Client.CreateTableAsync(request);
        }

        public static async Task<List<Dictionary<string, AttributeValue>>> GetList() {
            var client = new AmazonDynamoDBClient();

            var request = new ScanRequest {
                TableName = tableName
            };

            var response = await client.ScanAsync(request);
            var result = response.Items;
            return result;
        }
    }
}
