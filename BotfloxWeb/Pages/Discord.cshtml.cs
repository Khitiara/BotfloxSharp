using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BotfloxWeb.Pages
{
    public class Discord : PageModel
    {
        public IActionResult OnGet() => Redirect("https://discord.gg/NvQ5Udx");
    }
}