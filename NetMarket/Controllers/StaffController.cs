using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetMarket.Models;
using NetMarket.Repository;
using NetMarket.ViewModels;
using NetMarket.ViewModels.Employee;

namespace NetMarket.Controllers
{   
    /// <summary>
    /// Контроллер, который осуществляет работу интернет-магазина со стороны персонала магазина
    /// </summary>
    public class StaffController : Controller
    {
        private PeopleRepository _peopleRepository;
        private ProductRepository _productRepository;
        private OrderRepository _orderRepository;
        private ProductInBasketRepository _productInBasketRepository;
        IWebHostEnvironment _appEnvironment;

        public StaffController(PeopleRepository peopleRepository, ProductRepository productRepository, OrderRepository orderRepository, ProductInBasketRepository productInBasketRepository, IWebHostEnvironment appEnvironment)
        {
            _peopleRepository = peopleRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _productInBasketRepository = productInBasketRepository;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Employees()
        {
            return View(_peopleRepository.GetEmployees());
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Employees(string? textSearch)
        {
            if (textSearch != null)
            {
                return View(_peopleRepository.GetEmployees(textSearch));
            }
            return RedirectToAction("Employees");
        }

        /// <summary>
        /// Метод запроса на обновление прав доступа персонала
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <param name="rewriteRole">Новые права</param>
        /// <returns>Статусный код</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateRole(Guid id, string rewriteRole)
        {
            await _peopleRepository.EmployeeRoleUpdateAsync(id, rewriteRole == "Администратор" ? 1 : 2);
            return StatusCode(200);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult NewEmployee()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult NewEmployee(RegisterEmployeeViewModel registerEmployeeViewModel)
        {
            if (!_peopleRepository.IsUniqueLogin(registerEmployeeViewModel.Login, registerEmployeeViewModel.Email))
            {
                ModelState.AddModelError("", "Логин или Email уже занят!");
            }

            if (ModelState.IsValid)
            {
                _peopleRepository.AddHuman(registerEmployeeViewModel.Login, registerEmployeeViewModel.Email, Encryption.Encryption.GetHash(registerEmployeeViewModel.Password), registerEmployeeViewModel.Name,
                    registerEmployeeViewModel.Surname, registerEmployeeViewModel.MiddleName, registerEmployeeViewModel.PhoneNumber, registerEmployeeViewModel.Role);
                return RedirectToAction("Employees", "Staff");
            }

            return View(registerEmployeeViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "employee")]
        public IActionResult Orders()
        {
            return View(_orderRepository.GetAllOrders());
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult Orders(string search)
        {
            if (search != null)
            {
                return View(_orderRepository.GetSearchOrders(search));
            }
            return RedirectToAction("Orders");
        }

        /// <summary>
        /// Метод запроса на обновление статуса заказа
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <param name="rewriteStatus">Новый статус</param>
        /// <returns>Статусный код</returns>
        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> UpdateStatus(int id, int rewriteStatus)
        {
            await _orderRepository.OrderStatusUpdateAsync(id, rewriteStatus);
            return StatusCode(200);
        }

        /// <summary>
        /// Метод запроса о статусе заказа
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <returns>Статус заказа</returns>
        [HttpPost]
        [Authorize(Roles = "employee")]
        public string GetOrderStatus(int id)
        {
            return _orderRepository.GetOrderStatus(id);
        }

        [HttpGet]
        [Authorize(Roles = "admin, employee")]
        public IActionResult Warehouse()
        {
            var products = _productRepository.GetProducts();
            var productsViewModel = new List<ProductViewModel>();
            foreach (var product in products)
            {
                productsViewModel.Add(new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Color = product.Color
                });
            }
            return View(productsViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> Warehouse(string? textSearch, int? idWhichProductMustDelete)
        {
            if (idWhichProductMustDelete == null)
            {
                List<Product> products;
                var productsViewModel = new List<ProductViewModel>();
                if (textSearch != null)
                {
                    products = _productRepository.GetSearchProducts(textSearch);
                }
                else
                {
                    products = _productRepository.GetProducts();
                }
                foreach (var product in products)
                {
                    productsViewModel.Add(new ProductViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Color = product.Color
                    });
                }
                return View(productsViewModel);
            }
            await _productRepository.DeleteProductAsync((int)idWhichProductMustDelete);
            return RedirectToAction("Warehouse");
        }

        [Authorize(Roles = "admin, employee")]
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetProduct(id);
            return View(new ProductViewModel
            {
                Id = id,
                Name = product.Name,
                Company = product.Company,
                Color = product.Color,
                Price = product.Price,
                OperationSystem = product.OperationSystem,
                StorageCard = product.StorageCard,
                Weight = product.Weight,
                Description = product.Description,
                ImageString = product.ImageString,
                HaveInStock = product.HaveInStock
            });
        }

        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> Edit(IFormFile uploadImage)
        {
            var addressBar = HttpContext.Request.Path.ToString();
            var id = Convert.ToInt32(addressBar.Split(new [] { '/' })[3]);
            var pastImageString = await _productRepository.ImageStringUpdateAsync(id, uploadImage.FileName);
            if (uploadImage != null)
            {
                // путь к папке Files
                string path = "/" + uploadImage.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadImage.CopyToAsync(fileStream);
                }
            }
            if (System.IO.File.Exists($"wwwroot/{pastImageString}"))
            {
                System.IO.File.Delete($"wwwroot/{pastImageString}");
            }
            return RedirectToAction("Edit", id);
        }

        /// <summary>
        /// Метод запроса на обновление данных о товаре
        /// </summary>
        /// <param name="phoneId">ID товара</param>
        /// <param name="typeOfData">Тип данных, в которых происходят изменения (company, name, price, storageCard, color, operationSystem, weight, description, existence)</param>
        /// <param name="newData">Обновленные данные</param>
        /// <returns>Статусный код</returns>
        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> EditData(int phoneId, string typeOfData, string newData)
        {
            await _productRepository.UpdateAsync(phoneId, typeOfData, newData);
            if (typeOfData == "existence")
            {
                await _productInBasketRepository.DeleteAllProductsThatAreOutOfStockAsync(phoneId);
                _orderRepository.DeleteAllProductsThatAreOutOfStock(phoneId);
            }
            return StatusCode(200);
        }

        [HttpGet]
        [Authorize(Roles = "admin, employee")]
        public IActionResult NewProduct()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> NewProduct(NewProductViewModel newProductViewModel)
        {
            if (ModelState.IsValid)
            {
                string path = "/" + newProductViewModel.Image.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await newProductViewModel.Image.CopyToAsync(fileStream);
                }

                await _productRepository.AddProductAsync(newProductViewModel.Company, newProductViewModel.Name,
                    newProductViewModel.Price, newProductViewModel.StorageCard, newProductViewModel.Color,
                    newProductViewModel.OperationSystem, newProductViewModel.Weight, newProductViewModel.Description,
                    newProductViewModel.Existence, newProductViewModel.Image.FileName);
                return RedirectToAction("Warehouse");
            }
            return View(newProductViewModel);
        }
    }
}