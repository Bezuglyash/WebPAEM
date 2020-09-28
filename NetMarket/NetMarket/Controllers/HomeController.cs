using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetMarket.Models;
using NetMarket.Repository;
using NetMarket.ViewModels;

namespace NetMarket.Controllers
{
    public class HomeController : Controller
    {
        private ProductRepository _productRepository;
        private readonly ILogger<HomeController> _logger;
        private ProductViewModel _productViewModel;

        public HomeController(ProductRepository productRepository, ILogger<HomeController> logger, ProductViewModel productViewModel)
        {
            _productRepository = productRepository;
            _logger = logger;
            _productViewModel = productViewModel;
            /*_productRepository.AddProduct("Apple", "Apple iPhone 11", 73990, 256, "Белый", "iOS", 194, 
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
                "Такой дорогой Honor?", true, "Honor30ProPlusGreen.png");*/
        }

        [HttpGet]
        public IActionResult Phone()
        {
            return View(_productRepository.GetProducts());
        }

        public IActionResult Cart()
        {
            return View();
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

        public void AddToBasket()
        {
            _productViewModel.CountProductsInBasket++;
            _logger.LogInformation(_productViewModel.CountProductsInBasket.ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
