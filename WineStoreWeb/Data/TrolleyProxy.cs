using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using WineStoreShared;
using Microsoft.AspNetCore.Mvc;

namespace WineStoreWeb.Data
{
    public class TrolleyProxy : Proxy
    {
        private static Proxy _instance;

        public TrolleyProxy(string endpoint, string password) : base(endpoint, password)
        {
            _instance = this;
        }

        public static TrolleyProxy GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("Singelton not instantiated at app startup.");
            }

            return (TrolleyProxy)_instance;
        }

        public int GetCurrentNumberOfItems(string sessionIdentifier)
        {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new APIPackage { sessionIdentifier = sessionIdentifier, apiKey = this._password });
            var externalTask = client.PostAsync(this._endpoint + "api/trolley", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();
            var returnedValue = returnedValueTask.Result;

            return int.Parse(returnedValue.Split(";")[0]);
        }

        public string[] GetCurrentTrolleyItems(string sessionIdentifier)
        {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new APIPackage { sessionIdentifier = sessionIdentifier, apiKey = this._password });
            var externalTask = client.PostAsync(this._endpoint + "api/trolley", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();
            var returnedValue = returnedValueTask.Result;

            return returnedValue.Split(";")[1].Split(",");
        }

        public int AddItem(string sessionIdentifier, string typeAndIdString)
        {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new APIPackage { sessionIdentifier = sessionIdentifier, apiKey = this._password, contentItem = typeAndIdString });
            var externalTask = client.PutAsync(this._endpoint + "api/trolley/" + sessionIdentifier, new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();
            var returnedValue = returnedValueTask.Result;

            return int.Parse(returnedValue);
        }

        public int RemoveItem(string sessionIdentifier, string typeAndIdString)
        {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new APIPackage { sessionIdentifier = sessionIdentifier, apiKey = this._password, contentItem = ("-" + typeAndIdString) });
            var externalTask = client.PutAsync(this._endpoint + "api/trolley/" + sessionIdentifier, new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();
            var returnedValue = returnedValueTask.Result;

            return int.Parse(returnedValue);
        }
    }


}
