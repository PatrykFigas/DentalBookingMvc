using Microsoft.AspNetCore.Mvc;

namespace DentalBookingMvc.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
