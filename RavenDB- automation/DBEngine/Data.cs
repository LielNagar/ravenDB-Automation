using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBEngine
{
    public class Data
    {
        private List<User> users;
        private List<Order> orders;

        public Data(List<User> users, List<Order> orders)
        {
            this.users = users;
            this.orders = orders;
        }

        public List<User> Users { get => users; set => users = value; }
        public List<Order> Orders { get => orders; set => orders = value; }
    }
}
