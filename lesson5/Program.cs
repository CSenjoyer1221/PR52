using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityManagementSystem
{

    public enum AcademicDegree
    {
        Bachelor,
        Master,
        Doctor,
        Professor
    }

    public enum CourseStatus
    {
        Active,
        Inactive,
        Completed
    }

    public abstract class Person
    {
        private string _firstName;
        private string _lastName;
        private int _age;
        private string _email;

        public string FirstName
        {
            get => _firstName;
            set => _firstName = !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new ArgumentException("Имя не может быть пустым");
        }

        public string LastName
        {
            get => _lastName;
            set => _lastName = !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new ArgumentException("Фамилия не может быть пустой");
        }

        public int Age
        {
            get => _age;
            set => _age = value >= 16 && value <= 100
                ? value
                : throw new ArgumentException("Возраст должен быть от 16 до 100 лет");
        }

        public string Email
        {
            get => _email;
            set => _email = IsValidEmail(value)
                ? value
                : throw new ArgumentException("Некорректный email адрес");
        }

        public int Id { get; }
        public DateTime RegistrationDate { get; }

        protected Person(int id, string firstName, string lastName, int age, string email)
        {
            Id = id;
            RegistrationDate = DateTime.Now;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Email = email;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return email.Contains("@") && email.Contains(".") && email.Length > 5;
        }

        public virtual string GetFullInfo()
        {
            return $"{FirstName} {LastName} | Возраст: {Age} | Email: {Email}";
        }
    }
    public class Student : Person
    {
        private string _studentId;
        private double _gpa;

        public string StudentId
        {
            get => _studentId;
            set => _studentId = !string.IsNullOrWhiteSpace(value) && value.Length >= 6
                ? value
                : throw new ArgumentException("ID студента должен содержать минимум 6 символов");
        }

        public double GPA
        {
            get => _gpa;
            set => _gpa = value >= 0 && value <= 4.0
                ? value
                : throw new ArgumentException("GPA должен быть в диапазоне от 0.0 до 4.0");
        }

        public string Major { get; set; }
        public int YearOfStudy { get; set; }
        public List<Course> Courses { get; }

        public Student(int id, string firstName, string lastName, int age, string email,
                      string studentId, string major, int yearOfStudy, double gpa = 0.0)
            : base(id, firstName, lastName, age, email)
        {
            StudentId = studentId;
            Major = major;
            YearOfStudy = yearOfStudy >= 1 && yearOfStudy <= 6
                ? yearOfStudy
                : throw new ArgumentException("Год обучения должен быть от 1 до 6");
            GPA = gpa;
            Courses = new List<Course>();
        }

        public void EnrollInCourse(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course), "Курс не может быть null");

            if (Courses.Contains(course))
                throw new InvalidOperationException($"Студент уже записан на курс: {course.Name}");

            Courses.Add(course);
            course.AddStudent(this);
        }

        public override string GetFullInfo()
        {
            return base.GetFullInfo() +
                $" | ID: {StudentId} | Специальность: {Major} | Курс: {YearOfStudy} | GPA: {GPA:F2}";
        }

        public string GetCoursesInfo()
        {
            if (Courses.Count == 0)
                return "Студент не записан ни на один курс";

            return $"Курсы студента {FirstName} {LastName}:\n" +
                 string.Join("\n", Courses.Select(c => $"- {c.Name} ({c.Code})"));
        }
    }

    public class Professor : Person
    {
        private string _employeeId;
        private decimal _salary;

        public string EmployeeId
        {
            get => _employeeId;
            set => _employeeId = !string.IsNullOrWhiteSpace(value) && value.Length >= 6
                ? value
                : throw new ArgumentException("ID сотрудника должен содержать минимум 6 символов");
        }

        public decimal Salary
        {
            get => _salary;
            set => _salary = value > 0
                ? value
                : throw new ArgumentException("Зарплата должна быть положительной");
        }

        public AcademicDegree Degree { get; set; }
        public string Department { get; set; }
        public List<Course> TeachingCourses { get; }

        public Professor(int id, string firstName, string lastName, int age, string email,
                        string employeeId, string department, AcademicDegree degree, decimal salary)
            : base(id, firstName, lastName, age, email)
        {
            EmployeeId = employeeId;
            Department = department;
            Degree = degree;
            Salary = salary;
            TeachingCourses = new List<Course>();
        }

        public void AssignToCourse(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course), "Курс не может быть null");

            if (TeachingCourses.Contains(course))
                throw new InvalidOperationException($"Преподаватель уже ведет курс: {course.Name}");

            TeachingCourses.Add(course);
            course.AssignProfessor(this);
        }

        public override string GetFullInfo()
        {
            return base.GetFullInfo() +
                   $" | ID: {EmployeeId} | Кафедра: {Department} | Степень: {Degree} | Зарплата: {Salary:C}";
        }

        public string GetTeachingCoursesInfo()
        {
            if (TeachingCourses.Count == 0)
                return "Преподаватель не ведет ни одного курса";

            return $"Курсы преподавателя {FirstName} {LastName}:\n" +
                   string.Join("\n", TeachingCourses.Select(c => $"- {c.Name} ({c.Code})"));
        }
    }

    public class Course
    {
        private string _name;
        private string _code;
        private int _credits;
        private int _maxStudents;

        public string Name
        {
            get => _name;
            set => _name = !string.IsNullOrWhiteSpace(value) && value.Length >= 3
                ? value
                : throw new ArgumentException("Название курса должно содержать минимум 3 символа");
        }

        public string Code
        {
            get => _code;
            set => _code = !string.IsNullOrWhiteSpace(value) && value.Length >= 4
                ? value
                : throw new ArgumentException("Код курса должен содержать минимум 4 символа");
        }

        public int Credits
        {
            get => _credits;
            set => _credits = value > 0 && value <= 10
                ? value
                : throw new ArgumentException("Кредиты должны быть в диапазоне от 1 до 10");
        }

        public int MaxStudents
        {
            get => _maxStudents;
            set => _maxStudents = value > 0 && value <= 200
                ? value
                : throw new ArgumentException("Максимальное количество студентов должно быть от 1 до 200");
        }

        public string Description { get; set; }
        public CourseStatus Status { get; set; }
        public Professor Professor { get; private set; }
        public List<Student> EnrolledStudents { get; }

        public Course(string name, string code, int credits, int maxStudents, string description = "")
        {
            Name = name;
            Code = code;
            Credits = credits;
            MaxStudents = maxStudents;
            Description = string.IsNullOrWhiteSpace(description) ? "Описание отсутствует" : description;
            Status = CourseStatus.Active;
            EnrolledStudents = new List<Student>();
        }

        public void AssignProfessor(Professor professor)
        {
            Professor = professor ?? throw new ArgumentNullException(nameof(professor), "Преподаватель не может быть null");
        }

        public void AddStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student), "Студент не может быть null");

            if (EnrolledStudents.Contains(student))
                throw new InvalidOperationException($"Студент уже записан на курс: {Name}");

            if (EnrolledStudents.Count >= MaxStudents)
                throw new InvalidOperationException($"Курс {Name} заполнен. Максимум студентов: {MaxStudents}");

            EnrolledStudents.Add(student);
        }

        public string GetFullInfo()
        {
            string professorInfo = Professor != null
                ? $"{Professor.FirstName} {Professor.LastName}"
                : "Не назначен";

            return $"Курс: {Name} ({Code})\n" +
                   $"Описание: {Description}\n" +
                   $"Кредиты: {Credits} | Макс. студентов: {MaxStudents} | Записано: {EnrolledStudents.Count}\n" +
                   $"Статус: {Status} | Преподаватель: {professorInfo}";
        }

        public string GetEnrolledStudentsInfo()
        {
            if (EnrolledStudents.Count == 0)
                return "На курс еще никто не записан";

            return $"Студенты курса {Name}:\n" +
                   string.Join("\n", EnrolledStudents.Select(s => $"- {s.FirstName} {s.LastName} ({s.StudentId})"));
        }
    }

    public class UniversityManager
    {
        private List<Student> _students;
        private List<Professor> _professors;
        private List<Course> _courses;
        private int _nextStudentId;
        private int _nextProfessorId;

        public UniversityManager()
        {
            _students = new List<Student>();
            _professors = new List<Professor>();
            _courses = new List<Course>();
            _nextStudentId = 1;
            _nextProfessorId = 1;

            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            try
            {
                // Создаем тестовых преподавателей
                var prof1 = new Professor(_nextProfessorId++, "Иван", "Петров", 45,
                    "i.petrov@university.ru", "PROF001", "Компьютерные науки", AcademicDegree.Professor, 80000);
                var prof2 = new Professor(_nextProfessorId++, "Мария", "Сидорова", 38,
                    "m.sidorova@university.ru", "PROF002", "Математика", AcademicDegree.Doctor, 70000);

                _professors.Add(prof1);
                _professors.Add(prof2);

                // Создаем тестовых студентов
                var student1 = new Student(_nextStudentId++, "Алексей", "Иванов", 20,
                    "a.ivanov@university.ru", "STU001", "Компьютерные науки", 2, 3.8);
                var student2 = new Student(_nextStudentId++, "Елена", "Кузнецова", 19,
                    "e.kuznetsova@university.ru", "STU002", "Математика", 1, 3.9);
                var student3 = new Student(_nextStudentId++, "Дмитрий", "Смирнов", 21,
                    "d.smirnov@university.ru", "STU003", "Физика", 3, 3.5);

                _students.Add(student1);
                _students.Add(student2);
                _students.Add(student3);

                // Создаем тестовые курсы
                var course1 = new Course("Программирование на C#", "CS101", 4, 30,
                    "Основы программирования на языке C#");
                var course2 = new Course("Линейная алгебра", "MATH201", 3, 25,
                    "Основы линейной алгебры и матричных вычислений");
                var course3 = new Course("Общая физика", "PHYS101", 4, 35,
                    "Основы механики и термодинамики");

                _courses.Add(course1);
                _courses.Add(course2);
                _courses.Add(course3);

                // Назначаем преподавателей на курсы
                prof1.AssignToCourse(course1);
                prof2.AssignToCourse(course2);
                prof1.AssignToCourse(course3);

                // Записываем студентов на курсы
                student1.EnrollInCourse(course1);
                student1.EnrollInCourse(course2);
                student2.EnrollInCourse(course2);
                student3.EnrollInCourse(course1);
                student3.EnrollInCourse(course3);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации тестовых данных: {ex.Message}");
            }
        }

        public void AddStudent(string firstName, string lastName, int age, string email,
                             string studentId, string major, int yearOfStudy, double gpa = 0.0)
        {
            try
            {
                var student = new Student(_nextStudentId++, firstName, lastName, age, email,
                    studentId, major, yearOfStudy, gpa);
                _students.Add(student);
                Console.WriteLine($"✅ Студент успешно добавлен: {student.GetFullInfo()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при добавлении студента: {ex.Message}");
            }
        }

        public void DisplayAllStudents()
        {
            if (_students.Count == 0)
            {
                Console.WriteLine("📭 В системе нет студентов.");
                return;
            }

            Console.WriteLine("\n🎓 === ВСЕ СТУДЕНТЫ ===");
            foreach (var student in _students)
            {
                Console.WriteLine(student.GetFullInfo());
            }
            Console.WriteLine($"📊 Всего студентов: {_students.Count}");
        }

        public Student FindStudentById(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }

        public Student FindStudentByStudentId(string studentId)
        {
            return _students.FirstOrDefault(s => s.StudentId == studentId);
        }

        public void AddProfessor(string firstName, string lastName, int age, string email,
                               string employeeId, string department, AcademicDegree degree, decimal salary)
        {
            try
            {
                var professor = new Professor(_nextProfessorId++, firstName, lastName, age, email,
                    employeeId, department, degree, salary);
                _professors.Add(professor);
                Console.WriteLine($"✅ Преподаватель успешно добавлен: {professor.GetFullInfo()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при добавлении преподавателя: {ex.Message}");
            }
        }

        public void DisplayAllProfessors()
        {
            if (_professors.Count == 0)
            {
                Console.WriteLine("📭 В системе нет преподавателей.");
                return;
            }

            Console.WriteLine("\n👨‍🏫 === ВСЕ ПРЕПОДАВАТЕЛИ ===");
            foreach (var professor in _professors)
            {
                Console.WriteLine(professor.GetFullInfo());
            }
            Console.WriteLine($"📊 Всего преподавателей: {_professors.Count}");
        }

        public Professor FindProfessorById(int id)
        {
            return _professors.FirstOrDefault(p => p.Id == id);
        }

        public void AddCourse(string name, string code, int credits, int maxStudents, string description = "")
        {
            try
            {
                var course = new Course(name, code, credits, maxStudents, description);
                _courses.Add(course);
                Console.WriteLine($"✅ Курс успешно создан: {course.Name} ({course.Code})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при создании курса: {ex.Message}");
            }
        }

        public void DisplayAllCourses()
        {
            if (_courses.Count == 0)
            {
                Console.WriteLine("📭 В системе нет курсов.");
                return;
            }

            Console.WriteLine("\n📚 === ВСЕ КУРСЫ ===");
            foreach (var course in _courses)
            {
                Console.WriteLine(course.GetFullInfo());
                Console.WriteLine("---");
            }
            Console.WriteLine($"📊 Всего курсов: {_courses.Count}");
        }

        public Course FindCourseByCode(string code)
        {
            return _courses.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
    }
}