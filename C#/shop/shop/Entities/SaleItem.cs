namespace shop.Entities
{
    public class SaleItem
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtMoment { get; set; }
        public Sale Sale { get; set; }
        public Product Product { get; set; }
        public decimal TotalPrice => Quantity * PriceAtMoment;
    }
}