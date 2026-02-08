using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DentalBookingMvc.Models;

public class TimeSlot
{
    public int Id { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Range(5, 480)]
    public int DurationMinutes { get; set; } = 30;

    public bool IsActive { get; set; } = true;
    public bool IsBooked { get; set; } = false;

    [Required]
    public int DentistId { get; set; }

   
    [ValidateNever]
    public Dentist Dentist { get; set; } = null!;

    
    [ValidateNever]
    public List<Reservation> Reservations { get; set; } = new();
}
