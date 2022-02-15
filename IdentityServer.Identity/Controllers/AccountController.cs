using IdentityModel;
using IdentityServer.Identity.Models;
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
        public IActionResult Login(string ReturnUrl)
        {
           return View(new Login {RedirectUrl= ReturnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(Login login)
        {
            var clientClaim = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject,"MohammadJavad"),
                new Claim(JwtClaimTypes.FamilyName,"Tavakoli"),
                new Claim(JwtClaimTypes.Role,"user")
            };
            var identity = new ClaimsIdentity(clientClaim, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
           await HttpContext.SignInAsync(principal, new AuthenticationProperties());
            return Redirect(login.RedirectUrl);
        }
    }
}
