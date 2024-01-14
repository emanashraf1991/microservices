using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service
{
    public record ItemDto(Guid Id, string Name, string Description, decimal price, DateTimeOffset CreateDate);
    public record CreateItemDto([Required] string Name, string Description, [Range(0, 1000000)] decimal price);
    public record UpdateItemDto([Required] string Name, string Description, [Range(0, 1000000)] decimal price);
}