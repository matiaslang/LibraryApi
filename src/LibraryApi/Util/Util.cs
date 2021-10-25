using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Util {
    public static class Util {

        public static ActionResult FormatError(EventResult result) {
            switch (result.ExceptionCode) {
                case 400:
                    return new BadRequestObjectResult(result.Exception);
                case 404:
                    return new NotFoundObjectResult(result.Exception);
            }
            return new NotFoundObjectResult(result.Exception);
        }

        public static string ConfirmNeededValuesArePresent(BookModel book) {
            if (string.IsNullOrWhiteSpace(book.author)) return "author";
            if (string.IsNullOrWhiteSpace(book.description)) return "description";
            return string.IsNullOrWhiteSpace(book.title) ? "title" : null;
        }
    }
}
