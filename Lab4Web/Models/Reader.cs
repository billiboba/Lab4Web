namespace Lab4Web.Models
{
    public class Reader
    {
        public string LastName { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public DateTime DayOfBirthday { get; set; }
        public ICollection<Book> BorrowedBooks { get; set; }    
    }
}
