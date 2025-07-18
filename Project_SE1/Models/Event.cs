using System;
using System.Collections.Generic;

namespace project_dnc_se1.Models;

public partial class Event
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Value { get; set; }

    public int? CategoryId { get; set; }

    public string? Banners { get; set; }

    public int? UserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual EventsCategory? Category { get; set; }

    public virtual ICollection<ContactInfo> ContactInfos { get; set; } = new List<ContactInfo>();

    public virtual ICollection<EventsProduct> EventsProducts { get; set; } = new List<EventsProduct>();

    public virtual User? User { get; set; }
}
