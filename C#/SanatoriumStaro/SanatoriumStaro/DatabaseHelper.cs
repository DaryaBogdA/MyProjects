using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using SanatoriumStaro.Models;

namespace SanatoriumStaro
{
    public static class DatabaseHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        public static User ValidateUser(string login, string password)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT id, login, password_hash, full_name, phone, email, role, created_at 
                                 FROM users WHERE login = @login";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@login", login);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedHash = reader["password_hash"].ToString();
                        string inputHash = HashPassword(password);
                        if (storedHash == inputHash)
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Login = reader["login"].ToString(),
                                FullName = reader["full_name"].ToString(),
                                Phone = reader["phone"]?.ToString(),
                                Email = reader["email"]?.ToString(),
                                Role = reader["role"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["created_at"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static bool RegisterUser(string login, string password, string fullName, string phone, string email, string role = "user")
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO users (login, password_hash, full_name, phone, email, role) 
                                 VALUES (@login, @password_hash, @full_name, @phone, @email, @role)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@password_hash", HashPassword(password));
                cmd.Parameters.AddWithValue("@full_name", fullName);
                cmd.Parameters.AddWithValue("@phone", phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@role", role);
                try
                {
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (MySqlException ex) when (ex.Number == 1062)
                {
                    return false;
                }
            }
        }

        public static User GetUserById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT id, login, full_name, phone, email, role, created_at FROM users WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Login = reader["login"].ToString(),
                            FullName = reader["full_name"].ToString(),
                            Phone = reader["phone"]?.ToString(),
                            Email = reader["email"]?.ToString(),
                            Role = reader["role"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        };
                    }
                }
            }
            return null;
        }

        public static bool UpdateUser(User user)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE users SET full_name = @full_name, phone = @phone, email = @email WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@full_name", user.FullName);
                cmd.Parameters.AddWithValue("@phone", user.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@id", user.Id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static List<Service> GetAllServices()
        {
            var list = new List<Service>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT s.*, sc.name as category_name 
                                 FROM services s
                                 LEFT JOIN service_categories sc ON s.category_id = sc.id
                                 WHERE s.is_active = 1
                                 ORDER BY s.name";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Service
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"]?.ToString(),
                            Price = Convert.ToDecimal(reader["price"]),
                            Duration = reader["duration"] as int?,
                            CategoryId = reader["category_id"] as int?,
                            CategoryName = reader["category_name"]?.ToString(),
                            IsActive = Convert.ToBoolean(reader["is_active"]),
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        });
                    }
                }
            }
            return list;
        }

        public static Service GetServiceById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM services WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Service
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"]?.ToString(),
                            Price = Convert.ToDecimal(reader["price"]),
                            Duration = reader["duration"] as int?,
                            CategoryId = reader["category_id"] as int?,
                            IsActive = Convert.ToBoolean(reader["is_active"]),
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        };
                    }
                }
            }
            return null;
        }

        public static bool AddService(Service service)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO services (name, description, price, duration, category_id, is_active) 
                                 VALUES (@name, @description, @price, @duration, @category_id, @is_active)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", service.Name);
                cmd.Parameters.AddWithValue("@description", service.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@price", service.Price);
                cmd.Parameters.AddWithValue("@duration", service.Duration ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@category_id", service.CategoryId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@is_active", service.IsActive);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateService(Service service)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE services SET name=@name, description=@description, price=@price, 
                                 duration=@duration, category_id=@category_id, is_active=@is_active WHERE id=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", service.Name);
                cmd.Parameters.AddWithValue("@description", service.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@price", service.Price);
                cmd.Parameters.AddWithValue("@duration", service.Duration ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@category_id", service.CategoryId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@is_active", service.IsActive);
                cmd.Parameters.AddWithValue("@id", service.Id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteService(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE services SET is_active = 0 WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static List<ServiceCategory> GetAllCategories()
        {
            var list = new List<ServiceCategory>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM service_categories ORDER BY name";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ServiceCategory
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString()
                        });
                    }
                }
            }
            return list;
        }
        public static bool CreateAppointment(int userId, int serviceId, DateTime appointmentDate, string notes)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO appointments (user_id, service_id, appointment_date, notes) 
                                 VALUES (@user_id, @service_id, @appointment_date, @notes)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.Parameters.AddWithValue("@service_id", serviceId);
                cmd.Parameters.AddWithValue("@appointment_date", appointmentDate);
                cmd.Parameters.AddWithValue("@notes", notes ?? (object)DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<Appointment> GetAppointmentsByUser(int userId)
        {
            var list = new List<Appointment>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT a.*, u.full_name as user_name, s.name as service_name, st.name as status_name
                                 FROM appointments a
                                 JOIN users u ON a.user_id = u.id
                                 JOIN services s ON a.service_id = s.id
                                 JOIN appointment_statuses st ON a.status_id = st.id
                                 WHERE a.user_id = @user_id
                                 ORDER BY a.appointment_date DESC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user_id", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Appointment
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            UserFullName = reader["user_name"].ToString(),
                            ServiceId = Convert.ToInt32(reader["service_id"]),
                            ServiceName = reader["service_name"].ToString(),
                            AppointmentDate = Convert.ToDateTime(reader["appointment_date"]),
                            StatusId = Convert.ToInt32(reader["status_id"]),
                            StatusName = reader["status_name"].ToString(),
                            Notes = reader["notes"]?.ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        });
                    }
                }
            }
            return list;
        }

        public static List<Appointment> GetAllAppointments()
        {
            var list = new List<Appointment>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT a.*, u.full_name as user_name, s.name as service_name, st.name as status_name
                                 FROM appointments a
                                 JOIN users u ON a.user_id = u.id
                                 JOIN services s ON a.service_id = s.id
                                 JOIN appointment_statuses st ON a.status_id = st.id
                                 ORDER BY a.appointment_date DESC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Appointment
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            UserFullName = reader["user_name"].ToString(),
                            ServiceId = Convert.ToInt32(reader["service_id"]),
                            ServiceName = reader["service_name"].ToString(),
                            AppointmentDate = Convert.ToDateTime(reader["appointment_date"]),
                            StatusId = Convert.ToInt32(reader["status_id"]),
                            StatusName = reader["status_name"].ToString(),
                            Notes = reader["notes"]?.ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        });
                    }
                }
            }
            return list;
        }

        public static bool UpdateAppointmentStatus(int appointmentId, int statusId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE appointments SET status_id = @status_id WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@status_id", statusId);
                cmd.Parameters.AddWithValue("@id", appointmentId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool CancelAppointment(int appointmentId)
        {
            return UpdateAppointmentStatus(appointmentId, 3);
        }
        public static List<AppointmentStatus> GetAllStatuses()
        {
            var list = new List<AppointmentStatus>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM appointment_statuses ORDER BY id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AppointmentStatus
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}