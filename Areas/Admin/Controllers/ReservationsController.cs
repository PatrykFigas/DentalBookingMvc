using DentalBookingMvc.Data;
using DentalBookingMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalBookingMvc.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ReservationsController : Controller
{
    private readonly AppDbContext _db;

    public ReservationsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /Admin/Reservations
    public async Task<IActionResult> Index()
    {
        var list = await _db.Reservations
            .Include(r => r.User)
            .Include(r => r.Service)
            .Include(r => r.TimeSlot)!.ThenInclude(t => t!.Dentist)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var r = await _db.Reservations
            .Include(x => x.TimeSlot)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (r == null) return NotFound();

        // nie zmieniaj anulowanych/odrzuconych (opcjonalnie)
        if (r.Status == ReservationStatus.Cancelled || r.Status == ReservationStatus.Rejected)
        {
            TempData["Error"] = "Nie można zatwierdzić anulowanej/odrzuconej rezerwacji.";
            return RedirectToAction(nameof(Index));
        }

        r.Status = ReservationStatus.Approved;
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Rezerwacja zatwierdzona.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var r = await _db.Reservations
            .Include(x => x.TimeSlot)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (r == null) return NotFound();

        // przy odrzuceniu zwalniamy termin
        r.Status = ReservationStatus.Rejected;
        if (r.TimeSlot != null)
            r.TimeSlot.IsBooked = false;

        await _db.SaveChangesAsync();

        TempData["Ok"] = "Rezerwacja odrzucona (termin zwolniony).";
        return RedirectToAction(nameof(Index));
    }
}
