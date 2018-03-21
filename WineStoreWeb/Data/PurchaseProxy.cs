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
    public class PurchaseProxy : Proxy
    {
        private static Proxy _instance;

        public PurchaseProxy(string endpoint, string password) : base(endpoint, password)
        {
            _instance = this;
        }

        public static PurchaseProxy GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("Singelton not instantiated at app startup.");
            }

            return (PurchaseProxy) _instance;
        }

        public string TryOrder(string sessionIdentifier, string eMail) {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new APIPackage { sessionIdentifier = sessionIdentifier, apiKey = this._password, contentItem = (eMail) });
            var externalTask = client.PostAsync(this._endpoint + "api/purchase", new StringContent(content, Encoding.UTF8, "application/json"));

            var returnedValue = "Failed;A technical problem occured";

            try
            {
                externalTask.Wait();
                var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
                returnedValueTask.Wait();
                returnedValue = returnedValueTask.Result;
            }
            catch (Exception e)
            {
                // give customer generic failed
                Console.WriteLine(e.InnerException);
            }

            return returnedValue;
        }
    }
}
