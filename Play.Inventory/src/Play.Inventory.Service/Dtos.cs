using System;

namespace Play.Inventory.Service.Dtos
{
    public record GrantItemDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemId, string name, string Description, int Quantity, DateTimeOffset AcquireDate);
    public record CatalogItemDto(Guid Id, string Name, string Description, decimal price, DateTimeOffset CreateDate);

}
