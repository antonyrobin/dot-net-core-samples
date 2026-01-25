using WebApp.Models;
using WebApp.Repositories.Interfaces;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Student> GetStudents()
        {
            return _repository.GetAll();
        }

        public Student GetStudent(int id)
        {
            return _repository.GetById(id);
        }

        public void Create(Student student)
        {
            _repository.Add(student);
            _repository.Save();
        }

        public void Edit(Student student)
        {
            _repository.Update(student);
            _repository.Save();
        }

        public void Remove(int id)
        {
            _repository.Delete(id);
            _repository.Save();
        }
    }
}
