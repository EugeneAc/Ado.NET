using System;
using System.Collections.Generic;
using System.Text;

namespace My_Task.Models
{
    public class OdrerInfo
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Product> Products { get; set; }
    }
}
