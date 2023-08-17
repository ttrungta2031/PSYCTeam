using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int? Quantity { get; set; }
        public double? Total { get; set; }
        public int? ItemId { get; set; }
        public int? ShopId { get; set; }
        public int? PaymentId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? CreateDay { get; set; }
        public string Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
