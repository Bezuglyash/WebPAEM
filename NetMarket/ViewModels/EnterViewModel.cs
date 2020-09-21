using System.ComponentModel.DataAnnotations;

namespace NetMarket.ViewModels
{
    public class EnterViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}