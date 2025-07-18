using Microsoft.AspNetCore.Mvc;

namespace project_dnc_se1.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            int? role = HttpContext.Session.GetInt32("UserRole");

            if (role == null || (role != 0 && role != 1))
                return RedirectToAction("Login", "Users");

            return View("~/Views/Admin/Dashboard.cshtml"); // hoặc View("Dashboard") nếu dùng thư mục Views/Dashboard

        }
    }


}
