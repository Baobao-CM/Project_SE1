using System;
using System.Collections.Generic;

namespace project_dnc_se1.Models;

public partial class EventsProduct
{
    public int Id { get; set; }

    public int? EventsId { get; set; }

    public int? ProductId { get; set; }

    public virtual Event? Events { get; set; }

    public virtual Product? Product { get; set; }
}
