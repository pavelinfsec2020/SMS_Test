using System.Text.Json.Serialization;

namespace HttpClientLib.Models
{
    public class MenuItem
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("Article")]
        public string Article { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Price")]
        public decimal Price { get; set; }

        [JsonPropertyName("IsWeighted")]
        public bool IsWeighted { get; set; }

        [JsonPropertyName("FullPath")]
        public string FullPath { get; set; } = string.Empty;

        [JsonPropertyName("Barcodes")]
        public List<string> Barcodes { get; set; } = new();
    }
}
