using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemRepository;
        private readonly CatalogClient catalogClient;
        public ItemsController(IRepository<InventoryItem> _itemRepository, CatalogClient _catalogClient)
        {
            itemRepository = _itemRepository;
            catalogClient = _catalogClient;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            { return BadRequest(); }

            var catalogItems = await catalogClient.GetCatalogItemAsync();
            var inventoryItemEntities = await itemRepository.GetAllAsync(i => i.UserId == userId);
            var inventoryItemsDto = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });
            return Ok(inventoryItemsDto);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemDto itemDto)
        {
            var foundItem = await itemRepository.GetAsync(i => i.UserId == itemDto.UserId && i.CatalogItemId == itemDto.CatalogItemId);
            if (foundItem == null)
            {
                var inventoryItem = new InventoryItem
                {
                    CatalogItemId = itemDto.CatalogItemId,
                    Quantity = itemDto.Quantity,
                    AcquireDate = DateTime.UtcNow,
                    UserId = itemDto.UserId
                };
                await itemRepository.CreateAsync(inventoryItem);
            }
            else
            {
                foundItem.Quantity += itemDto.Quantity;
                await itemRepository.UpdateAsync(foundItem);
            }
            return Ok();
        }
    }
}