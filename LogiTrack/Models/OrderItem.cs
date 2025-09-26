using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTrack
{
    public class OrderItem
    {
        // Global constants
        public const int MinOrderId = 1;
        public const int MaxOrderId = 99999999;
        public const int MinItemId = 1;
        public const int MaxItemId = 99999999;
        public const int MinQuantity = 1;
        public const int MaxQuantity = 99999;


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; } = 0;

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("InventoryItem")]
        [Range(MinItemId, MaxItemId)]
        public int InventoryItemId { get; set; } = 0;

        [Required]
        [Range(MinQuantity, MaxQuantity)]
        public int Quantity { get; set; } = 0;

        // Navigation properties
        public Order? Order { get; set; }
        public InventoryItem? InventoryItem { get; set; }

        // Constructors
        public OrderItem() { }

        public OrderItem(int orderId, int itemId, int quantity)
        {
            if (MinOrderId > orderId || MaxOrderId < orderId)
                throw new ArgumentOutOfRangeException(nameof(orderId), $"OrderId must be in range of {MinOrderId}-{MaxOrderId}. (Value: {orderId})");

            if (MinItemId > itemId || MaxItemId < itemId)
                throw new ArgumentOutOfRangeException(nameof(itemId), $"ItemId must be in range of {MinItemId}-{MaxItemId}. (Value: {itemId})");

            if (MinQuantity > quantity || MaxQuantity < quantity)
                throw new ArgumentOutOfRangeException(nameof(quantity), $"Quantity must be in range of {MinQuantity}-{MaxQuantity}. (Value: {quantity})");

            OrderId = orderId;
            InventoryItemId = itemId;
            Quantity = quantity;
        }
    }
}