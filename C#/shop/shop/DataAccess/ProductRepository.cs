using MySql.Data.MySqlClient;
using shop.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace shop.DataAccess
{
    public class ProductRepository
    {
        public List<Product> GetAll()
        {
            var products = new List<Product>();

            string query = @"
        SELECT p.id, p.name, p.category_id, c.name as category_name, 
               p.price, p.size, p.color, p.quantity, p.image_url 
        FROM products p 
        LEFT JOIN categories c ON p.category_id = c.id";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString(),
                    CategoryId = Convert.ToInt32(row["category_id"]),
                    CategoryName = row["category_name"].ToString(),
                    Price = Convert.ToDecimal(row["price"]),
                    Size = row["size"].ToString(),
                    Color = row["color"].ToString(),
                    Quantity = Convert.ToInt32(row["quantity"]),
                    ImageUrl = row.Table.Columns.Contains("image_url") && row["image_url"] != DBNull.Value
                                ? row["image_url"].ToString()
                                : null
                });
            }

            return products;
        }

        public Product GetById(int id)
        {
            string query = "SELECT * FROM products WHERE id = @id";
            var param = new MySqlParameter("@id", id);
            DataTable dt = DatabaseHelper.ExecuteQuery(query, param);
            if (dt.Rows.Count == 0) return null;
            return MapProduct(dt.Rows[0]);
        }

        public void Add(Product product)
        {
            string query = @"INSERT INTO products (name, category_id, price, size, color, quantity) 
                             VALUES (@name, @categoryId, @price, @size, @color, @quantity)";
            var parameters = new[]
            {
                new MySqlParameter("@name", product.Name),
                new MySqlParameter("@categoryId", product.CategoryId),
                new MySqlParameter("@price", product.Price),
                new MySqlParameter("@size", product.Size ?? ""),
                new MySqlParameter("@color", product.Color ?? ""),
                new MySqlParameter("@quantity", product.Quantity)
            };
            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        public void Update(Product product)
        {
            string query = @"UPDATE products SET name=@name, category_id=@categoryId, price=@price, 
                             size=@size, color=@color, quantity=@quantity WHERE id=@id";
            var parameters = new[]
            {
                new MySqlParameter("@id", product.Id),
                new MySqlParameter("@name", product.Name),
                new MySqlParameter("@categoryId", product.CategoryId),
                new MySqlParameter("@price", product.Price),
                new MySqlParameter("@size", product.Size ?? ""),
                new MySqlParameter("@color", product.Color ?? ""),
                new MySqlParameter("@quantity", product.Quantity)
            };
            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        public void Delete(int id)
        {
            string query = "DELETE FROM products WHERE id = @id";
            var param = new MySqlParameter("@id", id);
            DatabaseHelper.ExecuteNonQuery(query, param);
        }

        private Product MapProduct(DataRow row)
        {
            return new Product
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"].ToString(),
                CategoryId = Convert.ToInt32(row["category_id"]),
                Price = Convert.ToDecimal(row["price"]),
                Size = row["size"].ToString(),
                Color = row["color"].ToString(),
                Quantity = Convert.ToInt32(row["quantity"]),
                ImageUrl = row.Table.Columns.Contains("image_url") && row["image_url"] != DBNull.Value
                            ? row["image_url"].ToString()
                            : null
            };
        }
    }
}