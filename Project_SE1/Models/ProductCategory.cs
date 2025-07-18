using System;
using System.Collections.Generic;

namespace project_dnc_se1.Models;

public partial class ProductCategory
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
