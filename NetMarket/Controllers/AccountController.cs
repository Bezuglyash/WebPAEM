using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetMarket.Models;
using NetMarket.Repository;
using NetMarket.ViewModels;

namespace NetMarket.Controllers
{
    public class AccountController : Controller
    {
        private UserRepository _userRepository;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserRepository userRepository, ILogger<AccountController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Authorization()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorization(EnterViewModel loginViewModel)
        {
            _logger.LogInformation(loginViewModel.Login);
            _logger.LogInformation(loginViewModel.Password);
            User user = _userRepository.CheckData(loginViewModel.Login, Encryption.Encryption.GetHash(loginViewModel.Password));
            if (user == null)
            {
                ModelState.AddModelError("", "Неправильный логин или пароль!");
            }

            if (ModelState.IsValid)
            {
                await Authenticate(user);
                return RedirectToAction("Phone", "Market");
            }

            return View(loginViewModel);
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(RegisterViewModel registerViewModel)
        {
            if (!_userRepository.IsUniqueLogin(registerViewModel.Login))
            {
                ModelState.AddModelError("", "Логин или Email уже занят!");
            }

            if (ModelState.IsValid)
            {
                _userRepository.AddUser(registerViewModel.Login, registerViewModel.Email, Encryption.Encryption.GetHash(registerViewModel.Password), registerViewModel.Name,
                    registerViewModel.Surname, registerViewModel.MiddleName, registerViewModel.PhoneNumber, 2);
                return RedirectToAction("Phone", "Market");
            }

            return View(registerViewModel);
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.MaxValue,
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                IssuedUtc = DateTimeOffset.Now,
                // The time at which the authentication ticket was issued.

                RedirectUri = "http://localhost:54946/"
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), authProperties);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Phone", "Market");
        }
    }
}