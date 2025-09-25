using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTrack
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey("InventoryItem")]
        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public int Quantity { get; set; }
    }
}
