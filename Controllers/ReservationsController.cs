using DentalBookingMvc.Data;
using DentalBookingMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalBookingMvc.Controllers;

[Authorize]
public class ReservationsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public ReservationsController(AppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }


    public async Task<IActionResult> ChooseDentist(int serviceId)
    {
        var service = await _db.Services.FirstOrDefaultAsync(s => s.Id == serviceId);
        if (service == null) return NotFound();

        var dentists = await _db.Dentists
            .Where(d => d.IsActive)
            .OrderBy(d => d.FullName)
            .ToListAsync();

        ViewBag.Service = service;
        return View(dentists);
    }

  
    public async Task<IActionResult> Create(int serviceId, int dentistId)
    {
        var service = await _db.Services.FirstOrDefaultAsync(s => s.Id == serviceId);
        if (service == null) return NotFound();

        var slots = await _db.TimeSlots
            .Include(t => t.Dentist)
            .Where(t => t.IsActive && !t.IsBooked && t.Start >= DateTime.Today && t.DentistId == dentistId)
            .OrderBy(t => t.Start)
            .Take(50)
            .ToListAsync();

        ViewBag.Service = service;
        ViewBag.Slots = slots;
        ViewBag.DentistId = dentistId;

        return View(new ReservationCreateVm
        {
            ServiceId = serviceId,
            DentistId = dentistId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationCreateVm vm)
    {
        if (!ModelState.IsValid)
            return await Create(vm.ServiceId, vm.DentistId);

        var slot = await _db.TimeSlots
            .FirstOrDefaultAsync(t => t.Id == vm.TimeSlotId && t.DentistId == vm.DentistId);

        if (slot == null || slot.IsBooked || !slot.IsActive)
        {
            ModelState.AddModelError("", "Wybrany termin jest już niedostępny.");
            return await Create(vm.ServiceId, vm.DentistId);
        }

        var userId = _userManager.GetUserId(User)!;

        var reservation = new Reservation
        {
            ServiceId = vm.ServiceId,
            TimeSlotId = vm.TimeSlotId,
            UserId = userId,
            Note = vm.Note?.Trim(),
            Status = ReservationStatus.Pending
        };

        slot.IsBooked = true;

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(My));
    }


    public async Task<IActionResult> My()
    {
        var userId = _userManager.GetUserId(User)!;

        var list = await _db.Reservations
            .Include(r => r.Service)
            .Include(r => r.TimeSlot)!.ThenInclude(t => t!.Dentist)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(list);
    }
 
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User)!;

        var r = await _db.Reservations
            .Include(x => x.TimeSlot)!.ThenInclude(t => t!.Dentist)
            .Include(x => x.Service)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (r == null) return NotFound();

   
        if (r.Status == ReservationStatus.Cancelled || r.Status == ReservationStatus.Rejected)
            return RedirectToAction(nameof(My));

        var dentistId = r.TimeSlot!.DentistId;

        var slots = await _db.TimeSlots
            .Include(t => t.Dentist)
            .Where(t => t.IsActive && !t.IsBooked && t.Start >= DateTime.Today && t.DentistId == dentistId)
            .OrderBy(t => t.Start)
            .Take(50)
            .ToListAsync();

       
        var currentSlot = await _db.TimeSlots
            .Include(t => t.Dentist)
            .FirstAsync(t => t.Id == r.TimeSlotId);

        if (!slots.Any(s => s.Id == currentSlot.Id))
            slots.Insert(0, currentSlot);

        ViewBag.Service = r.Service;
        ViewBag.Slots = slots;

        return View(new ReservationEditVm
        {
            Id = r.Id,
            ServiceId = r.ServiceId,
            DentistId = dentistId,
            TimeSlotId = r.TimeSlotId,
            Note = r.Note
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ReservationEditVm vm)
    {
        var userId = _userManager.GetUserId(User)!;

        var r = await _db.Reservations
            .Include(x => x.TimeSlot)
            .FirstOrDefaultAsync(x => x.Id == vm.Id && x.UserId == userId);

        if (r == null) return NotFound();

        if (!ModelState.IsValid)
            return await Edit(vm.Id);

        // jeśli zmieniono slot, to zwalniamy stary i rezerwujemy nowy
        if (r.TimeSlotId != vm.TimeSlotId)
        {
            var newSlot = await _db.TimeSlots
                .FirstOrDefaultAsync(t => t.Id == vm.TimeSlotId && t.DentistId == vm.DentistId);

            if (newSlot == null || newSlot.IsBooked || !newSlot.IsActive)
            {
                ModelState.AddModelError("", "Wybrany termin jest niedostępny.");
                return await Edit(vm.Id);
            }

            // zwolnij stary
            if (r.TimeSlot != null) r.TimeSlot.IsBooked = false;

            // zarezerwuj nowy
            newSlot.IsBooked = true;
            r.TimeSlotId = newSlot.Id;
        }

        r.Note = vm.Note?.Trim(); 

       
        r.Status = ReservationStatus.Pending;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(My));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = _userManager.GetUserId(User)!;

        var r = await _db.Reservations
            .Include(x => x.TimeSlot)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (r == null) return NotFound();


        if (r.Status == ReservationStatus.Cancelled || r.Status == ReservationStatus.Rejected)
        {
            TempData["Error"] = "Tej rezerwacji nie można już anulować.";
            return RedirectToAction(nameof(My));
        }

        // BLOKADA < 24h (admin może zawsze)
        if (!User.IsInRole("Admin") && r.TimeSlot != null)
        {
            var now = DateTime.Now;
            if (r.TimeSlot.Start <= now.AddHours(24))
            {
                TempData["Error"] = "Nie można anulować wizyty na mniej niż 24 godziny przed terminem.";
                return RedirectToAction(nameof(My));
            }
        }

        r.Status = ReservationStatus.Cancelled;

        if (r.TimeSlot != null)
            r.TimeSlot.IsBooked = false;

        await _db.SaveChangesAsync();

        TempData["Ok"] = "Rezerwacja została anulowana.";
        return RedirectToAction(nameof(My));
    }

}
