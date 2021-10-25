using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using LibraryApi.Commands;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryApi.Models;
using LibraryApi.Util;
using Newtonsoft.Json;

namespace LibraryApi.Controllers {
    //[Authorize]
    [Route("[controller]")]
    public class BooksController : ControllerBase{
        public BooksController() { }

        [HttpGet]
        public async Task<IActionResult> Get() {
            try {
                var result = await HandleBookEventCommand.ProcessBooksGetEvent();
                return result.OperationSucceeded 
                    ? (IActionResult) new OkObjectResult(result.NameList)
                    : new BadRequestObjectResult(
                        $"There was an error getting books. Error message: {result.Exception}");
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BookModel book) {
            LambdaLogger.Log("starting to process post -event of book");
            try {
                var result = await HandleBookEventCommand.ProcessBookPutEvent(book);
                return result.OperationSucceeded
                    ? (IActionResult) new OkObjectResult($"{book.title} was updated successfully")
                    : Util.Util.FormatError(result);
            }
            catch (Exception e) {
                return new BadRequestObjectResult(
                    $"There was a problem with updating {book.title}. Error message: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(string id) {
            try {
                var result = await HandleBookEventCommand.ProcessBookDeleteEvent(id);
                return result.OperationSucceeded
                    ? (IActionResult) new OkObjectResult($"{id} was deleted successfully")
                    : Util.Util.FormatError(result);
            }
            catch (Exception e) {
                return new BadRequestObjectResult(
                    $"There was a problem with updating {id}. Error message: {e.Message}");
            }
            
        }
        
        [HttpPost("list")]
        public async Task<IActionResult> PostList([FromBody] BookModel[] books) {
            try {
                var result = await HandleBookEventCommand.ProcessBookPatchPutEvent(books);
                return result.OperationSucceeded
                    ? (IActionResult) new OkObjectResult($"Books were updated successfully")
                    : new BadRequestObjectResult(
                        $"There was an error processing update of list of books. Error message: {result.Exception}");
            }
            catch (Exception e) {
                return new BadRequestObjectResult(
                    $"There was a problem with updating this list of books. Error message: {e.Message}");
            }
        }
    }
}
