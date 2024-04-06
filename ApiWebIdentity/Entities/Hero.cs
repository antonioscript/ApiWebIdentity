using System.ComponentModel.DataAnnotations;

namespace ApiWebIdentity.Entities
{
    public class Hero
    {
        public int Id { get; set; }
        [Required, MaxLength(80)]
        public string? CivilName { get; set; }
        
        [Required, MaxLength(80)]
        public string? HeroName { get; set; }

        [EmailAddress]
        [Required, MaxLength(120)]
        public string? Email { get; set; }

        public string? Team { get; set; }
        public int Age { get; set; }
        public string? City { get; set; }
    }
}
