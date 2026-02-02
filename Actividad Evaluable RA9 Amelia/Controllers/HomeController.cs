using Microsoft.AspNetCore.Mvc;

namespace Actividad_Evaluable_RA9_Amelia.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
