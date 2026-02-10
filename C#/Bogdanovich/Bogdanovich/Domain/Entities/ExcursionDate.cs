namespace Bogdanovich.Domain.Entities
{
    public class ExcursionDate : Entity
    {
        public int ExcursionId { get; set; }
        public DateOnly? AvailableDate { get; set; }

        public virtual Excursion? Excursion { get; set; }
    }
}
