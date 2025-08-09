using Microsoft.AspNetCore.Mvc;
using project_dnc_se1.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;

namespace project_dnc_se1.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CompanyWebContext _context;

        public HomeController(ILogger<HomeController> logger, CompanyWebContext context) : base(context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var bannerInfo = _context.CompanyInfos.FirstOrDefault(c => c.Title == "homeBanner");
            List<string> bannerImages = new();
            if (bannerInfo != null && !string.IsNullOrWhiteSpace(bannerInfo.Value))
            {
                bannerImages = bannerInfo.Value
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();
            }

            ViewData["BannerImages"] = bannerImages;

            // products
            var productList = _context.Products
                .Where(p => p.CreatedAt != null)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToList();
            ViewData["productList"] = productList;

            // newest
            var newestNews = _context.News
                .Where(n => n.IsDeleted != true && n.UpdatedAt != null)
                .OrderByDescending(n => n.UpdatedAt)
                .FirstOrDefault();
            ViewData["newestNews"] = newestNews;

            //  news
            var featuredNewsList = _context.News
                .Where(n => n.IsDeleted != true && n.UpdatedAt != null && n.Id != newestNews.Id)
                .OrderByDescending(n => n.UpdatedAt)
                .Take(8)
                .ToList();
            ViewData["featuredNewsList"] = featuredNewsList;

            //events
            var latestEvents = _context.Events
    .Where(e => e.IsDeleted != true && e.UpdatedAt != null)
    .OrderByDescending(e => e.UpdatedAt)
    .Take(3)
    .ToList();

            ViewData["LatestEvents"] = latestEvents;

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
