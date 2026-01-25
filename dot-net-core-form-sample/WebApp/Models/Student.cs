using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Course { get; set; }

        [Range(1, 120)]
        public int Age { get; set; }
    }
}
