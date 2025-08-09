using System;
using System.Collections.Generic;

namespace project_dnc_se1.Models;

public partial class CompanyInfo
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Value { get; set; }
}
public class CompanyInfoViewModel
{
    public string Title { get; set; }

    public string ValueText { get; set; }

    public IFormFile[] ValueImage { get; set; }

    public string ValueType { get; set; } // "text" hoặc "image"
}

