namespace NetMarket.ViewModels.MyOrders
{
    public class OrderViewModel
    {
        public int OrderNumber { get; set; }

        public string CustomerFullName { get; set; }

        public string Address { get; set; }

        public string OrderDate { get; set; }

        public int Sum { get; set; }

        public string Status { get; set; }

        public string Comment { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string InformationForEmployee { get; set; }
    }
}