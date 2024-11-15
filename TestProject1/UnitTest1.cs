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
        private LibraryContext CreateTestContext()
        {
            var options = GetInMemoryOptions();
            var context = new LibraryContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated(); 
            return context;
        }

       
        [Fact]
        public async Task BorrowBook_ShouldReturnOk_WhenBookIsSuccessfullyBorrowed()
        {
            var options = GetInMemoryOptions();

            using var context = new LibraryContext(options);
            context.Readers.Add(new Reader { Name = "Роман", LastName = "Корнеев", MiddleName = "Александрович", DayOfBirthday = new DateTime(1995, 5, 15) });
            context.Books.Add(new Book { Title = "C# для начинающих", Count = 1, Article = "ABC123", Author = "billiboba", YearPublication = 1999 });
            await context.SaveChangesAsync();

            var controller = new ReadersController(context);

            var result = await controller.BorrowBook(1, 1);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("книга выдана читателю", actionResult.Value);
        }

        /// <summary>
        /// Проверка на удаление читателя. Если читатель существует.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteReaderById()
        {
            var options = GetInMemoryOptions();

            using (var context = new LibraryContext(options))
            {
                context.Readers.Add(new Reader
                {
                    Name = "Иван",
                    LastName = "Иванов",
                    MiddleName = "Иванович",
                    DayOfBirthday = new DateTime(2003, 11, 19)
                });
                await context.SaveChangesAsync();
            }

            using (var context = new LibraryContext(options))
            {
                var controller = new ReadersController(context);

                var result = await controller.DeleteById(1);

                Assert.IsType<NoContentResult>(result);
                Assert.Null(await context.Readers.FindAsync(1)); 
            }
        }

        /// <summary>
        /// Проверка на удаление читателя. Если читателя не существует.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteReaderById_ifReaderNull()
        {
            var options = GetInMemoryOptions();

            using var context = new LibraryContext(options);
            
            var controller = new ReadersController(context);
            var result = await controller.DeleteById(11);
            Assert.IsType<NotFoundResult>(result);
           
        }
    }
}