using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
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
                return RedirectToAction("Phone", "Home");
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
                return RedirectToAction("Phone", "Home");
            }

            return View(registerViewModel);
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.RoleId == 1 ? "admin" : "user")
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Phone", "Home");
        }
    }
}