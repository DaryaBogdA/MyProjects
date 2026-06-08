using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using TattooYou.Database;

namespace TattooYou.Helpers
{
    public class RevenueSummary
    {
        public decimal TotalRevenue { get; set; }
        public int AppointmentCount { get; set; }
        public decimal AverageCheck => AppointmentCount > 0 ? TotalRevenue / AppointmentCount : 0;
    }

    public static class StatisticsHelper
    {
        private const string RevenueBase = @"
            FROM appointments a
            JOIN services s ON a.service_id = s.id
            WHERE a.status <> 'cancelled'";

        public static RevenueSummary GetSummaryForPeriod(DateTime? from, DateTime? to)
        {
            var summary = new RevenueSummary();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var conditions = new List<string>();
                var cmd = new MySqlCommand { Connection = conn };

                if (from.HasValue)
                {
                    conditions.Add("a.appointment_date >= @from");
                    cmd.Parameters.AddWithValue("@from", from.Value.Date);
                }
                if (to.HasValue)
                {
                    conditions.Add("a.appointment_date <= @to");
                    cmd.Parameters.AddWithValue("@to", to.Value.Date);
                }

                string whereExtra = conditions.Count > 0 ? " AND " + string.Join(" AND ", conditions) : "";
                cmd.CommandText = $@"
                    SELECT COALESCE(SUM(s.price), 0), COUNT(*)
                    {RevenueBase}{whereExtra}";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        summary.TotalRevenue = reader.GetDecimal(0);
                        summary.AppointmentCount = reader.GetInt32(1);
                    }
                }
            }
            return summary;
        }

        public static DataTable GetMonthlyStatsForYear(int year)
        {
            var dt = new DataTable();
            dt.Columns.Add("Месяц", typeof(string));
            dt.Columns.Add("Записей", typeof(int));
            dt.Columns.Add("Выручка (BYN)", typeof(decimal));

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = $@"
                    SELECT MONTH(a.appointment_date) AS m,
                           COUNT(*) AS cnt,
                           COALESCE(SUM(s.price), 0) AS rev
                    {RevenueBase}
                      AND YEAR(a.appointment_date) = @year
                    GROUP BY MONTH(a.appointment_date)
                    ORDER BY m";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@year", year);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int month = reader.GetInt32("m");
                        dt.Rows.Add(
                            new DateTime(year, month, 1).ToString("MMMM"),
                            reader.GetInt32("cnt"),
                            reader.GetDecimal("rev"));
                    }
                }
            }
            return dt;
        }

        public static DataTable GetYearlyStats()
        {
            var dt = new DataTable();
            dt.Columns.Add("Год", typeof(int));
            dt.Columns.Add("Записей", typeof(int));
            dt.Columns.Add("Выручка (BYN)", typeof(decimal));

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = $@"
                    SELECT YEAR(a.appointment_date) AS y,
                           COUNT(*) AS cnt,
                           COALESCE(SUM(s.price), 0) AS rev
                    {RevenueBase}
                    GROUP BY YEAR(a.appointment_date)
                    ORDER BY y DESC";
                using (var adapter = new MySqlDataAdapter(query, conn))
                {
                    var raw = new DataTable();
                    adapter.Fill(raw);
                    foreach (DataRow row in raw.Rows)
                    {
                        dt.Rows.Add(
                            Convert.ToInt32(row["y"]),
                            Convert.ToInt32(row["cnt"]),
                            Convert.ToDecimal(row["rev"]));
                    }
                }
            }
            return dt;
        }

        public static List<int> GetAvailableYears()
        {
            var years = new List<int> { DateTime.Now.Year };
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = $@"
                    SELECT DISTINCT YEAR(a.appointment_date) AS y
                    {RevenueBase}
                    ORDER BY y DESC";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    years.Clear();
                    while (reader.Read())
                        years.Add(reader.GetInt32("y"));
                }
            }
            if (years.Count == 0)
                years.Add(DateTime.Now.Year);
            return years;
        }

        public static Dictionary<int, decimal> GetMonthlyRevenueMap(int year)
        {
            var map = new Dictionary<int, decimal>();
            for (int m = 1; m <= 12; m++)
                map[m] = 0;

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = $@"
                    SELECT MONTH(a.appointment_date) AS m,
                           COALESCE(SUM(s.price), 0) AS rev
                    {RevenueBase}
                      AND YEAR(a.appointment_date) = @year
                    GROUP BY MONTH(a.appointment_date)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@year", year);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        map[reader.GetInt32("m")] = reader.GetDecimal("rev");
                }
            }
            return map;
        }
    }
}
