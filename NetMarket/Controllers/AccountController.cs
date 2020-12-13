using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetMarket.Models;
using NetMarket.Repository;
using NetMarket.ViewModels;

namespace NetMarket.Controllers
{
    /// <summary>
    /// Контроллер, который осуществляет работу с аккаунтами пользователей
    /// </summary>
    public class AccountController : Controller
    {
        private PeopleRepository _peopleRepository;
        private readonly ILogger<AccountController> _logger;

        public AccountController(PeopleRepository peopleRepository, ILogger<AccountController> logger)
        {
            _peopleRepository = peopleRepository;
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
            People human = _peopleRepository.CheckData(loginViewModel.Login, Encryption.Encryption.GetHash(loginViewModel.Password));
            if (human == null)
            {
                ModelState.AddModelError("", "Неправильный логин или пароль!");
                _peopleRepository.ClearCache();
            }

            if (ModelState.IsValid)
            {
                await Authenticate(human);
                if (human.RoleId == 3)
                {
                    return RedirectToAction("Phone", "Market");
                }
                return RedirectToAction("Warehouse", "Staff");
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
            if (!_peopleRepository.IsUniqueLogin(registerViewModel.Login, registerViewModel.Email))
            {
                ModelState.AddModelError("", "Логин или Email уже занят!");
            }

            if (ModelState.IsValid)
            {
                _peopleRepository.AddHuman(registerViewModel.Login, registerViewModel.Email, Encryption.Encryption.GetHash(registerViewModel.Password), registerViewModel.Name,
                    registerViewModel.Surname, registerViewModel.MiddleName, registerViewModel.PhoneNumber, 3);
                return RedirectToAction("Phone", "Market");
            }

            return View(registerViewModel);
        }

        /// <summary>
        /// Метод, который сохраняет в куки-файл данные авторизированного пользователя
        /// </summary>
        /// <param name="human">Авторизированный пользователь</param>
        /// <returns></returns>
        private async Task Authenticate(People human)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, human.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, human.Role.Name)
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

        /// <summary>
        /// Метод, который сохраняет в куки-файл данные авторизированного пользователя (вызывается при обновлении логина пользователя)
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="role">Права доступа</param>
        /// <returns></returns>
        private async Task Authenticate(string login, string role)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
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

        [HttpGet]
        public IActionResult Settings()
        {
            var user = _peopleRepository.GetUser(HttpContext.User.Identity.Name);
            return View(new UserSettingsViewModel
            {
                Login = user.Login,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                MiddleName = user.MiddleName,
                PhoneNumber = user.PhoneNumber
            });
        }

        /// <summary>
        /// Метод принимает запрос на изменение настроек пользователя
        /// </summary>
        /// <param name="type">Тип данных, которые нужно обновить (login, password, email, name, surname, middleName, phoneNumber) </param>
        /// <param name="data">Оновленные данные</param>
        /// <param name="additionalData">Дополнительные данные (при изменении пароля нужен текущий для проверки)</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> RewriteUserSettings(string type, string data, string additionalData)
        {
            string response = await _peopleRepository.UpdateAsync(HttpContext.User.Identity.Name, type, data, additionalData);
            if (type == "login" && response == "good")
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await Authenticate(data, HttpContext.User.IsInRole("user") ? "user" : HttpContext.User.IsInRole("admin") ? "admin" : "employee");
            }
            return response;
        }

        public async Task<IActionResult> Logout()
        {
            _peopleRepository.ClearCache();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Phone", "Market");
        }
    }
}