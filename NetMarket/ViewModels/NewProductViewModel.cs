using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NetMarket.ViewModels
{
    public class NewProductViewModel
    {
        [Required]
        public string Company { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int StorageCard { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string OperationSystem { get; set; }

        [Required]
        public int Weight { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Existence { get; set; }

        [Required(ErrorMessage = "Вы не выбрали фотографию для этого товара!")]
        public IFormFile Image { get; set; }
    }
}