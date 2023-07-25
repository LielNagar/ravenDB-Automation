// See https://aka.ms/new-console-template for more information

using DBEngine;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;

Data data = initializeData();

while (true)
{
    Console.WriteLine("Hello, enter your SQL query string:");
    string sqlQuery = Console.ReadLine();
    QueryEngine engine = new QueryEngine();
    if (engine.validateQueryString(sqlQuery))
    {
        var result = engine.Query(data);
        List<User> users = result.Item1;
        List<Order> orders = result.Item2;
        int matrixRows = 0;
        List<string> matrixColumns = new List<string>();
        string[,] tableView = null;
        switch (engine.Source)
        {
            case "Orders":
                Order orderSample = new Order();
                PropertyInfo[] orderProperties = orderSample.GetType().GetProperties();
                foreach (var property in orderProperties)
                {
                    matrixColumns.Add(property.Name);
                }
                matrixRows = orders.Count+1;
                tableView = new string[matrixRows,matrixColumns.Count-1];
                for (int i = 0; i < matrixRows; i++)
                {
                    for (int j = 0; j < engine.FieldsArray.Length; j++)
                    {
                        if (i == 0)
                        {
                            tableView[i, j] = engine.FieldsArray[j];
                        }
                        else
                        {
                            switch (engine.FieldsArray[j])
                            {
                                case "orderid":
                                    tableView[i, j] = orders[i-1].OrderID.ToString();
                                    break;
                                case "amount":
                                    tableView[i, j] = orders[i-1].Amount.ToString();
                                    break;
                                case "date":
                                    tableView[i, j] = orders[i - 1].Date.ToString();
                                    break;
                                case "userid":
                                    tableView[i, j] = orders[i - 1].UserID.ToString();
                                    break;
                            }
                        }
                    }
                }
                break;
            case "Users":
                User userSample = new User();
                PropertyInfo[] userProperties = userSample.GetType().GetProperties();
                foreach (var property in userProperties)
                {
                    matrixColumns.Add(property.Name);
                }
                matrixRows = users.Count + 1;
                tableView = new string[matrixRows, matrixColumns.Count - 1];
                for (int i = 0; i < matrixRows; i++)
                {
                    for (int j = 0; j < engine.FieldsArray.Length; j++)
                    {
                        if (i == 0)
                        {
                            tableView[i, j] = engine.FieldsArray[j];
                        }
                        else
                        {
                            switch (engine.FieldsArray[j].ToLower())
                            {
                                case "id":
                                    tableView[i, j] = users[i - 1].Id.ToString();
                                    break;
                                case "fullname":
                                    tableView[i, j] = users[i - 1].FullName.ToString();
                                    break;
                                case "city":
                                    tableView[i, j] = users[i - 1].City.ToString();
                                    break;
                                case "age":
                                    tableView[i, j] = users[i - 1].Age.ToString();
                                    break;
                                case "email":
                                    tableView[i, j] = users[i - 1].Email.ToString();
                                    break;
                            }
                        }
                    }
                }
                break;
        }

        for (int i = 0; i < matrixRows; i++)
        {
            for (int j = 0; j < matrixColumns.Count-1; j++)
            {
                Console.Write(tableView[i, j] + "\t");
            }
            Console.WriteLine();
        }
    }
}

static Data initializeData()
{
    List<User> users = initializeUsers();
    List<Order> orders = initializeOrders();
    Data data = new Data(users, orders);
    return data;
}

static List<User> initializeUsers()
{
    Random rnd = new Random();
    List<User> users = new List<User>();
    string[] lastName = { "Cohen", "Levi", "Avraham", "Eini", "Nagar", "Noy", "Yosef", "Israeli" };
    string[] firstName = { "John", "Israel", "Liel", "Oren", "Egor", "Dor", "Yuval" };
    string[] cities = { "Tel Aviv", "Hadera", "Qadima", "Tzoran", "Tel Mond", "Netanya" };

    //Create 20 users
    for (int i = 0; i < 20; i++)
    {
        int userID = i + 1;
        string userFirst = firstName[rnd.Next(firstName.Length-1)];
        string userLast = lastName[rnd.Next(lastName.Length - 1)];
        string userCity = cities[rnd.Next(cities.Length-1)];
        string userEmail = userFirst + "." + userLast + "@gmail.com"; // Assuming that email can be the same for simplicity
        userEmail = userEmail.ToLower();
        int userAge = rnd.Next(20, 50);

        User user = new User(userFirst + " " + userLast, userID, userCity, userAge, userEmail);
        users.Add(user);
    }

    users.Add(new User("John Doe",21,"Hadera", 30, "jobs@ravendb.net"));
    users.Add(new User("Hibernate Rhino",22,"Hadera", 30, "jobs@hibernatingrhinos.com"));


    return users;
}

static List<Order> initializeOrders()
{
    List<Order> orders = new List<Order>();
    Random rnd = new Random();
    DateTime startDate = new DateTime(2023, 1, 1);
    DateTime endDate = new DateTime(2023, 12, 31);
    int totalDays = (endDate - startDate).Days;

    //Create 10 Orders
    for (int i = 0; i < 10; i++)
    {
        int orderID= i + 1;
        int userID = rnd.Next(1, 20);
        int randomDays = rnd.Next(totalDays);
        DateTime randomDate = startDate.AddDays(randomDays);
        int orderAmount = rnd.Next(900, 10000);

        Order order = new Order(orderID, userID, randomDate, orderAmount);
        orders.Add(order);
    }

    return orders;
}