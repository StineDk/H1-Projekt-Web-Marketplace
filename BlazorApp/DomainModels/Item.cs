using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModels
{
    public abstract class Item
    {
        public int itemID;
        public string gameName;
        public int price;
        public bool orderStatus;
        public string condition;
        public DateTime created;
        public DateTime updated;
        public string genre;
        public string manufacture;
        public bool addToFaverite;
        public string description;
        public string type;

        public int userID { get; set; }

        void SetItem() { }
        void AddItem() { }
        void RemoveItem() { }
        void ItemStatus() { }
        void GetItem() { } 
    }
}