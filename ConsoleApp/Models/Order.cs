namespace ConsoleApp.Models
{

    public class OrderItem
    {
        public string Id { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }
    public class Order
    {
        public string OrderId { get; set; } = string.Empty;
        public List<OrderItem> MenuItems { get; set; } = new();
    }
}
