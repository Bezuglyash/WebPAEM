using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetMarket.Repository;
using NetMarket.ViewModels;

namespace NetMarket.Controllers
{
    public class StaffController : Controller
    {
        private UserRepository _userRepository;
        private ProductRepository _productRepository;
        IWebHostEnvironment _appEnvironment;

        public StaffController(UserRepository userRepository, ProductRepository productRepository, IWebHostEnvironment appEnvironment)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _appEnvironment = appEnvironment;
        }

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

        [Authorize(Roles = "admin, employee")]
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetProduct(id);
            return View(new ProductViewModel
            {
                Name = product.Name,
                Company = product.Company,
                Color = product.Color,
                Price = product.Price,
                OperationSystem = product.OperationSystem,
                StorageCard = product.StorageCard,
                Weight = product.Weight,
                Description = product.Description,
                ImageString = product.ImageString
            });
        }

        [HttpPost]
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

        [Authorize(Roles = "admin, employee")]
        public string Delete(int id)
        {
            return "View()";
        }
    }
}