using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LogiTrack
{
    public class InventoryItem
    {
        // Global constants
        public const int MinItemId = 1;
        public const int MaxItemId = 99999999;
        public const int MinQuantity = 1;
        public const int MaxQuantity = 99999;
        public const int MaxNameLength = 50;
        public const int MaxLocationLength = 50;

        // Data members
        [Key]
        [Range(MinItemId, MaxItemId)]
        public int ItemId { get; set; } = 0;

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(MinQuantity, MaxQuantity)]
        public int Quantity { get; set; } = 0;

        [Required]
        [MaxLength(MaxLocationLength)]
        public string Location { get; set; } = string.Empty;

        // Constructors
        public InventoryItem() { }

        public InventoryItem(int id, string name, int quantity, string location)
        {
            var regex = new Regex(@"^[a-zA-Z0-9\s!@#$%^&*]+$");     // Sanitize inputs

            if (MinItemId >= id || MaxItemId < id)
                throw new ArgumentOutOfRangeException(nameof(id), $"ItemId must be in range of {MinItemId}-{MaxItemId}.");

            if (MinQuantity >= quantity || MaxQuantity < quantity)
                throw new ArgumentOutOfRangeException(nameof(quantity), $"Quantity must be in range of {MinQuantity}-{MaxQuantity}.");

            if (string.IsNullOrWhiteSpace(name) || MaxNameLength < name.Length || !regex.IsMatch(name))
                throw new ArgumentException($"Name is invalid or exceeds {MaxNameLength} characters.", nameof(name));

            if (string.IsNullOrWhiteSpace(location) || MaxLocationLength < location.Length || !regex.IsMatch(location))
                throw new ArgumentException($"Location is invalid or exceeds {MaxLocationLength} characters.", nameof(location));

            ItemId = id;
            Name = name;
            Quantity = quantity;
            Location = location;
        }

        public override string ToString() => $"Item: {Name}|Quantity: {Quantity}|Location: {Location}";

        public void DisplayInfo() => Console.WriteLine(ToString());
    }
}