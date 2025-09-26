using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

[Authorize(Roles = "Manager")]
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly LogiTrackContext _context;
    private readonly IMemoryCache _cache;

    public InventoryController(LogiTrackContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAllItems()
    {
        const string cacheKey = "inventoryList";

        if (_cache.TryGetValue(cacheKey, out List<InventoryItem> cachedInventory))
            return Ok(cachedInventory);

        var inventory = await _context.InventoryItems
            .AsNoTracking()
            .ToListAsync();

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        _cache.Set(cacheKey, inventory, cacheOptions);

        return Ok(inventory);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> AddItem([FromBody] InventoryItem item)
    {
        if (item.ItemId < InventoryItem.MinItemId || item.ItemId > InventoryItem.MaxItemId)
            return BadRequest($"ItemId must be in range of {InventoryItem.MinItemId}-{InventoryItem.MaxItemId}.");

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();
        _cache.Remove("inventoryList");
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
        _cache.Remove("inventoryList");
        return NoContent();
    }
}