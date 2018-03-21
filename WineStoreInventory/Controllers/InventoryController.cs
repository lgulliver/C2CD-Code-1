using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WineStoreShared;
using WineStoreInventory.Data;

namespace WineStoreInventory.Controllers
{
    [Route("api/[controller]")]
    public class InventoryController : Controller
    {
        private readonly APIOptions _options;
        private InventoryBroker _broker;

        public InventoryController(IOptions<APIOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
            _broker = new InventoryBroker();
            _broker.PullAsync(_options.StorageConnectionString).Wait();
        }

        // POST api/inventory
        [HttpPost]
        public Dictionary<string, WineItem> Post([FromBody]APIPackage package)
        {
            var sessionId = package.sessionIdentifier;
            var apiKey = package.apiKey;

            if (apiKey == null)
            {
                return new Dictionary<string, WineItem>();
            }

            if (!apiKey.Equals(_options.MyAPIKey))
            {
                return new Dictionary<string, WineItem>();
            }

            return _broker.GetCurrentInventory();
        }

        // POST api/inventory/5
        [HttpPost("{id}")]
        public WineItem PostItem(string id, [FromBody]APIPackage package)
        {
            var sessionId = package.sessionIdentifier;
            var apiKey = package.apiKey;

            if (apiKey == null)
            {
                return null;
            }

            if (!apiKey.Equals(_options.MyAPIKey))
            {
                return null;
            }

            if(string.IsNullOrWhiteSpace(id)) {
                return null;
            }

            if(!id.Contains(":")) {
                return null;
            }

            return _broker.GetInventoryItemWithId(id);
        }

        // PUT api/inventory
        public string Put([FromBody]APIPackage package) {
            
            return _broker.ChangeStock(package.contentItem, _options.StorageConnectionString);
        }
    }
}
