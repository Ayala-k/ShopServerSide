using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace serverSide.Utils
{
    public static class DbUtils
    {
        public static List<T> ExecuteQuery<T>(string query) where T : new()
        {
            //string connectionString = Configuration.GetConnectionString("DBConnection");
            string connectionString = "Server=localhost; Database=shop_db;  UID=root;  PWD=1135;";
            
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
