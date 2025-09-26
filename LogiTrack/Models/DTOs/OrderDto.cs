namespace LogiTrack.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime DatePlaced { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}