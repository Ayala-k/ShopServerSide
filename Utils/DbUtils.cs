using MySql.Data.MySqlClient;
using serverSide.Exceptions;
using serverSide.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace serverSide.Utils;

public static class DbUtils
{
    private static readonly string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

    public static List<T> ExecuteSelectQuery<T>(string query) where T : new()
    {
        List<T> results = new List<T>();

        try
        {

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
        }
        catch (Exception ex)
        {
            throw new InternalDataBaseException();
        }

        if (results.Count == 0)
        {
            throw new DataNotFoundException("Data not found");
        }

        return results;

    }



    public static int ExecuteNonQuery(string query)
    {
        try
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
        catch (MySql.Data.MySqlClient.MySqlException ex) when (ex.Number == 1062)
        {
            throw new DataAlreadyExistsException("Data already exists");
        }
        catch (Exception ex)
        {
            throw new InternalDataBaseException();
        }
    }

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
                    if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                    {
                        value = Convert.ChangeType(value, propType);
                    }
                    else
                    {
                        value = value == DBNull.Value ? Activator.CreateInstance(propType) : Convert.ChangeType(value, propType);
                    }
                }
                catch (System.InvalidCastException ex)
                {
                    if (typeof(T) == typeof(User))
                    {
                        value = (Roles)Enum.Parse(typeof(Roles), value.ToString());
                    }
                    if (typeof(T) == typeof(Order))
                    {
                        value = (OrderStatus)Enum.Parse(typeof(OrderStatus), value.ToString());
                    }
                    if (typeof(T) == typeof(Item))
                    {
                        value = (InstrumentalCategory)Enum.Parse(typeof(InstrumentalCategory), value.ToString());
                    }
                }
                prop.SetValue(obj, value);
            }
        }
        return obj;
    }
}
