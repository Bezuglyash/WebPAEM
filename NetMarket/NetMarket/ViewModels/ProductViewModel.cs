namespace NetMarket.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            CountProductsInBasket = 0;
        }

        public int CountProductsInBasket { get; set; }
    }
}