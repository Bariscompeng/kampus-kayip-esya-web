namespace KampusKayipEsya.Web.Models;

public class ItemPhoto
{
    public int Id { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public string Path { get; set; } = string.Empty;
    public int Order { get; set; }
}
