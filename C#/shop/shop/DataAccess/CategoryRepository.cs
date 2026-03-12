using MySql.Data.MySqlClient;
using shop.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace shop.DataAccess
{
    public class CategoryRepository
    {
        // Добавляем правильный метод GetAll для категорий
        public List<Category> GetAll()
        {
            var list = new List<Category>();
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM categories ORDER BY name");
            foreach (DataRow row in dt.Rows)
                list.Add(MapCategory(row));
            return list;
        }

        public Category GetById(int id)
        {
            string query = "SELECT * FROM categories WHERE id = @id";
            var param = new MySqlParameter("@id", id);
            DataTable dt = DatabaseHelper.ExecuteQuery(query, param);
            if (dt.Rows.Count == 0) return null;
            return MapCategory(dt.Rows[0]);
        }

        public void Add(Category category)
        {
            string query = "INSERT INTO categories (name) VALUES (@name)";
            DatabaseHelper.ExecuteNonQuery(query, new MySqlParameter("@name", category.Name));
        }

        public void Update(Category category)
        {
            string query = "UPDATE categories SET name=@name WHERE id=@id";
            DatabaseHelper.ExecuteNonQuery(query,
                new MySqlParameter("@id", category.Id),
                new MySqlParameter("@name", category.Name));
        }

        public void Delete(int id)
        {
            string query = "DELETE FROM categories WHERE id=@id";
            DatabaseHelper.ExecuteNonQuery(query, new MySqlParameter("@id", id));
        }

        private Category MapCategory(DataRow row)
        {
            return new Category
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"].ToString()
            };
        }
    }
}