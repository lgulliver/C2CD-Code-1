using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WineStoreShared;

namespace WineStoreWeb.Data
{
    public class InventoryProxy : Proxy
    {
        private static Proxy _instance;

        public InventoryProxy(string endpoint, string password): base(endpoint, password)
        {
            _instance = this;
        }

        public static InventoryProxy GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("Singelton not instantiated at app startup.");
            }

            return (InventoryProxy) _instance;
        }

        public Dictionary<string, WineItem> GetInventory() {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new APIPackage { apiKey = this._password });
            var externalTask = client.PostAsync(this._endpoint + "api/inventory", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();


            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync(); 

            returnedValueTask.Wait();
            var returnedValue = returnedValueTask.Result;

            return JsonConvert.DeserializeObject<Dictionary<string, WineItem>>(returnedValue);
        }

        internal WineItem GetInventoryItem(string key)
        {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new APIPackage { apiKey = this._password });
            var externalTask = client.PostAsync(this._endpoint + "api/inventory/" + key, new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();


            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();

            returnedValueTask.Wait();
            var returnedValue = returnedValueTask.Result;

            return JsonConvert.DeserializeObject<WineItem>(returnedValue);
        }

    }
}
