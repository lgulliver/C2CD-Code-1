using System;
namespace WineStoreShared
{
    public class WineItem
    {
        public string WineType;
        public string WineId;
        public string WinePicture;
        public string WineName;
        public string WineInfo;
        public double WinePrice;
        public int WineInStock;

        public WineItem(string type, string id, string name, string pictureUri, string info, double price, int stock)
        {
            WineType = type;
            WineId = id;
            WineName = name;
            WinePicture = pictureUri;
            WineInfo = info;
            WinePrice = price;
            WineInStock = stock;
        }

        public bool IsTheSameAs(WineItem other) {
            if(!this.WineType.Equals(other.WineType)) {
                return false;
            }

            if (!this.WineId.Equals(other.WineId))
            {
                return false;
            }

            if (!this.WinePicture.Equals(other.WinePicture))
            {
                return false;
            }

            if (!this.WineInfo.Equals(other.WineInfo))
            {
                return false;
            }

            if (!this.WineName.Equals(other.WineName))
            {
                return false;
            }

            if(!(((this.WinePrice + 0.01) > other.WinePrice) && ((this.WinePrice - 0.01) < other.WinePrice))) {
                return false;                
            }

            if(this.WineInStock != other.WineInStock) {
                return false;
            }

            return true;
        }
    }
}
