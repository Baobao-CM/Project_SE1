using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using project_dnc_se1.Models;

namespace project_dnc_se1.Controllers
{
    public class BaseController : Controller
    {
        private readonly CompanyWebContext _context;

        public BaseController(CompanyWebContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Hotline
            var hotline = _context.CompanyInfos.FirstOrDefault(c => c.Title == "hotline");
            ViewData["Hotline"] = hotline?.Value ?? "";

            // address
            var address = _context.CompanyInfos.FirstOrDefault(c => c.Title == "address");
            ViewData["Address"] = address?.Value ?? "";

            var email = _context.CompanyInfos.FirstOrDefault(c => c.Title == "email");
            ViewData["Email"] = email?.Value ?? "";


            // Social Links
            ViewData["Facebook"] = _context.CompanyInfos.FirstOrDefault(c => c.Title == "facebook")?.Value ?? "#";
            ViewData["TikTok"] = _context.CompanyInfos.FirstOrDefault(c => c.Title == "tiktok")?.Value ?? "#";
            ViewData["Instagram"] = _context.CompanyInfos.FirstOrDefault(c => c.Title == "instagram")?.Value ?? "#";
            ViewData["YouTube"] = _context.CompanyInfos.FirstOrDefault(c => c.Title == "youtube")?.Value ?? "#";

            base.OnActionExecuting(context);
        }


    }
}
