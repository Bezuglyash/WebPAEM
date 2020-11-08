﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetMarket.Models;
using NetMarket.Repository;
using NetMarket.ViewModels;
using Newtonsoft.Json.Linq;

namespace NetMarket.Controllers
{
    public class MarketController : Controller
    {
        private UserRepository _userRepository;
        private ProductRepository _productRepository;
        private ProductInBasketRepository _productInBasketRepository;
        private readonly ILogger<MarketController> _logger;

        public MarketController(UserRepository userRepository, ProductRepository productRepository, ProductInBasketRepository productInBasketRepository, ILogger<MarketController> logger)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _productInBasketRepository = productInBasketRepository;
            _logger = logger;
            //_httpContextAccessor = httpContextAccessor;
            if (_productRepository.GetProducts().Count == 0)
            {
                _productRepository.AddProduct("Apple", "Apple iPhone 11", 73990, 256, "Белый", "iOS", 194,
                    "Дорогой, но топовый телефон!", true, "iPhoneWhite11.png");

                _productRepository.AddProduct("Apple", "Apple iPhone 11", 73990, 256, "Жёлтый", "iOS", 194,
                    "Дорогой, но топовый телефон!", true, "iPhoneYellow11.png");

                _productRepository.AddProduct("Apple", "Apple iPhone 7", 26990, 32, "Золотистый", "iOS", 138,
                    "Староват, но цена поражает!", true, "iPhoneGold7.png");

                _productRepository.AddProduct("Samsung", "Samsung Galaxy Z Fold2", 179990, 256, "Чёрный", "Android", 282,
                    "Очень дорогой, но, удвительно, не iPhone!", true, "GalaxyZFold2Black.png");

                _productRepository.AddProduct("Samsung", "Samsung Galaxy M21", 15990, 64, "Синий", "Android", 188,
                    "Недорогой, хороший, ещё и Samsung - отличный вариант для студента!", true, "GalaxyM21Blue.png");

                _productRepository.AddProduct("Samsung", "Samsung Galaxy M21", 15990, 64, "Чёрный", "Android", 188,
                    "Недорогой, хороший, ещё и Samsung - отличный вариант для студента!", true, "GalaxyM21Black.png");

                _productRepository.AddProduct("Honor", "Honor 10 Lite", 12990, 64, "Синий", "Android", 162,
                    "Бюждетный и хороший вариант! Сами таким пользуемся)", true, "Honor10LiteBlue.png");

                _productRepository.AddProduct("Honor", "Honor 10 Lite", 12990, 64, "Чёрный", "Android", 162,
                    "Бюждетный и хороший вариант!", true, "Honor10LiteBlack.png");

                _productRepository.AddProduct("Honor", "Honor 30 Pro Plus", 54990, 256, "Зелёный", "Android", 190,
                    "Такой дорогой Honor?", true, "Honor30ProPlusGreen.png");
            }
        }

        [HttpGet]
        public IActionResult Phone()
        {
            if (HttpContext.User.Identity.Name == null)
            {
                if (!HttpContext.Request.Cookies.ContainsKey("NotAuthorizedUser"))
                {
                    var options = new CookieOptions
                    {
                        MaxAge = TimeSpan.MaxValue
                    };
                    HttpContext.Response.Cookies.Append("NotAuthorizedUser", Guid.NewGuid().ToString(), options);
                }
            }
            return View(_productRepository.GetProducts());
        }

        [HttpGet]
        public int GetCountPhonesInBasket()
        {
            if (HttpContext.User.Identity.Name == null)
            {
                Guid userId = new Guid(HttpContext.Request.Cookies["NotAuthorizedUser"]);
                return _productInBasketRepository.GetCountProductsInBasketForNotAuthorizedUser(userId);
            }

            return _productInBasketRepository.GetCountProductsInBasketForAuthorizedUser(HttpContext.User.Identity.Name);
        }

        [HttpPost]
        public JsonResult GetMoreDetailed(int productId)
        {
            var product = _productRepository.GetProduct(productId);
            var productViewModel = new ProductViewModel
            {
                Id = productId,
                Company = product.Company,
                Name = product.Name,
                Price = product.Price,
                StorageCard = product.StorageCard,
                Color = product.Color,
                OperationSystem = product.OperationSystem,
                Weight = product.Weight,
                Description = product.Description,
                HaveInStock = product.HaveInStock,
                ImageString = product.ImageString
            };
            return Json(productViewModel);
        }

        [HttpGet]
        public IActionResult Cart()
        {
            if (HttpContext.User.Identity.Name == null)
            {
                Guid userId = new Guid(HttpContext.Request.Cookies["NotAuthorizedUser"]);
                return View(_productInBasketRepository.GetProductsInCartForNotAuthorizedUser(userId));
            }

            return View(_productInBasketRepository.GetProductsInCartForAuthorizedUser(HttpContext.User.Identity.Name));
        }

        [HttpGet]
        public IActionResult OrderRegistration()
        {
            if (HttpContext.User.Identity.Name == null)
            {
                Guid userId = new Guid(HttpContext.Request.Cookies["NotAuthorizedUser"]);
                return View(_productInBasketRepository.GetDataProductsInCartForNotAuthorizedUser(userId));
            }

            return View(_productInBasketRepository.GetDataProductsInCartForAuthorizedUser(HttpContext.User.Identity.Name));
        }

        [HttpGet]
        public JsonResult GetUserData()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var user = _userRepository.GetUser(HttpContext.User.Identity.Name);
                var userViewModel = new UserViewModel
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    MiddleName = user.MiddleName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
                return Json(userViewModel);
            }
            return null;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Users()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Warehouse()
        {
            return View();
        }

        [HttpPost]
        public async Task AddToBasket(int productId)
        {
            _logger.LogInformation(productId.ToString());
            if (HttpContext.User.Identity.Name == null)
            {
                Guid userId = new Guid(HttpContext.Request.Cookies["NotAuthorizedUser"]);
                await _productInBasketRepository.AddProductInBasketForNotAuthorizedUserAsync(userId, productId);
            }
            else
            {
                await _productInBasketRepository.AddProductInBasketForAuthorizedUserAsync(_userRepository.GetUsrId(HttpContext.User.Identity.Name), productId);
            }
        }

        [HttpPost]
        public async Task DeleteFromBasket(int idInBasket)
        {
            _logger.LogInformation(idInBasket.ToString());
            await _productInBasketRepository.DeleteProductFromCartAsync(idInBasket);
        }

        [HttpPost]
        public int Add(int productId)
        {
            return productId;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void AddCookies(string name)
        {
            //await HttpContext.Response.WriteAsync($"Hello {name}!");
            _logger.LogInformation(name);
        }
    }
}
