using MySql.Data.MySqlClient;
using serverSide.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace serverSide.Utils
{
    public static class DbUtils
    {
        private static readonly string connectionString = "Server=localhost; Database=shop_db;  UID=root;  PWD=1135;";

        public static List<T> ExecuteSelectQuery<T>(string query) where T : new()
        {
            List<T> results = new List<T>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T result = MapToObject<T>(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        public static int ExecuteNonQuery(string query)
        {
            int insertedId = -1; // Default value if no ID is retrieved

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Execute the query
                    command.ExecuteNonQuery();

                    // Retrieve the ID of the last inserted row
                    insertedId = (int)command.LastInsertedId;
                }
            }

            return insertedId;
        }

        //public static T MapToObject<T>(MySqlDataReader reader) where T : new()
        //{
        //    T obj = new T();
        //    foreach (var prop in typeof(T).GetProperties())
        //    {
        //        if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
        //        {
        //            prop.SetValue(obj, reader[prop.Name]);
        //        }
        //    }
        //    return obj;
        //}

        public static T MapToObject<T>(MySqlDataReader reader) where T : new()
        {
            T obj = new T();
            foreach (var prop in typeof(T).GetProperties())
            {
                if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                {
                    object value = reader[prop.Name];
                    Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    try
                    {
                        // Check if property type is nullable
                        if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                        {
                            // Convert DBNull to null for nullable types
                            value = Convert.ChangeType(value, propType);
                        }
                        else
                        {
                            // Convert DBNull to default value for non-nullable types
                            value = value == DBNull.Value ? Activator.CreateInstance(propType) : Convert.ChangeType(value, propType);
                        }
                    }
                    catch (System.InvalidCastException ex)
                    {
                        Console.WriteLine(typeof(T));
                        if (typeof(T) == typeof(User))
                        {
                            value = (Roles)Enum.Parse(typeof(Roles), value.ToString());
                        }
                        if (typeof(T) == typeof(Order))
                        {
                            value = (OrderStatus)Enum.Parse(typeof(OrderStatus), value.ToString());
                        }
                        //if (prop.PropertyType.IsEnum)
                        //{
                        //    // Convert the value to the enum type
                        //    value = Enum.Parse(prop.PropertyType, value.ToString());
                        //}
                    }
                    prop.SetValue(obj, value);
                }
            }
            return obj;
        }
    }
}
