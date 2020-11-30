using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetMarket.Models;
using NetMarket.Repository;
using NetMarket.ViewModels;

namespace NetMarket.Controllers
{
    public class MarketController : Controller
    {
        private PeopleRepository _peopleRepository;
        private ProductRepository _productRepository;
        private ProductInBasketRepository _productInBasketRepository;
        private OrderRepository _orderRepository;
        private readonly ILogger<MarketController> _logger;

        public MarketController(PeopleRepository peopleRepository, ProductRepository productRepository, ProductInBasketRepository productInBasketRepository, OrderRepository orderRepository, ILogger<MarketController> logger)
        {
            _peopleRepository = peopleRepository;
            _productRepository = productRepository;
            _productInBasketRepository = productInBasketRepository;
            _orderRepository = orderRepository;
            _logger = logger;
            //_httpContextAccessor = httpContextAccessor;
            if (_productRepository.GetProducts().Count == 0)
            {
                Task.Run(async () => await _productRepository.AddProductAsync("Apple", "Apple iPhone 11", 73990, 256, "Белый", "iOS", 194,
                    "Дорогой, но топовый телефон!", "Есть в наличии", "iPhoneWhite11.png"));

                Task.Run(async () => await _productRepository.AddProductAsync("Apple", "Apple iPhone 11", 73990, 256, "Жёлтый", "iOS", 194,
                    "Дорогой, но топовый телефон!", "Есть в наличии", "iPhoneYellow11.png"));

                Task.Run(async () => await _productRepository.AddProductAsync("Apple", "Apple iPhone 7", 26990, 32, "Золотистый", "iOS", 138,
                    "Староват, но цена поражает!", "Есть в наличии", "iPhoneGold7.png"));

                Task.Run(async () => await _productRepository.AddProductAsync("Samsung", "Samsung Galaxy Z Fold2", 179990, 256, "Чёрный", "Android", 282,
                    "Очень дорогой, но, удвительно, не iPhone!", "Есть в наличии", "GalaxyZFold2Black.png"));

                Task.Run(async () => await _productRepository.AddProductAsync("Samsung", "Samsung Galaxy M21", 15990, 64, "Синий", "Android", 188,
                    "Недорогой, хороший, ещё и Samsung - отличный вариант для студента!", "Есть в наличии", "GalaxyM21Blue.png"));

                Task.Run(async () => await _productRepository.AddProductAsync("Samsung", "Samsung Galaxy M21", 15990, 64, "Чёрный", "Android", 188,
                    "Недорогой, хороший, ещё и Samsung - отличный вариант для студента!", "Есть в наличии", "GalaxyM21Black.png"));

                Task.Run(async () => await _productRepository.AddProductAsync("Honor", "Honor 10 Lite", 12990, 64, "Синий", "Android", 162,
                    "Бюждетный и хороший вариант! Сами таким пользуемся)", "Есть в наличии", "Honor10LiteBlue.png"));

                Task.Run(async () => await _productRepository.AddProductAsync("Honor", "Honor 10 Lite", 12990, 64, "Чёрный", "Android", 162,
                    "Бюждетный и хороший вариант!", "Есть в наличии", "Honor10LiteBlack.png"));

                Task.Run(async () => await _productRepository.AddProductAsync("Honor", "Honor 30 Pro Plus", 54990, 256, "Зелёный", "Android", 190,
                    "Такой дорогой Honor?", "Есть в наличии", "Honor30ProPlusGreen.png"));
            }
        }

        [HttpGet]
        public IActionResult Phone()
        {
            if (!HttpContext.User.IsInRole("admin"))
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
            else
            {
                return RedirectToAction("Warehouse", "Staff");
            }
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
            var controller = RouteData.Values["controller"].ToString();
            var action = RouteData.Values["action"].ToString();
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
                ViewBag.Sum = _productInBasketRepository.GetPriceSumProductsInCartForNotAuthorizedUser(userId);
                return View();
            }

            ViewBag.Sum = _productInBasketRepository.GetPriceSumProductsInCartForAuthorizedUser(HttpContext.User.Identity.Name);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OrderRegistration(OrderRegistrationViewModel orderRegistrationViewModel)
        {
            if (ModelState.IsValid)
            {
                List<int> productsId;
                int sum;
                if (HttpContext.User.Identity.Name == null)
                {
                    sum = _productInBasketRepository.GetPriceSumProductsInCartForNotAuthorizedUser(new Guid(HttpContext.Request.Cookies["NotAuthorizedUser"]));
                    productsId = await _productInBasketRepository.DeleteProductsInBasketForNotAuthorizedUserAsync(new Guid(HttpContext.Request.Cookies["NotAuthorizedUser"]));
                    await _orderRepository.AddNewOrderAsync(null,
                        DateTime.Now,
                        orderRegistrationViewModel.Name,
                        orderRegistrationViewModel.Surname,
                        orderRegistrationViewModel.MiddleName,
                        orderRegistrationViewModel.Email,
                        orderRegistrationViewModel.PhoneNumber,
                        orderRegistrationViewModel.Address,
                        orderRegistrationViewModel.Comment,
                        sum,
                        productsId);
                    return RedirectToAction("OrderRegistrationComplete", "Market");
                }
                sum = _productInBasketRepository.GetPriceSumProductsInCartForAuthorizedUser(HttpContext.User.Identity.Name);
                var userId = _peopleRepository.GetUserId(HttpContext.User.Identity.Name);
                productsId = await _productInBasketRepository.DeleteProductsInBasketForAuthorizedUserAsync(HttpContext.User.Identity.Name);
                await _orderRepository.AddNewOrderAsync(userId,
                    DateTime.Now,
                    orderRegistrationViewModel.Name,
                    orderRegistrationViewModel.Surname,
                    orderRegistrationViewModel.MiddleName,
                    orderRegistrationViewModel.Email,
                    orderRegistrationViewModel.PhoneNumber,
                    orderRegistrationViewModel.Address,
                    orderRegistrationViewModel.Comment,
                    sum,
                    productsId);
                return RedirectToAction("OrderRegistrationComplete", "Market");
            }

            ViewBag.Sum = HttpContext.User.Identity.Name != null
                ? _productInBasketRepository.GetPriceSumProductsInCartForAuthorizedUser(HttpContext.User.Identity.Name)
                : _productInBasketRepository.GetPriceSumProductsInCartForNotAuthorizedUser(new Guid(HttpContext.Request.Cookies["NotAuthorizedUser"]));

            return View(orderRegistrationViewModel);
        }

        [HttpGet]
        public JsonResult GetUserData()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var user = _peopleRepository.GetUser(HttpContext.User.Identity.Name);
                var userViewModel = new UserInOrderRegistrationViewModel
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
        public IActionResult OrderRegistrationComplete()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public IActionResult MyOrders()
        {
            return View(_orderRepository.GetAllUserOrders(HttpContext.User.Identity.Name));
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public JsonResult GetProductsInOrder(int orderNumber)
        {
            return Json(_orderRepository.GetProductsInOrder(orderNumber));
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
                await _productInBasketRepository.AddProductInBasketForAuthorizedUserAsync(HttpContext.User.Identity.Name, _peopleRepository.GetUserId(HttpContext.User.Identity.Name), productId);
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