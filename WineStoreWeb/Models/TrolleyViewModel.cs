using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WineStoreShared;

namespace WineStoreWeb.Models
{
    public class TrolleyViewModel
    {
        private Dictionary<WineItem, int> trolleyItemsToDisplay = new Dictionary<WineItem, int>();

        public void AddTrolleyItemToDisplay(WineItem wineItem, int amount)
        {
            if (trolleyItemsToDisplay == null)
            {
                trolleyItemsToDisplay = new Dictionary<WineItem, int>();
            }

            trolleyItemsToDisplay.Add(wineItem, amount);
        }

        public Dictionary<WineItem, int> TrolleyItemsToDisplay { get { return trolleyItemsToDisplay; } }
    }
}
