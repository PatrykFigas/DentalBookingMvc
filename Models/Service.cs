namespace DentalBookingMvc.Models;
using System.ComponentModel.DataAnnotations;

public class Service
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 99999)]
    public decimal Price { get; set; }

    [Range(5, 240)]
    public int DefaultDurationMinutes { get; set; } = 30;

    public List<Reservation> Reservations { get; set; } = new();
}
