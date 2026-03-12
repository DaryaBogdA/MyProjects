using MySql.Data.MySqlClient;
using shop.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace shop.DataAccess
{
    public class SaleRepository
    {
        public List<Sale> GetAll()
        {
            var list = new List<Sale>();
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM sales ORDER BY sale_date DESC");
            foreach (DataRow row in dt.Rows)
                list.Add(MapSale(row));
            return list;
        }

        public List<Sale> GetByUser(int userId)
        {
            string query = "SELECT * FROM sales WHERE user_id = @userId ORDER BY sale_date DESC";
            var param = new MySqlParameter("@userId", userId);
            DataTable dt = DatabaseHelper.ExecuteQuery(query, param);
            var list = new List<Sale>();
            foreach (DataRow row in dt.Rows)
                list.Add(MapSale(row));
            return list;
        }

        public Sale GetById(int id)
        {
            string query = "SELECT * FROM sales WHERE id = @id";
            var param = new MySqlParameter("@id", id);
            DataTable dt = DatabaseHelper.ExecuteQuery(query, param);
            if (dt.Rows.Count == 0) return null;
            return MapSale(dt.Rows[0]);
        }

        private Sale MapSale(DataRow row)
        {
            return new Sale
            {
                Id = Convert.ToInt32(row["id"]),
                UserId = Convert.ToInt32(row["user_id"]),
                TotalAmount = Convert.ToDecimal(row["total_amount"]),
                SaleDate = Convert.ToDateTime(row["sale_date"]),
                CustomerName = row["customer_name"]?.ToString()
            };
        }
    }
}