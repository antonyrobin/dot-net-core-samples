# 📘 Student CRUD – ASP.NET Core MVC (In‑Memory List)

This project demonstrates a **complete Student CRUD (Create, Read, Update, Delete)** application using **ASP.NET Core MVC** with a **local in‑memory List** instead of a database.

✅ No SQL Server required  
✅ No Entity Framework required  
✅ Clean architecture with **SOLID principles**  
✅ Beginner & interview friendly

---

## 🧱 Architecture Overview (SOLID)

```
StudentManagement
│
├── Controllers
│   └── StudentController.cs
│
├── Models
│   └── Student.cs
│
├── Data
│   └── StudentStore.cs        // In‑memory list
│
├── Repositories
│   ├── Interfaces
│   │   └── IStudentRepository.cs
│   └── StudentRepository.cs
│
├── Services
│   ├── Interfaces
│   │   └── IStudentService.cs
│   └── StudentService.cs
│
├── Views
│   └── Student
│       ├── Index.cshtml
│       ├── Create.cshtml
│       ├── Edit.cshtml
│       ├── Delete.cshtml
│       └── Details.cshtml
│
└── Program.cs
```

---

## 🧠 SOLID Principles Used

| Principle | How it’s applied |
|---------|------------------|
| **S** – Single Responsibility | Controller, Service, Repository all have one job |
| **O** – Open/Closed | Can switch List → DB without changing controller |
| **L** – Liskov | Interfaces allow swapping implementations |
| **I** – Interface Segregation | Small focused interfaces |
| **D** – Dependency Inversion | High‑level modules depend on interfaces |

---

## 1️⃣ Prerequisites

- .NET SDK **8 / 9 / 10** (any recent version)
- Visual Studio 2022 / VS Code
- Basic C# knowledge

Check SDK:
```bash
dotnet --version
```

---

## 2️⃣ Create the Project

```bash
dotnet new mvc -n StudentManagement
cd StudentManagement
```

Run once to verify:
```bash
dotnet run
```

---

## 3️⃣ Dependencies Required

👉 **No external NuGet packages required**

The project uses only:
- ASP.NET Core MVC (built‑in)
- Dependency Injection (built‑in)
- Razor Views (cshtml)

---

## 4️⃣ Model – Student

📁 `Models/Student.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Course { get; set; }

        [Range(1, 120)]
        public int Age { get; set; }
    }
}
```

---

## 5️⃣ In‑Memory Data Store

📁 `Data/StudentStore.cs`

```csharp
using StudentManagement.Models;

namespace StudentManagement.Data
{
    public static class StudentStore
    {
        public static List<Student> Students { get; } = new()
        {
            new Student { Id = 1, Name = "Antony", Course = "DotNet", Age = 22 },
            new Student { Id = 2, Name = "Robin", Course = "Java", Age = 24 }
        };
    }
}
```

⚠️ Data resets when application restarts.

---

## 6️⃣ Repository Layer

### Interface
📁 `Repositories/Interfaces/IStudentRepository.cs`

```csharp
public interface IStudentRepository
{
    IEnumerable<Student> GetAll();
    Student GetById(int id);
    void Add(Student student);
    void Update(Student student);
    void Delete(int id);
}
```

### Implementation
📁 `Repositories/StudentRepository.cs`

```csharp
public class StudentRepository : IStudentRepository
{
    public IEnumerable<Student> GetAll() => StudentStore.Students;

    public Student GetById(int id) =>
        StudentStore.Students.FirstOrDefault(s => s.Id == id);

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
}
```

---

## 7️⃣ Service Layer

📁 `Services/Interfaces/IStudentService.cs`

```csharp
public interface IStudentService
{
    IEnumerable<Student> GetStudents();
    Student GetStudent(int id);
    void Create(Student student);
    void Edit(Student student);
    void Remove(int id);
}
```

📁 `Services/StudentService.cs`

```csharp
public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Student> GetStudents() => _repository.GetAll();

    public Student GetStudent(int id) => _repository.GetById(id);

    public void Create(Student student) => _repository.Add(student);

    public void Edit(Student student) => _repository.Update(student);

    public void Remove(int id) => _repository.Delete(id);
}
```

---

## 8️⃣ Controller

📁 `Controllers/StudentController.cs`

```csharp
public class StudentController : Controller
{
    private readonly IStudentService _service;

    public StudentController(IStudentService service)
    {
        _service = service;
    }

    public IActionResult Index() => View(_service.GetStudents());

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Student student)
    {
        if (!ModelState.IsValid) return View(student);
        _service.Create(student);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id) => View(_service.GetStudent(id));

    [HttpPost]
    public IActionResult Edit(Student student)
    {
        if (!ModelState.IsValid) return View(student);
        _service.Edit(student);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id) => View(_service.GetStudent(id));

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        _service.Remove(id);
        return RedirectToAction(nameof(Index));
    }
}
```

---

## 9️⃣ Views (Forms)

### Index.cshtml
Displays all students with Edit/Delete links.

### Create.cshtml
Used to create a student using an HTML form.

### Edit.cshtml
Same as Create but includes hidden `Id` field.

### Delete.cshtml
Confirmation page before deleting.

📌 **Important:** View file names must match action names.

---

## 🔟 Program.cs (Dependency Injection)

```csharp
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IStudentRepository, StudentRepository>();
builder.Services.AddSingleton<IStudentService, StudentService>();
```

🔑 `Singleton` keeps the in‑memory list alive while the app runs.

---

## ▶️ Run the Application

```bash
dotnet run
```

Open browser:
```
https://localhost:5001
```

---

## ▶️ Run on a Different Port

### Option 1: Command line
```bash
dotnet run --urls="http://localhost:8080"
```

### Option 2: launchSettings.json
```json
"applicationUrl": "http://localhost:8080"
```

---

## 🧪 Common Errors & Fixes

| Error | Fix |
|-----|-----|
| Edit.cshtml not found | Ensure file is in `Views/Student` |
| Data resets | Expected (in‑memory list) |
| Null model | Ensure correct `asp-route-id` |

---

## 🚀 Next Improvements

- Replace List with **EF Core InMemory**
- Convert to **Web API**
- Add **Bootstrap UI**
- Add **Unit Tests (xUnit)**
- Add **Search & Pagination**

---

## ✅ Summary

This project teaches:
- ASP.NET Core MVC fundamentals
- CRUD operations
- SOLID architecture
- Dependency Injection
- Razor Forms

Perfect for **learning, interviews, and practice projects** 🎯

