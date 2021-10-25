using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using LibraryApi.Commands;
using LibraryApi.Models;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace LibraryApi.Tests {
    public class BooksControllerTest {
        
        [Fact]
        public async Task GetBooks() {
            var dbMock = new Mock<IAmazonDynamoDB>(MockBehavior.Loose);

            dbMock.Setup(s => s.
                ScanAsync(It.IsAny<ScanRequest>(), It.IsAny<CancellationToken>())).
                Returns(Task.FromResult<ScanResponse>(new ScanResponse{Items = CreateBookMocks()}));

            var result = HandleBookEventCommand.ProcessBooksGetEvent();
            Assert.True(result.Result.OperationSucceeded);
            Assert.NotNull(result.Result.NameList);
        }

        public List<Dictionary<string, AttributeValue>> CreateBookMocks() {
            var book1 = new Dictionary<string, AttributeValue>();
            book1.Add("id", new AttributeValue("6f088392-eee9-4fa7-9165-759ae701605d"));
            book1.Add("title", new AttributeValue("Tarina koirasta"));
            book1.Add("author", new AttributeValue("Mikko Mallikas"));
            book1.Add("description", new AttributeValue("Tämä on tarina koirasta, joka on jännittävä"));
            
            var book2 = new Dictionary<string, AttributeValue>();
            book2.Add("id", new AttributeValue("eee9-4fa7-9165-759ae701bb33-6f088392"));
            book2.Add("title", new AttributeValue("Tarina kissasta"));
            book2.Add("author", new AttributeValue("Mikko Mallikas"));
            book2.Add("description", new AttributeValue("Tämä on tarina kissasta, joka ei ole jännittävä"));

            return new List<Dictionary<string, AttributeValue>> {book1, book2};
            
        }
        
    }
}
