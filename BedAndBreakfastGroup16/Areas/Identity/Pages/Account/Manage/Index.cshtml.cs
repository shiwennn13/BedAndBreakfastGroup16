// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BedAndBreakfastGroup16.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BedAndBreakfastGroup16.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<BedAndBreakfastGroup16User> _userManager;
        private readonly SignInManager<BedAndBreakfastGroup16User> _signInManager;

        public IndexModel(
            UserManager<BedAndBreakfastGroup16User> userManager,
            SignInManager<BedAndBreakfastGroup16User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Required(ErrorMessage = "Please enter Customer Name first!")]
            [Display(Name = "Customer Full Name")]
            [StringLength(50, ErrorMessage = "Between 5 to 50 characters", MinimumLength = 5)]
            public string CustomerName { get; set; }
            [Required(ErrorMessage = "Please enter Customer Age first!")]
            [Display(Name = "Customer Age")]
            [Range(18, 99, ErrorMessage = "Only allow 18 - 99 years old adults to register")]
            public int CustomerAge { get; set; }
            [Required(ErrorMessage = "Please enter Customer Address first!")]
            [Display(Name = "Customer Address")]
            public string CustomerAddress { get; set; }
            [Required(ErrorMessage = "Please enter Customer DoB first!")]
            [Display(Name = "Customer DoB")]
            [DataType(DataType.Date)]
            public DateTime CustomerDoB { get; set; }

        }

        private async Task LoadAsync(BedAndBreakfastGroup16User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                CustomerName=user.CustomerFullName,
                CustomerAge=user.CustomerAge,
                CustomerDoB=user.CustomerDoB,
                CustomerAddress=user.CustomerAddress
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

        public async Task<IActionResult> OnPostAsync()
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

            if (Input.CustomerName != user.CustomerFullName)
            {
                user.CustomerFullName = Input.CustomerName;
            }
            if (Input.CustomerAge != user.CustomerAge)
            {
                user.CustomerAge = Input.CustomerAge;
            }
            if (Input.CustomerDoB != user.CustomerDoB)
            {
                user.CustomerDoB = Input.CustomerDoB;
            }
            if (Input.CustomerAddress != user.CustomerAddress)
            {
                user.CustomerAddress = Input.CustomerAddress;
            }
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
