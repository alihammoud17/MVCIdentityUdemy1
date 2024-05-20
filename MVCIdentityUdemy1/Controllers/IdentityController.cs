using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCIdentityUdemy1.Models;
using MVCIdentityUdemy1.Services;

namespace MVCIdentityUdemy1.Controllers
{
    public class IdentityController(UserManager<IdentityUser> _userManager, IEmailSender _emailSender) : Controller
    {

        public Task<IActionResult> Signup()
        {
            var model = new SignupViewModel();
            return Task.FromResult<IActionResult>(View(model));
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if(ModelState.IsValid)
            {
                if((await _userManager.FindByEmailAsync(model.Email)) != null)
                {
                    IdentityUser user = new()
                    {
                        Email = model.Email,
                        UserName = model.Email
                    };

                    IdentityResult res = await _userManager.CreateAsync(user, model.Password);

                    user = await _userManager.FindByEmailAsync(model.Email);

                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);    

                    if (res.Succeeded) 
                    {
                        string confirmationLink =  Url.ActionLink("ConfirmEmail", "Identity", new { userId = user.Id, token });

                        await _emailSender.SendEmailAsync(user.Email, "Confirm Registration", confirmationLink);

                        return RedirectToAction("Signin"); 
                    }

                    ModelState.AddModelError("Signup", string.Join('\n', res.Errors.Select(x => x.Description)));

                    return View(model);
                }

                return RedirectToAction("Signin");
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if(result.Succeeded)
            {
                return RedirectToAction("Signin");

            }

            return Unauthorized();
        }

        public async Task<IActionResult> Signin()
        {
            return View();
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
    }
}
