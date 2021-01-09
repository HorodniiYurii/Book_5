using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book50
{
    class Program
    {
        public class Book
        {
            public int Id { get; set; }
            public string Title { get; set; }
           
            public int AuthorId { get; set; }
            public BookReview BookReview { get; set; }
        }
        public class BookReview
        {

            public int Id { get; set; }
            public string ReviewerName { get; set; }
            public int Rating { get; set; }
            public Book Book { get; set; }
            public int BookId { get; set; }
        }
        public class Author
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class DTO
        {
            public string AuthorName { get; set; }
            public string BookTitle { get; set; }
            public int Rating { get; set; }

            public List<DTO> GetDTO()
            {
                List<DTO> dto = new List<DTO>();
                using (BooksContext ctx = new BooksContext())
                {
                    var dTOs = from author in ctx.Authors
                               join book in ctx.Books on author.Id equals book.AuthorId
                               join br in ctx.BookReviews on book.Id equals br.BookId
                               where (book.BookReview != null)
                               select new DTO()
                               {
                                   AuthorName = author.Name,
                                   BookTitle = book.Title,
                                   Rating = br.Rating
                               };
                    return dTOs.ToList();
                }

            }
        }
        public class BooksContext : DbContext
        {
            public DbSet<Book> Books { get; set; }
            public DbSet<BookReview> BookReviews { get; set; }
            public DbSet<Author> Authors { get; set; }
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

                modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired().HasMaxLength(512);
                modelBuilder.Entity<BookReview>().Property(r => r.ReviewerName).IsRequired();
                modelBuilder.Entity<Author>().Property(a => a.Name).IsRequired();

                modelBuilder.Entity<Book>()
                    .HasOptional(p => p.BookReview)
                    .WithOptionalPrincipal(p => p.Book);

            }
        }

        static void Main(string[] args)
        {
            DTO dTO = new DTO();
            var dtoList = dTO.GetDTO();
        }
    }
}
