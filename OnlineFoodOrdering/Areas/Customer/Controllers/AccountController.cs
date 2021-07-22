using Microsoft.AspNetCore.Mvc;

namespace OnlineFoodOrdering.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OrderHistory()
        {
            return PartialView("..Admin/Views/Category/Index.cshtml");
        }
    }
}