# LibraryApi

### 

### <u>What is this?</u>

Library API -project offers an REST interface, which can be used to access and modify database which consists of books and information about them.

This project is built to run in AWS, and it can be accessed here: [GET books](https://libraryapi.matiaslang.info/api/books) . Rest of the requests can be found from postman collection which is included in this [repository](https://github.com/matiaslang/LibraryApi/blob/master/Library%20V.1.postman_collection.json). (More information about importing postman collection can be found here: [Importing and exporting data](https://learning.postman.com/docs/getting-started/importing-and-exporting-data/))

Endpoints:

Get Books: https://libraryapi.matiaslang.info/api/books

Post a Book: https://libraryapi.matiaslang.info/api/books

Post a list of Books: https://libraryapi.matiaslang.info/api/books/list

Delete a book: https://libraryapi.matiaslang.info/api/books/{{BookId}}

note: if you include an id of existing book in the payload, then the book with that id will be updated. If the id does not exist, then one will be generated and a new book will be created.

------

### <u>What still needs to be done?</u>

- [ ] Add proper instructions to run this locally

  - [ ] Make it easier and more straighforward to do so

  