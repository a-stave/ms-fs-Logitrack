namespace LogiTrack.DTOs
{
    public class CreateOrderDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime DatePlaced { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}