using System.Web.Mvc;

namespace SouthlandMetals.Web.Controllers
{
    public class HomeController : ApplicationBaseController
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}