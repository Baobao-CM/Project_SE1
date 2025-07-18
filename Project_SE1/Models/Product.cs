using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace project_dnc_se1.Models;

public partial class Product
{
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }
    [Required]
    public string? Description { get; set; }

    [NotMapped]
    public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
    public string ImageUrls { get; set; } = ""; // ảnh chi tiết (nhiều ảnh)

    public int? CategoryId { get; set; }

    public bool IsFeatured { get; set; }

    [NotMapped]
    public IFormFile? ThumbNailFile { get; set; }
    public string? ThumbNail { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ProductCategory? Category { get; set; }

    public virtual ICollection<ContactInfo> ContactInfos { get; set; } = new List<ContactInfo>();

    public virtual ICollection<EventsProduct> EventsProducts { get; set; } = new List<EventsProduct>();
}
