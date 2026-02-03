using BookTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookTracking.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(BookTrackingDbContext context)
    {        

        if (!context.Books.Any())
        {
            var rowling = new Author { Name = "J.K. Rowling", IsActive = true, CreatedAt = DateTime.UtcNow };
            var tolkien = new Author { Name = "J.R.R. Tolkien", IsActive = true, CreatedAt = DateTime.UtcNow };
            var orwell = new Author { Name = "George Orwell", IsActive = true, CreatedAt = DateTime.UtcNow };

            var books = new List<Book>
            {
                new Book
                {
                    Title = "Harry Potter and the Philosopher's Stone",
                    Description = "A young wizard's journey begins.",
                    Isbn = "9780747532743",
                    PublishDate = new DateOnly(1997, 6, 26),
                    Authors = new List<Author> { rowling },
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    Title = "The Hobbit",
                    Description = "In a hole in the ground there lived a hobbit.",
                    Isbn = "9780547928227",
                    PublishDate = new DateOnly(1937, 9, 21),
                    Authors = new List<Author> { tolkien },
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    Title = "The Lord of the Rings",
                    Description = "One Ring to rule them all.",
                    Isbn = "9780618640157",
                    PublishDate = new DateOnly(1954, 7, 29),
                    Authors = new List<Author> { tolkien },
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    Title = "1984",
                    Description = "Big Brother is watching you.",
                    Isbn = "9780451524935",
                    PublishDate = new DateOnly(1949, 6, 8),
                    Authors = new List<Author> { orwell },
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Books.AddRange(books);
            context.SaveChanges();
        }
    }
}
