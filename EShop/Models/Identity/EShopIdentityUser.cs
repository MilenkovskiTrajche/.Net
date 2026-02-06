using Microsoft.AspNetCore.Identity;

namespace EShop.Models.Identity
{
    public class EShopIdentityUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
