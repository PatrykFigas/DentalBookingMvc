using System.ComponentModel.DataAnnotations;

namespace DentalBookingMvc.Models;

public class ReservationCreateVm
{
    [Required]
    public int ServiceId { get; set; }

    [Required(ErrorMessage = "Wybierz dentystę.")]
    public int DentistId { get; set; }

    [Required(ErrorMessage = "Wybierz termin.")]
    public int TimeSlotId { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }
}
