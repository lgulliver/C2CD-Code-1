using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WineStoreShared;

namespace WineStoreWeb.Models
{
    public class StoreViewModel
    {
        private List<WineItem> winesToDisplay = new List<WineItem>();

        public void AddWineToDisplay(WineItem wineItem)
        {
            if(winesToDisplay == null) {
                winesToDisplay = new List<WineItem>();
            }

            winesToDisplay.Add(wineItem);
        }

        public List<WineItem> WinesToDisplay { get { return winesToDisplay; }}
    }
}
