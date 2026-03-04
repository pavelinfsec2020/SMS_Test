using ConsoleApp.Models;
using Serilog;

namespace ConsoleApp.Services
{
    public class OrderService
    {
        private readonly ILogger _logger;
        private readonly DatabaseService _dbService;
        private readonly OrderParser _orderParser;

        public OrderService(ILogger logger, DatabaseService dbService, OrderParser orderParser)
        {
            _logger = logger;
            _dbService = dbService;
            _orderParser = orderParser;
        }

        public async Task<Order> CreateOrderFromInputAsync()
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString()
            };

            var menuDict = await _dbService.GetMenuItemsDictAsync();
            bool inputValid = false;

            while (!inputValid)
            {
                _logger.Information(Constants.Messages.EnterOrder);
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    _logger.Warning(Constants.Messages.InputEmpty);
                    continue;
                }

                try
                {
                    var items = _orderParser.ParseOrderItems(input);

                    if (!_orderParser.ValidateOrderItems(items, menuDict))
                    {
                        _logger.Warning(Constants.Messages.CodeNotFound);
                        continue;
                    }

                    foreach (var item in items)
                    {
                        var menuItem = menuDict[item.Key];
                        order.MenuItems.Add(new OrderItem
                        {
                            Id = menuItem.Id,
                            Quantity = item.Value
                        });
                    }

                    _logger.Information(Constants.Messages.OrderCreated);
                    inputValid = true;
                }
                catch (Exception ex)
                {
                    _logger.Warning(Constants.Errors.ParseError, ex.Message);
                }
            }

            return order;
        }
    }
}
