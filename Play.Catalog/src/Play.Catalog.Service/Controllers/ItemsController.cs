
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Entities;
using Play.Common;
using Play.Catalog.Contracts;
namespace Play.Catalog.Service
{
    [ApiController]
    [Route("Items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> _itemRepository, IPublishEndpoint _publishEndpoint)
        {
            itemRepository = _itemRepository;
            publishEndpoint = _publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            var items = (await itemRepository.GetAllAsync())
                        .Select(item => item.AsDto());

            return Ok(items);
        }
        [HttpGet("{Id}", Name = "GetByIdAsync")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid Id)
        {
            var item = await itemRepository.GetAsync(Id);
            if (item == null)
                return NotFound();
            return item.AsDto();
        }
        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item { Id = Guid.NewGuid(), Name = createItemDto.Name, Desription = createItemDto.Description, Price = createItemDto.price, CreateDate = DateTimeOffset.UtcNow };

            await itemRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Desription));

            return CreatedAtRoute(nameof(GetByIdAsync), new { Id = item.Id }, item);
        }
        [HttpPut]
        public async Task<IActionResult> PutAsync(Guid Id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemRepository.GetAsync(Id);
            if (existingItem == null)
                return NotFound();

            existingItem.Name = updateItemDto.Name;
            existingItem.Desription = updateItemDto.Description;
            existingItem.Price = updateItemDto.price;

            await itemRepository.UpdateAsync(existingItem);
            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Desription));

            return NoContent();
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteItem(Guid Id)
        {
            var existingItem = await itemRepository.GetAsync(Id);
            if (existingItem == null)
                return NotFound();
            await itemRepository.RemoveAsync(existingItem.Id);
            await publishEndpoint.Publish(new CatalogItemDeleted(existingItem.Id));

            return NoContent();
        }
    }
}