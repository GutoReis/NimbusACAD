using System.Web.Mvc;

namespace NimbusACAD.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Desenvolverdores";

            return View();
        }

        public ActionResult Assinatura()
        {
            ViewBag.Message = "Assinatura";

            return View();
        }
    }
}