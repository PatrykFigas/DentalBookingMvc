using DentalBookingMvc.Data;
using DentalBookingMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalBookingMvc.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TimeSlotsController : Controller
{
    private readonly AppDbContext _db;

    public TimeSlotsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _db.TimeSlots
            .Include(t => t.Dentist)
            .OrderBy(t => t.Start)
            .ToListAsync();

        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Dentists = await _db.Dentists
            .Where(d => d.IsActive)
            .OrderBy(d => d.FullName)
            .ToListAsync();

        return View(new TimeSlot
        {
            Start = DateTime.Now.AddHours(1),
            DurationMinutes = 30,
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("DentistId,Start,DurationMinutes,IsActive")] TimeSlot model)
    {
        if (model.DentistId <= 0)
            ModelState.AddModelError(nameof(model.DentistId), "Wybierz dentystę.");

        if (model.Start == default)
            ModelState.AddModelError(nameof(model.Start), "Podaj datę i godzinę.");

        if (model.DurationMinutes < 5 || model.DurationMinutes > 480)
            ModelState.AddModelError(nameof(model.DurationMinutes), "Czas trwania musi być 5–480 min.");

        if (!ModelState.IsValid)
        {
            ViewBag.Dentists = await _db.Dentists
                .Where(d => d.IsActive)
                .OrderBy(d => d.FullName)
                .ToListAsync();

            return View(model);
        }

        model.IsBooked = false;

        _db.TimeSlots.Add(model);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Dodano termin.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var slot = await _db.TimeSlots.FindAsync(id);
        if (slot == null)
        {
            TempData["Error"] = "Nie znaleziono terminu.";
            return RedirectToAction(nameof(Index));
        }

        var hasReservation = await _db.Reservations.AnyAsync(r => r.TimeSlotId == id);
        if (hasReservation)
        {
            TempData["Error"] = "Nie można usunąć terminu, który ma rezerwację.";
            return RedirectToAction(nameof(Index));
        }

        _db.TimeSlots.Remove(slot);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Usunięto termin.";
        return RedirectToAction(nameof(Index));
    }
}
