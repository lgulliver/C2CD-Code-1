using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WineStoreApi.Data;
using WineStoreShared;

namespace WineStoreApi.Controllers
{
    [Route("api/[controller]")]
    public class PurchaseController : Controller
    {
        private readonly APIOptions _apiOptions;
        private readonly PurchaseOptions _purchaseOptions;
        private PurchaseBroker _broker;

        public PurchaseController(IOptions<APIOptions> apiOptionsAccessor, IOptions<PurchaseOptions> purchaseOptionsAccessor)
        {
            _apiOptions = apiOptionsAccessor.Value;
            _purchaseOptions = purchaseOptionsAccessor.Value;
            _broker = new PurchaseBroker();
        }

        // POST api/purchase
        [HttpPost]
        public string Post([FromBody]APIPackage package)
        {
            var result = _broker.CheckoutTrolley(package.sessionIdentifier, package.contentItem, _purchaseOptions.SendGridAPIKey, _purchaseOptions.TrolleyAPI, _purchaseOptions.TrolleyAPIKey, _purchaseOptions.InventoryAPI, _purchaseOptions.InventoryAPIKey);

            return result;
        }

    }
}
