namespace KampusKayipEsya.Web.Models;

public class Location
{
    public int Id { get; set; }
    public string Building { get; set; } = string.Empty;
    public string? FloorOrRoom { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
}
