using MySql.Data.MySqlClient;
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

        public static void ExecuteNonQuery(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static T MapToObject<T>(MySqlDataReader reader) where T : new()
        {
            T obj = new T();
            foreach (var prop in typeof(T).GetProperties())
            {
                if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                {
                    prop.SetValue(obj, reader[prop.Name]);
                }
            }
            return obj;
        }
    }
}
