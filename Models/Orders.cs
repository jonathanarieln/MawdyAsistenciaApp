namespace MawdyAsistenciaApp.Models
{
    public class Order
    {
        public string OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public string EmailID { get; set; }
        public string Geography { get; set; }
        public string Category { get; set; }
        public string ProductName { get; set; }
        public decimal Sales { get; set; }
        public int Quantity { get; set; }
        public decimal Profit { get; set; }
    }
 }
    
