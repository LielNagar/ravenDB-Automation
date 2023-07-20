using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBEngine
{
    public class Order
    {
        private int orderID;
        private int userID;
        private DateTime date;
        private int amount;

        public Order() { }

        public Order(int orderID, int userID, DateTime date, int amount)
        {
            this.OrderID = orderID;
            this.UserID = userID;
            this.Date = date;
            this.Amount = amount;
        }

        public int OrderID { get => orderID; set => orderID = value; }
        public int UserID { get => userID; set => userID = value; }
        public DateTime Date { get => date; set => date = value; }
        public int Amount { get => amount; set => amount = value; }
    }
}
