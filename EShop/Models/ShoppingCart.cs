using EShop.Models.Identity;

namespace EShop.Models
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }

        public string EShopIdentityUserId { get; set; } = string.Empty;
        public EShopIdentityUser User { get; set; }
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}
