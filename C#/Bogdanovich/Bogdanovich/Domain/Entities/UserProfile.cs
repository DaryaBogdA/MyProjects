namespace Bogdanovich.Domain.Entities
{
    public class UserProfile : Entity
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SurName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? PassportNumber { get; set; }
        public DateOnly? PassportDate { get; set; }
        public string? Phone { get; set; }
        public string? Preferences { get; set; }
        public bool NotificationEmail { get; set; } = true;

        public virtual User? User { get; set; }
    }
}
