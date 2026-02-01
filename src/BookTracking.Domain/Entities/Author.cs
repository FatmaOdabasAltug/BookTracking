using System.Collections.Generic;
using BookTracking.Domain.Common;

namespace BookTracking.Domain.Entities;

public class Author : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>();
}