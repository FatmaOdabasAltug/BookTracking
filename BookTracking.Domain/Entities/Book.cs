using System;
using System.Collections.Generic;
using BookTracking.Domain.Common;

namespace BookTracking.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Author> Authors { get; set; } = new List<Author>();
}