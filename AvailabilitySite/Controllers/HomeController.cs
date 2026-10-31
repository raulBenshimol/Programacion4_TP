using Microsoft.AspNetCore.Mvc;

namespace AvailabilitySite.Controllers;

public class HomeController : Controller
{
    // Muestra la página "Acerca del sistema".
    public IActionResult Index()
    {
        return View();
    }

    // Muestra la página "Privacidad".
    public IActionResult Privacy()
    {
        return View();
    }
}
