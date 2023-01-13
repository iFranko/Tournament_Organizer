using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tournament_Organizer.Data;
using Tournament_Organizer.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Tournament_Organizer.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TournamentOrganizerContext _context;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager, TournamentOrganizerContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
          
            [Display(Name = "Age")]
            public uint Age { get; set; }

            [Display(Name = "Image")]
            public byte[] Image { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var age = user.Age;
            var image = user.Image;


            Username = userName;
            if (user.Image != null)
            {
                var base64 = Convert.ToBase64String(user.Image);
                string imgsrc = string.Format("data:image/*;base64,{0}", base64);
                ViewData["imgsrc"] = imgsrc;
                ViewData["imgValue"] = Convert.ToBase64String(user.Image); 
            }


            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Age = age,

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile> Image, string imgValue)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            var users = await _context.Users.FindAsync(user.Id);
            users.Age = Input.Age;


            if (Image.Count == 0)
            {
                if (imgValue != null)
                {
                    byte[] chartData = Convert.FromBase64String(imgValue);
                    users.Image = chartData;
                }
            }
            else
            {
                foreach (var item in Image)
                {
                    if (item.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await item.CopyToAsync(stream);
                            users.Image = stream.ToArray();
                        }
                    }
                }
            }
            
            _context.Update(users);
            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
