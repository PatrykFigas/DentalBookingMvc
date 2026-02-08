using System.ComponentModel.DataAnnotations;

namespace DentalBookingMvc.Models;

public enum ReservationStatus
{
    Pending = 0,
    Approved = 1,
    Cancelled = 2,
    Rejected = 3
}

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int ServiceId { get; set; }
    public Service? Service { get; set; }

    [Required]
    public int TimeSlotId { get; set; }
    public TimeSlot? TimeSlot { get; set; }

    [Required]
    public string UserId { get; set; } = default!;
    public AppUser? User { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
