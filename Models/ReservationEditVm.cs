using System.ComponentModel.DataAnnotations;

namespace DentalBookingMvc.Models;

public class ReservationEditVm
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int ServiceId { get; set; }

    [Required]
    public int DentistId { get; set; }

    [Required(ErrorMessage = "Wybierz termin.")]
    public int TimeSlotId { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }
}
