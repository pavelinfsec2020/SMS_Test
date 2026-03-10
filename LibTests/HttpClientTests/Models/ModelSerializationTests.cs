using System.Text.Json;
using HttpClientLib.Models;

namespace LibTests.HttpClientTests.Models
{
    public class ModelSerializationTests
    {
        [Fact]
        public void MenuItem_SerializesAndDeserializes_Correctly()
        {
            var original = new MenuItem
            {
                Id = "5979224",
                Article = "A1004292",
                Name = "Каша гречневая",
                Price = 50,
                IsWeighted = false,
                FullPath = "ПРОИЗВОДСТВО\\Гарниры",
                Barcodes = new List<string> { "57890975627974236429" }
            };

            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<MenuItem>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(original.Id, deserialized.Id);
            Assert.Equal(original.Article, deserialized.Article);
            Assert.Equal(original.Name, deserialized.Name);
            Assert.Equal(original.Price, deserialized.Price);
            Assert.Equal(original.IsWeighted, deserialized.IsWeighted);
            Assert.Equal(original.FullPath, deserialized.FullPath);
            Assert.Equal(original.Barcodes, deserialized.Barcodes);
        }

        [Fact]
        public void Order_SerializesAndDeserializes_Correctly()
        {
            var original = new Order
            {
                OrderId = "62137983-1117-4D10-87C1-EF40A4348250",
                MenuItems = new List<OrderItem>
            {
                new() { Id = "5979224", Quantity = "1" },
                new() { Id = "9084246", Quantity = "0.408" }
            }
            };

            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<Order>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(original.OrderId, deserialized.OrderId);
            Assert.Equal(original.MenuItems.Count, deserialized.MenuItems.Count);
            Assert.Equal(original.MenuItems[0].Id, deserialized.MenuItems[0].Id);
            Assert.Equal(original.MenuItems[0].Quantity, deserialized.MenuItems[0].Quantity);
        }

        [Fact]
        public void GetMenuResponse_Deserializes_Correctly()
        {
            var json = @"{
            ""Command"": ""GetMenu"",
            ""Success"": true,
            ""ErrorMessage"": """",
            ""Data"": {
                ""MenuItems"": [
                    {
                        ""Id"": ""5979224"",
                        ""Article"": ""A1004292"",
                        ""Name"": ""Каша гречневая"",
                        ""Price"": 50,
                        ""IsWeighted"": false,
                        ""FullPath"": ""ПРОИЗВОДСТВО\\Гарниры"",
                        ""Barcodes"": [""57890975627974236429""]
                    }
                ]
            }
        }";

            var response = JsonSerializer.Deserialize<GetMenuResponse>(json);

            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal("GetMenu", response.Command);
            Assert.Empty(response.ErrorMessage);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data.MenuItems);
            Assert.Equal("5979224", response.Data.MenuItems[0].Id);
        }

        [Fact]
        public void SendOrderResponse_Deserializes_Correctly()
        {
            var json = @"{
            ""Command"": ""SendOrder"",
            ""Success"": true,
            ""ErrorMessage"": """"
        }";

            var response = JsonSerializer.Deserialize<SendOrderResponse>(json);

            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal("SendOrder", response.Command);
            Assert.Empty(response.ErrorMessage);
        }

        [Fact]
        public void GetMenuResponse_WithError_Deserializes_Correctly()
        {
            var json = @"{
            ""Command"": ""GetMenu"",
            ""Success"": false,
            ""ErrorMessage"": ""Server error"",
            ""Data"": null
        }";

            var response = JsonSerializer.Deserialize<GetMenuResponse>(json);

            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal("GetMenu", response.Command);
            Assert.Equal("Server error", response.ErrorMessage);
            Assert.Null(response.Data);
        }
    }
}
