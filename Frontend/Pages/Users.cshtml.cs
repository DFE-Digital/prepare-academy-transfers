using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontend.Pages
{
    public class UsersModel : PageModel
    {
        private readonly IUserRepository _userRepository;

        public UsersModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<User> Users { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Users = await _userRepository.GetAllUsers();

            return Page();
        }
    }
}
