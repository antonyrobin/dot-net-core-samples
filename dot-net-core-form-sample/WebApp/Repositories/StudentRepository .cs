using WebApp.Models;
using WebApp.Repositories.Interfaces;
using WebApp.Data;

namespace WebApp.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        public IEnumerable<Student> GetAll()
        {
            return StudentStore.Students;
        }

        public Student GetById(int id)
        {
            return StudentStore.Students.FirstOrDefault(s => s.Id == id);
        }

        public void Add(Student student)
        {
            student.Id = StudentStore.Students.Max(s => s.Id) + 1;
            StudentStore.Students.Add(student);
        }

        public void Update(Student student)
        {
            var existing = GetById(student.Id);
            if (existing == null) return;

            existing.Name = student.Name;
            existing.Course = student.Course;
            existing.Age = student.Age;
        }

        public void Delete(int id)
        {
            var student = GetById(id);
            if (student != null)
                StudentStore.Students.Remove(student);
        }

        public void Save()
        {
            
        }
    }
}
