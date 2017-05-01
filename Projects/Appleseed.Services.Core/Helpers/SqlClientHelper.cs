using System.Data;
using System.Data.SqlClient;

namespace Appleseed.Services.Core.Helpers
{
    using System;

    public static class SqlClientHelper
    {
        public static SqlDataReader GetReader(string connectionString, string commandText)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            using (var command = new SqlCommand(commandText, connection))
            {
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public static SqlDataReader GetReader(string connectionString, CommandType commandType, string commandText)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            using (var command = new SqlCommand(commandText, connection) { CommandType = commandType })
            {
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public static SqlDataReader GetReader(string connectionString, CommandType commandType, string commandText, SqlParameter[] parameters)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            using (var command = new SqlCommand(commandText, connection) { CommandType = commandType })
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;
                    command.Parameters.AddRange(parameters);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static string ExecuteScalar(string connectionString, CommandType commandType, string commandText, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;
                    command.Parameters.AddRange(parameters);
                    var result = command.ExecuteScalar();
                    if (result == null)
                    {
                        return string.Empty;
                    }

                    return result.ToString();
                }
            }
        }

        public static string GetNullableString(SqlDataReader reader, string fieldName, string defaultValue)
        {
            var index = reader.GetOrdinal(fieldName);
            return GetNullableString(reader, index, defaultValue);
        }

        public static string GetNullableString(SqlDataReader reader, int fieldIndex)
        {
            return GetNullableString(reader, fieldIndex, null);
        }

        public static string GetNullableString(SqlDataReader reader, int fieldIndex, string defaultValue)
        {
            if (reader.IsDBNull(fieldIndex))
            {
                return defaultValue;
            }

            return reader.GetString(fieldIndex);
        }
    }
}