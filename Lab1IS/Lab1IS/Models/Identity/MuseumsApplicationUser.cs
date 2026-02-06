using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Lab1IS.Models.Identity
{
    public class MuseumsApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;
    }
}
