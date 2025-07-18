using System;
using System.Collections.Generic;

namespace project_dnc_se1.Models;

public partial class EventsCategory
{
    public int Id { get; set; }

    public string? Title { get; set; }
    public bool? IsDeleted { get; set; } = false;
    public virtual ICollection<Event> Events { get; set; } = [];
}
