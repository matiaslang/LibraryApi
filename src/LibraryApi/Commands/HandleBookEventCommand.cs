using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using LibraryApi.Models;
using static LibraryApi.DynamoDB.DynamoClient;


namespace LibraryApi.Commands {
    public class HandleBookEventCommand {
        public static async Task<EventResult> ProcessBookPutEvent(BookModel book) {
            if (Util.Util.ConfirmNeededValuesArePresent(book) != null)
                return new EventResult {
                    OperationSucceeded = false, ExceptionCode = 400,
                    Exception = $"Missing a property in payload. Please add {Util.Util.ConfirmNeededValuesArePresent(book)}."
                };
            try {
                CreateClient(false);
                await CreateTableIfNotExisting();
                var result = await Client.PutItemAsync(CreatePutItemRequest(book));
                return new EventResult {OperationSucceeded = true};
            }
            catch (Exception e) {
                LambdaLogger.Log($"Error while posting single book: {e.Message}");
                return new EventResult {OperationSucceeded = false, Exception = e.Message};
            }

        }

        public static async Task<EventResult> ProcessBooksGetEvent() {
            try {
                var result = await GetList();
                var list = result.Select(b => new BookModel {
                    author = GetValue("author", b),
                    description = GetValue("description",b),
                    id = GetValue("id", b),
                    title = GetValue("title", b)
                });
                var sortedList = list.OrderByDescending(book => book.title).ToList();
                return new EventResult {OperationSucceeded = true, NameList = sortedList};
            }
            catch (Exception e) {
                LambdaLogger.Log($"Error while getting books. Errormessage: {e.Message}");
                return new EventResult {OperationSucceeded = false};
            }
        }

        private static string GetValue(string title, Dictionary<string,AttributeValue> dictionary) {
            return dictionary[title].S.ToString();
        }

        public static async Task<EventResult> ProcessBookPatchPutEvent(BookModel[] books) {
            try {
                CreateClient(false);
                var context = new DynamoDBContext(Client);
                var bookBatch = context.CreateBatchWrite<BookModel>(new DynamoDBOperationConfig());
                var booksWithIDs = books.Select(b => new BookModel
                {
                    author = b.author,
                    description = b.description,
                    title = b.title,
                    id = string.IsNullOrWhiteSpace(b.id) ? Guid.NewGuid().ToString() : b.id
                });
                bookBatch.AddPutItems(booksWithIDs);
                await bookBatch.ExecuteAsync();
                return new EventResult {OperationSucceeded = true};
            }
            catch (Exception e) {
                Console.WriteLine($"There was an error posting item to dynamo: {e.Message}");
                return new EventResult {OperationSucceeded = false};
            }
        }

        public static async Task<EventResult> ProcessBookDeleteEvent(string id) {
            try {
                CreateClient(false);
                var context = new DynamoDBContext(Client);
                var result = await DeleteItem(id);
                if (result.Attributes.Count > 0) {
                    return new EventResult {OperationSucceeded = true};
                }
                return new EventResult {OperationSucceeded = false, Exception = $"No items found based on id {id}", ExceptionCode = 404};
                
            }
            catch (Exception e) {
                Console.WriteLine($"There was an error deleting item from dynamo: {e.Message}");
                return new EventResult {OperationSucceeded = false};
            }
        }
    }
}
