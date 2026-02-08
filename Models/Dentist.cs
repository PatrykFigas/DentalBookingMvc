using System.ComponentModel.DataAnnotations;

namespace DentalBookingMvc.Models;

public class Dentist
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(120)]
    public string Specialty { get; set; } = "Stomatologia ogólna";

    public bool IsActive { get; set; } = true;

    public List<TimeSlot> TimeSlots { get; set; } = new();
}
