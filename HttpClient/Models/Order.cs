using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HttpClient.Models
{
    public class Order
    {
        [JsonPropertyName("OrderId")]
        public string OrderId { get; set; } = string.Empty;

        [JsonPropertyName("MenuItems")]
        public List<OrderItem> MenuItems { get; set; } = new();
    }

    public class OrderItem
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("Quantity")]
        public string Quantity { get; set; } = string.Empty;
    }
}
