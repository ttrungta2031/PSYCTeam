using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
            ProductTypes = new HashSet<ProductType>();
        }

        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDay { get; set; }
        public string Status { get; set; }
        public int? ShopId { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductType> ProductTypes { get; set; }
    }
}
