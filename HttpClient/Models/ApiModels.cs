using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HttpClient.Models
{
    public class GetMenuRequest
    {
        [JsonPropertyName("Command")]
        public string Command { get; set; } = "GetMenu";

        [JsonPropertyName("CommandParameters")]
        public GetMenuParameters CommandParameters { get; set; } = new();
    }

    public class GetMenuParameters
    {
        [JsonPropertyName("WithPrice")]
        public bool WithPrice { get; set; } = true;
    }

    public class GetMenuResponse
    {
        [JsonPropertyName("Command")]
        public string Command { get; set; } = string.Empty;

        [JsonPropertyName("Success")]
        public bool Success { get; set; }

        [JsonPropertyName("ErrorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;

        [JsonPropertyName("Data")]
        public MenuData Data { get; set; } = new();
    }

    public class MenuData
    {
        [JsonPropertyName("MenuItems")]
        public List<MenuItem> MenuItems { get; set; } = new();
    }

    public class SendOrderRequest
    {
        [JsonPropertyName("Command")]
        public string Command { get; set; } = "SendOrder";

        [JsonPropertyName("CommandParameters")]
        public Order CommandParameters { get; set; } = new();
    }

    public class SendOrderResponse
    {
        [JsonPropertyName("Command")]
        public string Command { get; set; } = string.Empty;

        [JsonPropertyName("Success")]
        public bool Success { get; set; }

        [JsonPropertyName("ErrorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
