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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        private LibraryContext CreateTestContext()
        {
            var options = GetInMemoryOptions();
            var context = new LibraryContext(options);
            context.Database.EnsureCreated();
            return context;
        }


        /// <summary>
        /// �������� �� �������� ��������. ���� �������� ����������.
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
                    Name = "����",
                    LastName = "������",
                    MiddleName = "��������",
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
        /// �������� �� �������� ��������. ���� �������� �� ����������.
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

        [Fact]
        public async Task AddReader_IsValid()
        {
            var options = GetInMemoryOptions();
            using var context = new LibraryContext(options);

            var controller = new ReadersController(context);

            var newReader = new Reader
            {
                Name = "�����",
                LastName = "�������",
                MiddleName = "�������������",
                DayOfBirthday = new DateTime(2003, 11, 19)
            };

            var result = await controller.AddReader(newReader);

            var actionResult = Assert.IsType<ActionResult<Reader>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var addedReader = Assert.IsType<Reader>(createdAtActionResult.Value);

            Assert.Equal("�����", addedReader.Name);
            Assert.Equal("�������", addedReader.LastName);
            Assert.Equal("�������������", addedReader.MiddleName);
            Assert.Equal(new DateTime(2003, 11, 19), addedReader.DayOfBirthday);

            var savedReader = await context.Readers.FirstOrDefaultAsync(r => r.Id == addedReader.Id);
            Assert.NotNull(savedReader);
            Assert.Equal("�����", savedReader.Name);
            Assert.Equal("�������", savedReader.LastName);
            Assert.Equal("�������������", savedReader.MiddleName);
            Assert.Equal(new DateTime(2003, 11, 19), savedReader.DayOfBirthday);
        }
        [Fact]
        public async Task AddReader_ReturnsBadRequest_WhenNameIsEmpty()
        {
            // Arrange: ������� �������� ��������
            var options = GetInMemoryOptions();
            using var context = new LibraryContext(options);

            var controller = new ReadersController(context);

            var newReader = new Reader
            {
                Name = "", // ��� ������
                LastName = "�������",
                MiddleName = "�������������",
                DayOfBirthday = new DateTime(2003, 11, 19)
            };

            // Act: �������� ����� �����������
            var result = await controller.AddReader(newReader);

            // Assert: ��������� ���������
            var actionResult = Assert.IsType<ActionResult<Reader>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);

            // ��������� ��������� �� ������
            Assert.Equal("������ �����������", badRequestResult.Value);

            // ��������, ��� ������ �� ��� �������� � ���� ������
            var savedReader = await context.Readers.FirstOrDefaultAsync(r => r.LastName == "�������");
            Assert.Null(savedReader);
        }









        [Fact]
        public async Task BorrowBook_ShouldReturnOk_WhenBookIsSuccessfullyBorrowed()
        {
            var options = GetInMemoryOptions();

            using var context = new LibraryContext(options);
            context.Readers.Add(new Reader { Name = "�����", LastName = "�������", MiddleName = "�������������", DayOfBirthday = new DateTime(1995, 5, 15) });
            context.Books.Add(new Book { Title = "C# ��� ����������", Count = 1, Article = "ABC123", Author = "billiboba", YearPublication = 1999 });
            await context.SaveChangesAsync();

            var controller = new ReadersController(context);

            var result = await controller.BorrowBook(1, 1);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("����� ������ ��������", actionResult.Value);
        }
    }
}