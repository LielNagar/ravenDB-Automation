using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBEngine
{
    public class QueryEngine
    {
        private string? _source;
        private string? _expression;
        private string? _field;
        private string[]? _fieldsArray;
        private string[]? _conditions;

        public string Source { get => _source; set => _source = value; }
        public string[] FieldsArray { get => _fieldsArray; set => _fieldsArray = value; }

        public bool validateQueryProperties(string source)
        {
            switch (source)
            {
                case "Orders":
                    Order orderSample = new Order();
                    var orderProperties = orderSample.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    List<string> orderPropertyNames = new List<string>();

                    foreach (var property in orderProperties)
                    {
                        var orderPropertyName = property.Name.ToLower();
                        orderPropertyNames.Add(orderPropertyName);
                    }
                    //Check that each property is a field
                    foreach (string str in _fieldsArray.Concat(_conditions))
                    {

                        MatchCollection matches = Regex.Matches(str, @"\b(\w+)\b");
                        string propertyToCheck = matches[0].Groups[1].Value;
                        if (!orderPropertyNames.Contains(propertyToCheck))
                        {
                            Console.WriteLine("Orders table doesn't contains one of those fields");
                            return false;
                        }
                    }
                    return true;
                case "Users":
                    User userSample = new User();
                    var userProperties = userSample.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    List<string> userPropertyNames = new List<string>();

                    foreach (var property in userProperties)
                    {
                        var userPropertyName = property.Name.ToLower();
                        userPropertyNames.Add(userPropertyName);
                    }
                    //Check that each property is a field
                    foreach (string str in _fieldsArray.Concat(_conditions))
                    {

                        MatchCollection matches = Regex.Matches(str, @"\b(\w+)\b");
                        string propertyToCheck = matches[0].Groups[1].Value;
                        if (!userPropertyNames.Contains(propertyToCheck))
                        {
                            Console.WriteLine("Users table doesn't contains one of those fields");
                            return false;
                        }
                    }
                    return true;
                    default: return false;
            }
        }
        public bool validateQueryString(string sqlQuery)
        {
            // Using Regular Expression to extract the parts
            Match match = Regex.Match(sqlQuery, @"from\s+(\w+)\s+where\s+(\S+)\s+select\s+([\w\s,]+)");

            if (match.Success)
            {
                this._source = match.Groups[1].Value;
                this._expression = match.Groups[2].Value;
                this._field = match.Groups[3].Value;
                this._fieldsArray = this._field.Split(',').Select(f => f.Trim()).ToArray();
                this._conditions = Regex.Split(_expression, @"(?<!\bor\b|\band\b)\s+(?:or|and)\s+", RegexOptions.IgnoreCase);

                if (_source != "Orders" && _source != "Users")
                {
                    Console.WriteLine("Invalid SQL string format");
                    return false;
                }
                if (_source == "Orders")
                {
                    if (!validateQueryProperties("Orders")) return false;
                }
                else if (_source == "Users")
                {
                    if (!validateQueryProperties("Users")) return false;
                }

                Console.WriteLine("Source: " + _source);
                Console.WriteLine("Expression: " + _expression);
                Console.WriteLine("Field: " + _field);
                return true;
            }
            else
            {
                Console.WriteLine("Invalid SQL string format");
                return false;
            }
        }

        public (List<User>,List<Order>) Query(Data data)
        {
           if(_source == "Users")
           {
                List<User> users = new List<User>();
                foreach (User user in data.Users)
                {
                    if (CheckCondition(user, _expression)) users.Add(user);
                }
                return (users, null);
            }
            else if(_source == "Orders")
            {
                List<Order> orders = new List<Order>();
                foreach (Order order in data.Orders)
                {
                    if (CheckCondition(order, _expression)) orders.Add(order);
                }
                return (null, orders);
            }
            return (null, null);
        }
        public bool CheckCondition<T>(T obj, string condition)
        {
            try
            {
                // Split the condition by logical operators (and/or)
                string[] conditions = Regex.Split(_expression, @"(?<!\bor\b|\band\b)\s+(?:or|and)\s+", RegexOptions.IgnoreCase);

                // Create a dynamic parameter for the lambda expression
                ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

                // Initialize the combined expression with true for 'and' logic and false for 'or' logic
                Expression combinedExpression = conditions[0].Trim().StartsWith("!")
                    ? Expression.Constant(false)
                    : Expression.Constant(true);

                // Process each individual condition
                foreach (string cond in conditions)
                {
                    string individualCondition = cond.Trim();
                    bool isNotCondition = individualCondition.StartsWith("!");
                    string cleanCondition = individualCondition.TrimStart('!').Trim();

                    Expression expression = DynamicExpressionParser.ParseLambda(new[] { parameter }, typeof(bool), cleanCondition).Body;

                    if (isNotCondition)
                    {
                        expression = Expression.Not(expression);
                    }

                    if (individualCondition.Contains("||"))
                    {
                        // Combine with 'or' logic
                        combinedExpression = Expression.OrElse(combinedExpression, expression);
                    }
                    else
                    {
                        // Combine with 'and' logic
                        combinedExpression = Expression.AndAlso(combinedExpression, expression);
                    }
                }

                // Compile the combined expression with the provided object
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                bool isValidCondition = lambda.Compile()(obj);

                return isValidCondition;
            }
            catch
            {
                // An error occurred while parsing or evaluating the expression
                return false;
            }
        }
    }
}
