using Lab4Web.Controllers;
using Lab4Web.Data;
using Lab4Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace TestProject1
{
    public class UnitTest1
    {
        private DbContextOptions<LibraryContext> GetInMemoryOptions()
        {
            return new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestLibrary") 
                .Options;
        }

        [Fact]
        public async Task AddBook_ShouldReturnCreatedAtAction_WhenBookIsValid()
        {
            var options = GetInMemoryOptions();

            using var context = new LibraryContext(options);
            var controller = new BooksController(context);
            var newBook = new Book
            {
                Title = "Test Book",
                Author = "Test Author",
                Article = "123ABC",
                YearPublication = 2023,
                Count = 5
            };

            var result = await controller.AddBook(newBook);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(BooksController.GetBookById), actionResult.ActionName);

            var createdBook = Assert.IsType<Book>(actionResult.Value);
            Assert.Equal(newBook.Title, createdBook.Title);

            var bookInDb = await context.Books.FirstOrDefaultAsync();
            Assert.NotNull(bookInDb);
            Assert.Equal(newBook.Title, bookInDb.Title);
        }

        [Fact]
        public async Task AddBook_ShouldReturnBadRequest_WhenBookIsInvalid()
        {
            var options = GetInMemoryOptions();

            using var context = new LibraryContext(options);
            var controller = new BooksController(context);
            var invalidBook = new Book
            {
                Title = "",
                Author = "Test Author",
                Article = "123ABC",
                YearPublication = 2023,
                Count = 5
            };

            var result = await controller.AddBook(invalidBook);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Данные некорректно заполнены.", badRequestResult.Value);
        }
        [Fact]
        public async Task BorrowBook_ShouldReturnOk_WhenBookIsSuccessfullyBorrowed()
        {
            var options = GetInMemoryOptions();

            using var context = new LibraryContext(options);
            context.Readers.Add(new Reader {  Name = "Роман", LastName = "Корнеев" , MiddleName = "Александрович" ,DayOfBirthday = new DateTime(1995, 5, 15) });
            context.Books.Add(new Book {  Title = "C# для начинающих", Count = 1 , Article = "ABC123" ,Author = "billiboba" , YearPublication = 1999});
            await context.SaveChangesAsync();

            var controller = new ReadersController(context);

            var result = await controller.BorrowBook(1, 1);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("книга выдана читателю", actionResult.Value);
        }
    }
}