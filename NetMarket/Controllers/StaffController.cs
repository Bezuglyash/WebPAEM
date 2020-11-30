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

namespace NetMarket.Controllers
{
    public class StaffController : Controller
    {
        private PeopleRepository _peopleRepository;
        private ProductRepository _productRepository;
        IWebHostEnvironment _appEnvironment;

        public StaffController(PeopleRepository peopleRepository, ProductRepository productRepository, IWebHostEnvironment appEnvironment)
        {
            _peopleRepository = peopleRepository;
            _productRepository = productRepository;
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

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateRole(Guid id, string rewriteRole)
        {
            await _peopleRepository.EmployeeRoleUpdateAsync(id, rewriteRole == "Администратор" ? 1 : 3);
            return StatusCode(200);
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

        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> EditData(int phoneId, string typeOfData, string newData)
        {
            await _productRepository.UpdateAsync(phoneId, typeOfData, newData);
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