using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BotfloxWeb.Pages
{
    public class Profiles : PageModel
    {
        [BindProperty]
        [Required]
        [Display(Name = "Character Lodestone Id")]
        public ulong LodestoneId { get; set; } = 0;

        public void OnGet() { }

        public IActionResult OnPost() {
            if (ModelState.IsValid)
                return RedirectToPage("Profile", new {lodestoneId = LodestoneId});
            return Page();
        }
    }
}