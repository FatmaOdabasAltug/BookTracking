using System;
using System.Collections.Generic;
using BookTracking.Domain.Common;

namespace BookTracking.Domain.Entities;

public class Book : BaseEntity
{
    public required string Isbn { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateOnly PublishDate { get; set; }
    public required ICollection<Author> Authors { get; set; }
}