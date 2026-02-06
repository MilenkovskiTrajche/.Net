using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab1IS.Models
{
    public class Visitor
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime MembershipDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string MembershipType { get; set; } = string.Empty;

        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}
