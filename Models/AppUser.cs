using Microsoft.AspNetCore.Identity;

namespace DentalBookingMvc.Models;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
}
