using Amazon.DynamoDBv2.DataModel;


namespace LibraryApi.Models {
    [DynamoDBTable("bookTable")]
    public class BookModel {
        [DynamoDBHashKey] public string id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string description { get; set; }
    }
}
