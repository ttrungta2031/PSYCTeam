using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public int? ItemId { get; set; }
        public int? OrderId { get; set; }

        public virtual Product Item { get; set; }
        public virtual Order Order { get; set; }
    }
}
