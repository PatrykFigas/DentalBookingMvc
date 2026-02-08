using DentalBookingMvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalBookingMvc.Controllers;

public class ServicesController : Controller
{
    private readonly AppDbContext _db;

    public ServicesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var services = await _db.Services
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(services);
    }
}
