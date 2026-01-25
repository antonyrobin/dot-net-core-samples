using Microsoft.AspNetCore.Mvc;
using WebApp.Services.Interfaces;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View(_service.GetStudents());
        }

        public IActionResult Details(int id)
        {
            return View(_service.GetStudent(id));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid) return View(student);

            _service.Create(student);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            return View(_service.GetStudent(id));
        }

        [HttpPost]
        public IActionResult Edit(Student student)
        {
            if (!ModelState.IsValid) return View(student);

            _service.Edit(student);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            return View(_service.GetStudent(id));
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _service.Remove(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
