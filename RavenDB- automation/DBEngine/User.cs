using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBEngine
{
    public class User
    {
        private string? fullName;
        private int? id;
        private string? city;
        private int? age;
        private string? email;

        public User() { }
        public User(string fullName, int id, string city, int age, string email)
        {
            this.FullName = fullName;
            this.Id = id;
            this.City = city;
            this.Age = age;
            this.Email = email;
        }

        public string? FullName { get => fullName; set => fullName = value; }
        public int? Id { get => id; set => id = value; }
        public string? City { get => city; set => city = value; }
        public int? Age { get => age; set => age = value; }
        public string? Email { get => email; set => email = value; }

        public override string ToString()
        {
            return $"User age is {this.Age} and his name is {this.fullName}, the email is {this.email}";
        }
    }
}
