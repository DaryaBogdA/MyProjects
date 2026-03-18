using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using Avto.Models;

namespace Avto.Data
{
    internal class DatabaseHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["AvtoDb"].ConnectionString;

        public static List<Vehicle> GetVehicles()
        {
            var vehicles = new List<Vehicle>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT v.*, vt.type_name AS VehicleTypeName
                    FROM vehicles v
                    LEFT JOIN vehicle_types vt ON v.vehicle_type_id = vt.id
                    ORDER BY v.id DESC";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vehicles.Add(MapVehicle(reader));
                    }
                }
            }
            return vehicles;
        }

        public static List<VehicleType> GetVehicleTypes()
        {
            var types = new List<VehicleType>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, type_name, description, created_at FROM vehicle_types ORDER BY type_name";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        types.Add(new VehicleType
                        {
                            Id = reader.GetInt32("id"),
                            TypeName = reader.GetString("type_name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                            CreatedAt = reader.GetDateTime("created_at")
                        });
                    }
                }
            }
            return types;
        }

        public static bool AddVehicle(Vehicle vehicle)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO vehicles 
                    (vehicle_type_id, make, year, number, color, fuel_type, registration_date, status, notes, created_by, created_at, is_available, price_per_hour)
                    VALUES 
                    (@vehicle_type_id, @make, @year, @number, @color, @fuel_type, @registration_date, @status, @notes, @created_by, NOW(), @is_available, @price_per_hour)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@vehicle_type_id", vehicle.VehicleTypeId);
                cmd.Parameters.AddWithValue("@make", vehicle.Make);
                cmd.Parameters.AddWithValue("@year", vehicle.Year);
                cmd.Parameters.AddWithValue("@number", vehicle.Number);
                cmd.Parameters.AddWithValue("@color", vehicle.Color);
                cmd.Parameters.AddWithValue("@fuel_type", vehicle.FuelType.ToString());
                cmd.Parameters.AddWithValue("@registration_date", vehicle.RegistrationDate);
                cmd.Parameters.AddWithValue("@status", vehicle.Status.ToString());
                cmd.Parameters.AddWithValue("@notes", vehicle.Notes);
                cmd.Parameters.AddWithValue("@created_by", vehicle.CreatedBy);
                cmd.Parameters.AddWithValue("@is_available", vehicle.IsAvailable);
                cmd.Parameters.AddWithValue("@price_per_hour", vehicle.PricePerHour);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateVehicle(Vehicle vehicle)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    UPDATE vehicles SET
                        vehicle_type_id = @vehicle_type_id,
                        make = @make,
                        year = @year,
                        number = @number,
                        color = @color,
                        fuel_type = @fuel_type,
                        registration_date = @registration_date,
                        status = @status,
                        notes = @notes,
                        is_available = @is_available,
                        price_per_hour = @price_per_hour
                    WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@vehicle_type_id", vehicle.VehicleTypeId);
                cmd.Parameters.AddWithValue("@make", vehicle.Make);
                cmd.Parameters.AddWithValue("@year", vehicle.Year);
                cmd.Parameters.AddWithValue("@number", vehicle.Number);
                cmd.Parameters.AddWithValue("@color", vehicle.Color);
                cmd.Parameters.AddWithValue("@fuel_type", vehicle.FuelType.ToString());
                cmd.Parameters.AddWithValue("@registration_date", vehicle.RegistrationDate);
                cmd.Parameters.AddWithValue("@status", vehicle.Status.ToString());
                cmd.Parameters.AddWithValue("@notes", vehicle.Notes);
                cmd.Parameters.AddWithValue("@id", vehicle.Id);
                cmd.Parameters.AddWithValue("@is_available", vehicle.IsAvailable);
                cmd.Parameters.AddWithValue("@price_per_hour", vehicle.PricePerHour);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<User> GetUsers()
        {
            var users = new List<User>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, username, full_name, role, is_active FROM users ORDER BY id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32("id"),
                            Username = reader.GetString("username"),
                            FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString("full_name"),
                            Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString("role")),
                            IsActive = reader.GetBoolean("is_active")
                        });
                    }
                }
            }
            return users;
        }

        public static bool AddUser(User user)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO users (username, password_hash, full_name, role, is_active) 
                               VALUES (@username, @password_hash, @full_name, @role, @is_active)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password_hash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@full_name", user.FullName);
                cmd.Parameters.AddWithValue("@role", user.Role.ToString());
                cmd.Parameters.AddWithValue("@is_active", user.IsActive);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateUser(User user)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE users SET 
                        username = @username, 
                        full_name = @full_name, 
                        role = @role, 
                        is_active = @is_active
                        WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@full_name", user.FullName);
                cmd.Parameters.AddWithValue("@role", user.Role.ToString());
                cmd.Parameters.AddWithValue("@is_active", user.IsActive);
                cmd.Parameters.AddWithValue("@id", user.Id);

                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    sql = @"UPDATE users SET 
                        username = @username, 
                        password_hash = @password_hash,
                        full_name = @full_name, 
                        role = @role, 
                        is_active = @is_active
                    WHERE id = @id";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@password_hash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@full_name", user.FullName);
                    cmd.Parameters.AddWithValue("@role", user.Role.ToString());
                    cmd.Parameters.AddWithValue("@is_active", user.IsActive);
                    cmd.Parameters.AddWithValue("@id", user.Id);
                }

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteUser(int userId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM users WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", userId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteVehicle(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM vehicles WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private static Vehicle MapVehicle(MySqlDataReader reader)
        {
            Vehicle v = new Vehicle
            {
                Id = reader.GetInt32("id"),
                VehicleTypeId = reader.IsDBNull(reader.GetOrdinal("vehicle_type_id")) ? null : (int?)reader.GetInt32("vehicle_type_id"),
                Make = reader.GetString("make"),
                Year = reader.IsDBNull(reader.GetOrdinal("year")) ? null : (int?)reader.GetInt32("year"),
                Number = reader.IsDBNull(reader.GetOrdinal("number")) ? null : reader.GetString("number"),
                Color = reader.IsDBNull(reader.GetOrdinal("color")) ? null : reader.GetString("color"),
                FuelType = (FuelType)Enum.Parse(typeof(FuelType), reader.GetString("fuel_type")),
                RegistrationDate = reader.IsDBNull(reader.GetOrdinal("registration_date")) ? null : (DateTime?)reader.GetDateTime("registration_date"),
                Status = (VehicleStatus)Enum.Parse(typeof(VehicleStatus), reader.GetString("status")),
                Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString("notes"),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? null : (int?)reader.GetInt32("created_by"),
                CreatedAt = reader.GetDateTime("created_at"),
                IsAvailable = reader.GetBoolean("is_available"),
                PricePerHour = reader.GetDecimal("price_per_hour")
            };

            if (!reader.IsDBNull(reader.GetOrdinal("VehicleTypeName")))
                v.VehicleTypeName = reader.GetString("VehicleTypeName");

            return v;
        }

        public static List<Vehicle> GetAvailableVehicles()
        {
            var vehicles = new List<Vehicle>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT v.*, vt.type_name AS VehicleTypeName
                    FROM vehicles v
                    LEFT JOIN vehicle_types vt ON v.vehicle_type_id = vt.id
                    WHERE v.is_available = TRUE
                    ORDER BY v.id DESC";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vehicles.Add(MapVehicle(reader));
                    }
                }
            }
            return vehicles;
        }

        public static bool CreateOrder(int vehicleId, int userId, DateTime startTime, DateTime endTime)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string checkSql = "SELECT is_available, price_per_hour FROM vehicles WHERE id = @vehicleId";
                MySqlCommand checkCmd = new MySqlCommand(checkSql, conn);
                checkCmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                using (MySqlDataReader reader = checkCmd.ExecuteReader())
                {
                    if (!reader.Read() || !reader.GetBoolean("is_available"))
                        return false;
                    decimal pricePerHour = reader.GetDecimal("price_per_hour");
                    reader.Close();

                    double hours = (endTime - startTime).TotalHours;
                    decimal totalCost = pricePerHour * (decimal)hours;

                    string sql = @"INSERT INTO orders (vehicle_id, user_id, start_time, end_time, total_cost, status, created_at)
                                   VALUES (@vehicle_id, @user_id, @start_time, @end_time, @total_cost, 'active', NOW())";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@vehicle_id", vehicleId);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.Parameters.AddWithValue("@start_time", startTime);
                    cmd.Parameters.AddWithValue("@end_time", endTime);
                    cmd.Parameters.AddWithValue("@total_cost", totalCost);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        string updateVehicleSql = "UPDATE vehicles SET is_available = FALSE WHERE id = @vehicleId";
                        MySqlCommand updateCmd = new MySqlCommand(updateVehicleSql, conn);
                        updateCmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                        updateCmd.ExecuteNonQuery();
                        return true;
                    }
                    return false;
                }
            }
        }

        public static List<Order> GetUserOrders(int userId)
        {
            var orders = new List<Order>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT o.*, v.make AS VehicleMake, v.number AS VehicleNumber
                    FROM orders o
                    JOIN vehicles v ON o.vehicle_id = v.id
                    WHERE o.user_id = @userId
                    ORDER BY o.start_time DESC";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            Id = reader.GetInt32("id"),
                            VehicleId = reader.GetInt32("vehicle_id"),
                            UserId = reader.GetInt32("user_id"),
                            StartTime = reader.GetDateTime("start_time"),
                            EndTime = reader.GetDateTime("end_time"),
                            TotalCost = reader.GetDecimal("total_cost"),
                            Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), reader.GetString("status")),
                            CreatedAt = reader.GetDateTime("created_at"),
                            VehicleMake = reader.GetString("VehicleMake"),
                            VehicleNumber = reader.GetString("VehicleNumber")
                        });
                    }
                }
            }
            return orders;
        }

        public static bool CompleteOrder(int orderId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE orders SET status = 'completed' WHERE id = @orderId AND status = 'active'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    string getVehicleSql = "SELECT vehicle_id FROM orders WHERE id = @orderId";
                    MySqlCommand getCmd = new MySqlCommand(getVehicleSql, conn);
                    getCmd.Parameters.AddWithValue("@orderId", orderId);
                    int vehicleId = Convert.ToInt32(getCmd.ExecuteScalar());

                    string updateVehicleSql = "UPDATE vehicles SET is_available = TRUE WHERE id = @vehicleId";
                    MySqlCommand updateCmd = new MySqlCommand(updateVehicleSql, conn);
                    updateCmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                    updateCmd.ExecuteNonQuery();
                    return true;
                }
                return false;
            }
        }

        public static bool CancelOrder(int orderId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE orders SET status = 'cancelled' WHERE id = @orderId AND status = 'active'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    string getVehicleSql = "SELECT vehicle_id FROM orders WHERE id = @orderId";
                    MySqlCommand getCmd = new MySqlCommand(getVehicleSql, conn);
                    getCmd.Parameters.AddWithValue("@orderId", orderId);
                    int vehicleId = Convert.ToInt32(getCmd.ExecuteScalar());

                    string updateVehicleSql = "UPDATE vehicles SET is_available = TRUE WHERE id = @vehicleId";
                    MySqlCommand updateCmd = new MySqlCommand(updateVehicleSql, conn);
                    updateCmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                    updateCmd.ExecuteNonQuery();
                    return true;
                }
                return false;
            }
        }
    }
}