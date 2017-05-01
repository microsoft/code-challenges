using System;

namespace Prepare
{
    internal class OrderRecord
    {
        public int CustomerID { get; set; }
        public Guid OrderID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int ShipVia { get; set; }
        public decimal Freight { get; set; }
    }

    internal class OrderDetailRecord
    {
        public int CustomerID { get; set; }
        public Guid OrderID { get; set; }
        public int ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}
