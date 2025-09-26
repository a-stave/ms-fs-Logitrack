using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack;
using LogiTrack.DTOs;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly LogiTrackContext _context;

    public OrderController(LogiTrackContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.InventoryItem)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderDtos = orders.Select(order => new OrderDto
        {
            OrderId = order.OrderId,
            CustomerName = order.CustomerName,
            DatePlaced = order.DatePlaced,
            Items = order.Items.Select(oi => new OrderItemDto
            {
                InventoryItemId = oi.InventoryItemId,
                ItemName = oi.InventoryItem.Name,
                Quantity = oi.Quantity
            }).ToList()
        }).ToList();

        return Ok(orderDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        if (id < Order.MinOrderId || id > Order.MaxOrderId)
            return BadRequest($"OrderId must be in range of {Order.MinOrderId}-{Order.MaxOrderId}.");

        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.InventoryItem)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound($"Order with ID {id} not found.");

        var dto = new OrderDto
        {
            OrderId = order.OrderId,
            CustomerName = order.CustomerName,
            DatePlaced = order.DatePlaced,
            Items = order.Items.Select(oi => new OrderItemDto
            {
                InventoryItemId = oi.InventoryItemId,
                ItemName = oi.InventoryItem.Name,
                Quantity = oi.Quantity
            }).ToList()
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (dto.OrderId < Order.MinOrderId || dto.OrderId > Order.MaxOrderId)
            return BadRequest($"OrderId must be in range of {Order.MinOrderId}-{Order.MaxOrderId}.");

        foreach (var item in dto.Items)
        {
            var exists = await _context.InventoryItems.AnyAsync(i => i.ItemId == item.InventoryItemId);
            if (!exists)
                return BadRequest($"Inventory item with ID {item.InventoryItemId} does not exist.");
        }

        var order = new Order
        {
            OrderId = dto.OrderId,
            CustomerName = dto.CustomerName,
            DatePlaced = dto.DatePlaced,
            Items = dto.Items.Select(i => new OrderItem
            {
                InventoryItemId = i.InventoryItemId,
                Quantity = i.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Fetch the saved order with navigation properties
        var savedOrder = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.InventoryItem)
            .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

        var dtoResponse = new OrderDto
        {
            OrderId = savedOrder.OrderId,
            CustomerName = savedOrder.CustomerName,
            DatePlaced = savedOrder.DatePlaced,
            Items = savedOrder.Items.Select(oi => new OrderItemDto
            {
                InventoryItemId = oi.InventoryItemId,
                ItemName = oi.InventoryItem.Name,
                Quantity = oi.Quantity
            }).ToList()
        };

        return CreatedAtAction(nameof(GetOrderById), new { id = dtoResponse.OrderId }, dtoResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        if (id < Order.MinOrderId || id > Order.MaxOrderId)
            return BadRequest($"OrderId must be in range of {Order.MinOrderId}-{Order.MaxOrderId}.");

        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound($"Order with ID {id} not found.");

        _context.OrderItems.RemoveRange(order.Items);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}