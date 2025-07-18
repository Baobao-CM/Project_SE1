using System;
using System.Collections.Generic;

namespace project_dnc_se1.Models;

public partial class ContactInfo
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Email { get; set; }

    public string? Message { get; set; }

    public int? ProductId { get; set; }

    public int? EventsId { get; set; }

    public virtual Event? Events { get; set; }

    public virtual Product? Product { get; set; }
}
