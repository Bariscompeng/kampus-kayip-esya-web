namespace KampusKayipEsya.Web.Dtos;

public record CategoryDto(int Id, string Name, string? Icon);

public record LocationDto(int Id, string Building, string? FloorOrRoom, double? Latitude, double? Longitude);
