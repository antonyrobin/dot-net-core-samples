using WebApp.Models;

namespace WebApp.Data
{
    public class StudentStore
    {
        public static List<Student> Students { get; } = new()
        {
            new Student { Id = 1, Name = "Antony", Course = "DotNet", Age = 22 },
            new Student { Id = 2, Name = "Robin", Course = "Java", Age = 24 }
        };
    }
}
