using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using WineStoreShared;

namespace WineStoreApi.Data
{
    public class PurchaseBroker
    {
        public PurchaseBroker()
        {
        }

        internal string CheckoutTrolley(string sessionId, string customerEMail, string sendGridKey, string trolleyEndpoint, string trolleyKey, string inventoryEndpoint, string inventoryKey)
        {
            if(!trolleyEndpoint.EndsWith("/", StringComparison.InvariantCulture)) {
                trolleyEndpoint = trolleyEndpoint + "/"; 
            }

            if (!inventoryEndpoint.EndsWith("/", StringComparison.InvariantCulture))
            {
                inventoryEndpoint = inventoryEndpoint + "/";
            }

            // get everything in the trolley
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new APIPackage { sessionIdentifier = sessionId, apiKey = trolleyKey });
            var externalTask = client.PostAsync(trolleyEndpoint + "api/trolley", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();
            var returnedValue = returnedValueTask.Result;

            var trolleyItems = returnedValue.Split(";")[1].Split(",");

            // confirm stock for the order and calculate total
            var preDict = new Dictionary<string, int>();
            foreach (var item in trolleyItems)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                if (preDict.ContainsKey(item))
                {
                    preDict[item]++;
                }
                else
                {
                    preDict.Add(item, 1);
                }
            }

            if(preDict.Keys.Count < 1) {
                return "Failed;No items in the trolley";
            }

            double total = 0.0;
            Dictionary<WineItem, int> wineDict = new Dictionary<WineItem, int>();
            foreach (var key in preDict.Keys)
            {
                var wine = this.GetInventoryItem(key, inventoryEndpoint, inventoryKey);
                if(wine.WineInStock < preDict[key]) {
                    return "Failed;Unfortunately we have only got " + wine.WineInStock + " bottles of " + wine.WineName + " left in stock. Please adjust your order so that we can accept it";
                }
                wineDict.Add(wine, preDict[key]);
                total = total + (wine.WinePrice * preDict[key]);
            }

            // now we have got a total and could charge the customer
            // we are not doing this here as it is just a demo

            // adjust stock
            var instructionString = "";
            foreach (var key in wineDict.Keys) {
                instructionString += key.WineType + ":" + key.WineId + ":-" + wineDict[key] + ";";        
            }

            content = JsonConvert.SerializeObject(new APIPackage { apiKey = inventoryKey, sessionIdentifier = sessionId, contentItem = instructionString });
            externalTask = client.PutAsync(inventoryEndpoint + "api/inventory", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();

            returnedValueTask.Wait();
            returnedValue = returnedValueTask.Result;

            if(!returnedValue.Equals("DONE!")) {
                return "Failed;An error occured while adjusting our stock, please try again.";
            }

            // now lets empty the trolley
            externalTask = client.DeleteAsync(trolleyEndpoint + "api/trolley/" + sessionId);
            externalTask.Wait();
            // would be worth adding some retry logic here

            // all done, now let's confirm the order
            var sendGridClient = new SendGridClient(sendGridKey);

            var formattedMessage = "<h3>Thank you for your order at Wine ONLINE</h3><p>You ordered the following items:</p>";
            var unformattedMessage = "Thank you for your order at Wine ONLINE! \r\n You ordered the following items: \r\n \r\n";

            foreach(var wine in wineDict) {
                formattedMessage = formattedMessage + "<p>" + wine.Value + "x <b>" + wine.Key.WineName + "</b>" + " at <i>&#163; " + String.Format("{0:N2}", wine.Key.WinePrice) + "</i> per bottle, totalling at <b>&#163; " + String.Format("{0:N2}", (wine.Key.WinePrice * wine.Value)) + "</b></p>";
                unformattedMessage = unformattedMessage + wine.Value + "x " + wine.Key.WineName + " at GBP " + String.Format("{0:N2}", wine.Key.WinePrice) + " per bottle, totalling at GBP " + String.Format("{0:N2}", (wine.Key.WinePrice * wine.Value)) + " \r\n";
            }

            formattedMessage = formattedMessage + "<h3>Total: &#163; " + String.Format("{0:N2}", total) + "</h3>";
            unformattedMessage = unformattedMessage + " \r\n \r\n Total: GBP " + String.Format("{0:N2}", total);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("orders@wine.online", "Wine Online Team"),
                Subject = "Thank you for your order at Wine ONLINE",
                PlainTextContent = unformattedMessage,
                HtmlContent = formattedMessage
            };
            msg.AddTo(new EmailAddress(customerEMail, "Order Receipient"));
            var responseTask = sendGridClient.SendEmailAsync(msg);

            try
            {
                responseTask.Wait();
            } catch {
                return "Success;Thank you for your order! Unfortunately we are unable to confirm over e-mail on this occassion. We will endeavour to contact you manually at the earliest opportunity. Your order has gone through as normal.";
            }

            return "Success;Thank you for your order! You should receive an e-mail to confirm your order soon.";
        }

        private WineItem GetInventoryItem(string key, string inventoryApi, string inventoryKey)
        {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new APIPackage { apiKey = inventoryKey });
            var externalTask = client.PostAsync(inventoryApi + "api/inventory/" + key, new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();


            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();

            returnedValueTask.Wait();
            var returnedValue = returnedValueTask.Result;

            return JsonConvert.DeserializeObject<WineItem>(returnedValue);
        }
    }
}
