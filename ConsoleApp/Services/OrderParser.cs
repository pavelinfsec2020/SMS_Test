using ConsoleApp.Models;

namespace ConsoleApp.Services
{
    public class OrderParser
    {
        public Dictionary<string, decimal> ParseOrderItems(string input)
        {
            var result = new Dictionary<string, decimal>();
            var pairs = input.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length != 2)
                    throw new FormatException(string.Format(Constants.Errors.InvalidFormat, pair));

                var code = parts[0].Trim();
                if (!decimal.TryParse(parts[1].Trim(), out var quantity))
                    throw new FormatException(string.Format(Constants.Errors.InvalidQuantity, parts[1]));

                if (quantity <= 0)
                    throw new ArgumentException(string.Format(Constants.Errors.QuantityPositive, quantity));

                result[code] = quantity;
            }

            return result;
        }

        public bool ValidateOrderItems(Dictionary<string, decimal> items, Dictionary<string, MenuItem> menuDict)
        {
            foreach (var code in items.Keys)
            {
                if (!menuDict.ContainsKey(code))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
