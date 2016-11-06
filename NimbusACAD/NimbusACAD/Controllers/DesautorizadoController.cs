using System.Web.Mvc;

namespace NimbusACAD.Controllers
{
    public class DesautorizadoController : Controller
    {
        // GET: Desautorizado
        public ActionResult Erro(string _errorMsg)
        {
            ViewBag.ErrorMsg = _errorMsg;
            return View();
        }
    }
}