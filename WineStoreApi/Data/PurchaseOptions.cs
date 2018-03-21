using System;
namespace WineStoreApi.Data
{
    public class PurchaseOptions
    {
        public string TrolleyAPIKey { get; set; }
        public string TrolleyAPI { get; set; }
        public string InventoryAPIKey { get; set; }
        public string InventoryAPI { get; set; }
        public string SendGridAPIKey { get; set; }
    }
}
