using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using Avto.Models;
using System.Configuration;

namespace Avto.Data
{
    internal class AuthHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["AvtoDb"].ConnectionString;

        public static User Authenticate(string username, string password)
        {
            string hashedPassword = ComputeSha256Hash(password);
            User user = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT id, username, password_hash, full_name, role, is_active 
                               FROM users 
                               WHERE username = @username AND password_hash = @hash AND is_active = 1";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@hash", hashedPassword);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            Id = reader.GetInt32("id"),
                            Username = reader.GetString("username"),
                            PasswordHash = reader.GetString("password_hash"),
                            FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString("full_name"),
                            Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString("role")),
                            IsActive = reader.GetBoolean("is_active")
                        };
                    }
                }
            }
            return user;
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}