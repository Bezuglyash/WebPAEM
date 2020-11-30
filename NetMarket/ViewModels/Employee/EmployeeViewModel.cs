using System;

namespace NetMarket.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int Role { get; set; }
    }
}