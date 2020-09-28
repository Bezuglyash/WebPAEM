using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetMarket.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Company { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public int StorageCard { get; set; }

        public string Color { get; set; }

        public string OperationSystem { get; set; }

        public int Weight { get; set; }

        public string Description { get; set; }

        public bool HaveInStock { get; set; }

        public string ImageString { get; set; }
    }
}