using MySql.Data.MySqlClient;
using shop.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace shop.DataAccess
{
    public class UserRepository
    {
        public User GetByLogin(string login)
        {
            string query = "SELECT * FROM users WHERE login = @login";
            var param = new MySqlParameter("@login", login);
            DataTable dt = DatabaseHelper.ExecuteQuery(query, param);
            if (dt.Rows.Count == 0) return null;
            return MapUser(dt.Rows[0]);
        }

        public List<User> GetAll()
        {
            var users = new List<User>();
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT id, login, password, role FROM users");
            foreach (DataRow row in dt.Rows)
                users.Add(MapUser(row));
            return users;
        }

        public User GetById(int id)
        {
            string query = "SELECT * FROM users WHERE id = @id";
            var param = new MySqlParameter("@id", id);
            DataTable dt = DatabaseHelper.ExecuteQuery(query, param);
            if (dt.Rows.Count == 0) return null;
            return MapUser(dt.Rows[0]);
        }

        public void Add(User user)
        {
            string query = "INSERT INTO users (login, password, role) VALUES (@login, @password, @role)";
            DatabaseHelper.ExecuteNonQuery(query,
                new MySqlParameter("@login", user.Login),
                new MySqlParameter("@password", user.Password),
                new MySqlParameter("@role", user.Role.ToString()));
        }

        public void Update(User user)
        {
            string query = "UPDATE users SET login=@login, password=@password, role=@role WHERE id=@id";
            DatabaseHelper.ExecuteNonQuery(query,
                new MySqlParameter("@id", user.Id),
                new MySqlParameter("@login", user.Login),
                new MySqlParameter("@password", user.Password),
                new MySqlParameter("@role", user.Role.ToString()));
        }

        public void Delete(int id)
        {
            string query = "DELETE FROM users WHERE id=@id";
            DatabaseHelper.ExecuteNonQuery(query, new MySqlParameter("@id", id));
        }

        private User MapUser(DataRow row)
        {
            return new User
            {
                Id = Convert.ToInt32(row["id"]),
                Login = row["login"].ToString(),
                Password = row["password"].ToString(),
                Role = row["role"].ToString() == "admin" ? UserRole.admin : UserRole.user
            };
        }
    }
}