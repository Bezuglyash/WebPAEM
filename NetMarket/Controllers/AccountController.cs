using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public IActionResult Authorization(EnterViewModel loginViewModel)
        {
            _logger.LogInformation(loginViewModel.Login);
            _logger.LogInformation(loginViewModel.Password);
            if (loginViewModel.Login == "test")
            {
                ModelState.AddModelError("Login", "Логин не очень!");
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Index", "Home");
            }

            return View(registerViewModel);
        }
    }
}