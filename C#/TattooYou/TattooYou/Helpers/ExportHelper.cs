using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace TattooYou.Helpers
{
    public static class ExportHelper
    {
        public static void DataTableToCsv(DataTable dt, string filePath, char separator = '\t')
        {
            var sb = new StringBuilder();

            string[] columnNames = dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
            sb.AppendLine(string.Join(separator.ToString(), columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field?.ToString() ?? "").ToArray();
                sb.AppendLine(string.Join(separator.ToString(), fields));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}