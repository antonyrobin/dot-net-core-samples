using WebApp.Models;

namespace WebApp.Services.Interfaces
{
    public interface IStudentService
    {
        IEnumerable<Student> GetStudents();
        Student GetStudent(int id);
        void Create(Student student);
        void Edit(Student student);
        void Remove(int id);
    }
}
