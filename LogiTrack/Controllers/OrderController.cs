using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack;

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
    public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        if (id < Order.MinOrderId || id > Order.MaxOrderId)
            return BadRequest($"OrderId must be in range of {Order.MinOrderId}-{Order.MaxOrderId}.");

        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (null == order)
            return NotFound($"Order with ID {id} not found.");

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
    {
        if (order.OrderId < Order.MinOrderId || order.OrderId > Order.MaxOrderId)
            return BadRequest($"OrderId must be in range of {Order.MinOrderId}-{Order.MaxOrderId}.");

        foreach (var item in order.Items)
        {
            var existingItem = await _context.InventoryItems.FindAsync(item.ItemId);
            if (existingItem == null)
                return BadRequest($"Inventory item with ID {item.ItemId} does not exist.");
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        if (id < Order.MinOrderId || id > Order.MaxOrderId)
            return BadRequest($"OrderId must be in range of {Order.MinOrderId}-{Order.MaxOrderId}.");

        var order = await _context.Orders.FindAsync(id);
        if (null == order)
            return NotFound($"Order with ID {id} not found.");

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}