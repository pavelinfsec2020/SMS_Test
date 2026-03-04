using ClientMenuItem = HttpClientLib.Models.MenuItem;
using GrpcMenuItem = SmsTest.GrpcClient.MenuItem;
using ConsoleMenuItem = ConsoleApp.Models.MenuItem;
using ClientOrder = HttpClientLib.Models.Order;
using ClientOrderItem = HttpClientLib.Models.OrderItem;
using GrpcOrder = SmsTest.GrpcClient.Order;
using GrpcOrderItem = SmsTest.GrpcClient.OrderItem;
using ConsoleOrder = ConsoleApp.Models.Order;

namespace ConsoleApp.Mappers
{
    public static class MenuItemMapper
    {
        public static ConsoleMenuItem MapFromClient(ClientMenuItem item)
        {
            return new ConsoleMenuItem
            {
                Id = item.Id,
                Article = item.Article,
                Name = item.Name,
                Price = item.Price,
                IsWeighted = item.IsWeighted,
                FullPath = item.FullPath,
                Barcodes = item.Barcodes
            };
        }

        public static ConsoleMenuItem MapFromGrpc(GrpcMenuItem item)
        {
            return new ConsoleMenuItem
            {
                Id = item.Id,
                Article = item.Article,
                Name = item.Name,
                Price = (decimal)item.Price,
                IsWeighted = item.IsWeighted,
                FullPath = item.FullPath,
                Barcodes = item.Barcodes.ToList()
            };
        }

        public static ClientOrder MapToClient(ConsoleOrder order)
        {
            var clientOrder = new ClientOrder
            {
                OrderId = order.OrderId
            };

            foreach (var item in order.MenuItems)
            {
                clientOrder.MenuItems.Add(new ClientOrderItem
                {
                    Id = item.Id,
                    Quantity = item.Quantity.ToString("F3")
                });
            }

            return clientOrder;
        }

        public static GrpcOrder MapToGrpc(ConsoleOrder order)
        {
            var grpcOrder = new GrpcOrder
            {
                Id = order.OrderId
            };

            foreach (var item in order.MenuItems)
            {
                grpcOrder.OrderItems.Add(new GrpcOrderItem
                {
                    Id = item.Id,
                    Quantity = (double)item.Quantity
                });
            }

            return grpcOrder;
        }
    }
}
