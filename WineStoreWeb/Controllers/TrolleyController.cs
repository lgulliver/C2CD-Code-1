using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WineStoreWeb.Data;
using WineStoreWeb.Models;

namespace WineStoreWeb.Controllers
{
    public class TrolleyController : Controller
    {
        public IActionResult Index()
        {
            ViewData["TrolleyItems"] = TrolleyProxy.GetInstance().GetCurrentNumberOfItems(HttpContext.Session.Id.ToString());

            var trolleyModel = new TrolleyViewModel();
            var trolleyItems = TrolleyProxy.GetInstance().GetCurrentTrolleyItems(HttpContext.Session.Id);

            Dictionary<string, int> preDict = new Dictionary<string, int>();
            foreach(var item in trolleyItems) {
                if(string.IsNullOrWhiteSpace(item)) {
                    continue;
                }

                if(preDict.ContainsKey(item)) {
                    preDict[item]++;
                } else {
                    preDict.Add(item, 1);
                }
            }

            double total = 0.0;
            foreach(var key in preDict.Keys) {
                var wine = InventoryProxy.GetInstance().GetInventoryItem(key);
                trolleyModel.AddTrolleyItemToDisplay(wine, preDict[key]);
                total = total + (wine.WinePrice * preDict[key]);
            }

            ViewData["Total"] = String.Format("{0:N2}", total);

            return View(trolleyModel);
        }

        public IActionResult Remove(string wineTypeAndId) {
            TrolleyProxy.GetInstance().RemoveItem(HttpContext.Session.Id, wineTypeAndId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Order(string EMail) {

            var trolleyModel = new TrolleyViewModel();
            ViewData["CustomerEMail"] = EMail;
            ViewData["OrderResult"] = "Your order has gone through! We will confirm over e-mail shortly.";
            ViewData["OrderBool"] = true;

            ViewData["TrolleyItems"] = TrolleyProxy.GetInstance().GetCurrentNumberOfItems(HttpContext.Session.Id.ToString());

            if(string.IsNullOrWhiteSpace(EMail)) {
                ViewData["OrderResult"] = "E-Mail address not valid. Please try again.";
                ViewData["OrderBool"] = false;
            } else {
                if (!EMail.Contains('@') || !EMail.Contains('.'))
                {
                    ViewData["OrderResult"] = "E-Mail address not valid.  Please try again.";
                    ViewData["OrderBool"] = false;
                }

            }


            var trolleyItems = TrolleyProxy.GetInstance().GetCurrentTrolleyItems(HttpContext.Session.Id);

            Dictionary<string, int> preDict = new Dictionary<string, int>();
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

            double total = 0.0;
            foreach (var key in preDict.Keys)
            {
                var wine = InventoryProxy.GetInstance().GetInventoryItem(key);
                trolleyModel.AddTrolleyItemToDisplay(wine, preDict[key]);
                total = total + (wine.WinePrice * preDict[key]);
            }

            ViewData["Total"] = String.Format("{0:N2}", total);

            if ((bool)ViewData["OrderBool"] == false)
            {
                return View(trolleyModel);
            }

            var orderResult = PurchaseProxy.GetInstance().TryOrder(HttpContext.Session.Id, EMail);

            var orderArray = orderResult.Split(";");
            if (orderArray[0].Equals("Success"))
            {
                ViewData["OrderBool"] = true;
                ViewData["OrderResult"] = orderArray[1];
                ViewData["TrolleyItems"] = "0";
            }
            else
            {
                ViewData["OrderBool"] = false;
                ViewData["OrderResult"] = orderArray[1];
            }

            return View(trolleyModel);
        }
    }
}