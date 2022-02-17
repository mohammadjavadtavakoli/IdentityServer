using IdentityModel;
using IdentityServer.Identity.Models;
using IdentityServer.Identity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Identity.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _UserService;

        public AccountController(IUserService userService)
        {
            _UserService = userService;
        }

        public IActionResult Login(string ReturnUrl)
        {
           return View(new Login {RedirectUrl= ReturnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(Login login)
        {
            if(!ModelState.IsValid)
            {
                return View(login);
            }
           var user= _UserService.GetUserByUserNameAndPassword(login.Username, login.Password);
            if(user==null)
            {
                return View(login);
            }
            var clientClaim = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject,user.UserId.ToString()),
                new Claim(JwtClaimTypes.Name,user.UserName),
                new Claim(JwtClaimTypes.Role,"user")
            };
            var identity = new ClaimsIdentity(clientClaim, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
           await HttpContext.SignInAsync(principal, new AuthenticationProperties());
            return Redirect(login.RedirectUrl);
        }
    }
}
