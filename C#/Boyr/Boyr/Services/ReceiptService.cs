using Boyr.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Boyr.Services
{
    public static class ReceiptService
    {
        public static void SaveReceiptToFile(Sale sale, List<SaleItem> items)
        {
            try
            {
                string fileName = $"Чек_{sale.Id}_{DateTime.Now:yyyyMMddHHmmss}.txt";
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fullPath = Path.Combine(desktopPath, fileName);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=========================================");
                sb.AppendLine("         Boyr - ЧЕК");
                sb.AppendLine("=========================================");
                sb.AppendLine($"Заказ №: {sale.Id}");
                sb.AppendLine($"Дата: {sale.SaleDate:dd.MM.yyyy HH:mm}");
                sb.AppendLine($"Покупатель: {sale.CustomerName}");
                sb.AppendLine();
                sb.AppendLine("-----------------------------------------");
                sb.AppendLine("Изделия:");
                sb.AppendLine("-----------------------------------------");

                foreach (var item in items)
                {
                    sb.AppendLine($"{item.Product.Name}");
                    sb.AppendLine($"   Металл: {item.Product.Metal} {item.Product.Purity} проба");
                    sb.AppendLine($"   Вес: {item.Product.Weight} г, Камень: {item.Product.Gemstone} {item.Product.GemCharacteristics}");
                    sb.AppendLine($"   Количество: {item.Quantity} × {item.PriceAtMoment:C} = {item.TotalPrice:C}");
                    sb.AppendLine();
                }

                sb.AppendLine("-----------------------------------------");
                sb.AppendLine($"ИТОГО: {sale.TotalAmount:C}");
                sb.AppendLine();
                sb.AppendLine("=========================================");
                sb.AppendLine("Благодарим за покупку!");
                sb.AppendLine("Boyr - Ваш идеальный блеск.");

                File.WriteAllText(fullPath, sb.ToString(), Encoding.UTF8);

                MessageBox.Show($"Чек сохранён на рабочем столе:\n{fileName}", "Чек сохранён",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить чек: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}