using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModels
{
    public class User
    {
        public string name { get; set; }
        public int userID { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string city { get; set; }
        public DateTime yearCreated { get; set; }
        public int itemsSold { get; set; }
        public List<string> favorites { get; set; }
        public int ItemsViewed { get; set; }
        public List<Item> ListedSales { get; set; } = new List<Item>();
        public int CurrentUserID { get; set; }

        
    }
}