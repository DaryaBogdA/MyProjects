using System.Collections.Generic;

namespace TattooYou.Models
{
    public class Master
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsActive { get; set; }
        public List<Style> Styles { get; set; } = new List<Style>();
    }
}