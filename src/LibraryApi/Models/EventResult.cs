using System.Collections.Generic;

namespace LibraryApi.Models {
    public class EventResult {
        public bool OperationSucceeded { get; set; }
        public string Exception { get; set; }
        public int ExceptionCode { get; set; }
        public List<BookModel> NameList { get; set; }
    }
}
