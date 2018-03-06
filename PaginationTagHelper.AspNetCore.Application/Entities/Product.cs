using System;

namespace PaginationTagHelper.AspNetCore.Application.Entities
{
    public class Product
    {
        public int Id { get; set; }
  
        public string Name { get; set; }

        public double Price { get; set; }

        public string SKU { get; set; }
    }
}
