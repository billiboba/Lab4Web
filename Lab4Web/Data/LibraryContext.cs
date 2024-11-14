using Lab4Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Lab4Web.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Reader> Readers { get; set; }
    }

}
