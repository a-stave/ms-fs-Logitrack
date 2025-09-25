using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LogiTrack
{
    public class Order
    {
        // Global constants
        public const int MinOrderId = 1;
        public const int MaxOrderId = 99999999;
        public const int MaxCustomerNameLength = 50;

        // Data members
        [Key]
        [Range(MinOrderId, MaxOrderId)]
        public int OrderId { get; set; } = 0;

        [Required]
        [MaxLength(MaxCustomerNameLength)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public DateTime DatePlaced { get; set; } = DateTime.UtcNow;

        [Required]
        public List<InventoryItem> Items { get; set; } = new List<InventoryItem>();

        // Constructors
        public Order() { }

        public Order(int id, string name, DateTime time, List<InventoryItem> items)
        {
            var regex = new Regex(@"^[a-zA-Z0-9\s!@#$%^&*]+$");     // Sanitize inputs

            if (MinOrderId >= id || MaxOrderId < id)
                throw new ArgumentOutOfRangeException(nameof(id), $"OrderId must be in range of {MinOrderId}-{MaxOrderId}. (Value: {id})");

            if (string.IsNullOrWhiteSpace(name) || MaxCustomerNameLength < name.Length || !regex.IsMatch(name))
                throw new ArgumentException($"Customer name is invalid or exceeds {MaxCustomerNameLength} characters.", nameof(name));

            if (time == default || time > DateTime.UtcNow)
                throw new ArgumentException($"DatePlaced must be a valid past or present UTC date. (Value: {time})", nameof(time));

            OrderId = id;
            CustomerName = name;
            DatePlaced = time;
            Items = new List<InventoryItem>(items);
        }

        public override string ToString() => $"Order #{OrderId} for {CustomerName}| Items: {Items.Count()}| Placed: {DatePlaced.ToShortDateString()}";

        public void AddItem(InventoryItem item)
        {
            if (null == item)
                throw new ArgumentNullException(nameof(item), "Cannot add a null item.");
            Items.Add(item);
        }

        public void RemoveItem(int itemId)
        {
            var target = Items.Find(item => item.ItemId == itemId);
            if (null != target)
                Items.Remove(target);
        }

        public void GetOrderSummary() => Console.WriteLine(ToString());
    }
}