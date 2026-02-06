using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Web.Pages.Admin
{

    [Authorize(Roles = "Admin")]
    public class indexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
