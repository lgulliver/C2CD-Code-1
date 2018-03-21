using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using WineStoreShared;
using WineStoreTrolley.Data;

namespace WineStoreTrolley.Controllers
{
    [Route("api/[controller]")]
    public class TrolleyController : Controller
    {
        private readonly APIOptions _options;
        private TrolleyBroker _broker;

        public TrolleyController(IOptions<APIOptions> optionsAccessor)
        {
            _broker = new TrolleyBroker();
            _options = optionsAccessor.Value;
        }

        // POST api/trolley
        [HttpPost]
        public string Post([FromBody]APIPackage package)
        {
            var sessionId = package.sessionIdentifier;
            var apiKey = package.apiKey;

            if (sessionId == null || apiKey == null)
            {
                throw new InvalidOperationException("package invalid.");
            }

            if (!apiKey.Equals(_options.MyAPIKey))
            {
                throw new InvalidOperationException("api key not valid.");
            }

            return _broker.GetItemsInTrolley(_options.StorageConnectionString, sessionId);
        }

        // PUT api/trolley/5
        [HttpPut("{id}")]
        public string Put(string id, [FromBody]APIPackage package)
        {
            var sessionId = package.sessionIdentifier;
            var apiKey = package.apiKey;
            var sessionIdGet = id;

            if (sessionId == null || apiKey == null)
            {
                throw new InvalidOperationException("package invalid.");
            }

            if (!apiKey.Equals(_options.MyAPIKey))
            {
                throw new InvalidOperationException("api key not valid.");
            }

            if (!sessionIdGet.Equals(sessionId)) {
                throw new InvalidOperationException("session id does not match criteria.");
            }

            int newNumberOfItems = _broker.ChangeItemInTrolley(_options.StorageConnectionString, sessionId, package.contentItem);

            return newNumberOfItems.ToString();
        }

        // DELETE api/trolley/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _broker.EmptyTrolley(_options.StorageConnectionString, id);
        }
    }


}
