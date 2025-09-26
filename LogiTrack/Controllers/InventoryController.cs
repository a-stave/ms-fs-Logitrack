using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly LogiTrackContext _context;

    public InventoryController(LogiTrackContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAllItems()
    {
        return Ok(await _context.InventoryItems.ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> AddItem([FromBody] InventoryItem item)
    {
        if (item.ItemId < InventoryItem.MinItemId || item.ItemId > InventoryItem.MaxItemId)
            return BadRequest($"ItemId must be in range of {InventoryItem.MinItemId}-{InventoryItem.MaxItemId}.");

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAllItems), new { id = item.ItemId }, item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        if (id < InventoryItem.MinItemId || id > InventoryItem.MaxItemId)
            return BadRequest($"ItemId must be in range of {InventoryItem.MinItemId}-{InventoryItem.MaxItemId}.");

        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null)
            return NotFound($"Item with ID {id} not found.");

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}