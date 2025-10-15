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
                Console.WriteLine($"Студент успешно добавлен: {student.GetFullInfo()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении студента: {ex.Message}");
            }
        }

        public void DisplayAllStudents()
        {
            if (_students.Count == 0)
            {
                Console.WriteLine("В системе нет студентов.");
                return;
            }

            Console.WriteLine("\n=== ВСЕ СТУДЕНТЫ ===");
            foreach (var student in _students)
            {
                Console.WriteLine(student.GetFullInfo());
            }
            Console.WriteLine($"Всего студентов: {_students.Count}");
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
                Console.WriteLine($"Преподаватель успешно добавлен: {professor.GetFullInfo()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении преподавателя: {ex.Message}");
            }
        }

        public void DisplayAllProfessors()
        {
            if (_professors.Count == 0)
            {
                Console.WriteLine("В системе нет преподавателей.");
                return;
            }

            Console.WriteLine("\n=== ВСЕ ПРЕПОДАВАТЕЛИ ===");
            foreach (var professor in _professors)
            {
                Console.WriteLine(professor.GetFullInfo());
            }
            Console.WriteLine($"Всего преподавателей: {_professors.Count}");
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
                Console.WriteLine($"Курс успешно создан: {course.Name} ({course.Code})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании курса: {ex.Message}");
            }
        }

        public void DisplayAllCourses()
        {
            if (_courses.Count == 0)
            {
                Console.WriteLine("В системе нет курсов.");
                return;
            }

            Console.WriteLine("\n=== ВСЕ КУРСЫ ===");
            foreach (var course in _courses)
            {
                Console.WriteLine(course.GetFullInfo());
                Console.WriteLine("---");
            }
            Console.WriteLine($"Всего курсов: {_courses.Count}");
        }

        public Course FindCourseByCode(string code)
        {
            return _courses.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public void EnrollStudentInCourse(int studentId, string courseCode)
        {
            try
            {
                var student = FindStudentById(studentId);
                var course = FindCourseByCode(courseCode);

                if (student == null)
                {
                    Console.WriteLine("Студент не найден.");
                    return;
                }

                if (course == null)
                {
                    Console.WriteLine("Курс не найден.");
                    return;
                }

                student.EnrollInCourse(course);
                Console.WriteLine($"Студент {student.FirstName} {student.LastName} успешно записан на курс {course.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи на курс: {ex.Message}");
            }
        }

        public void AssignProfessorToCourse(int professorId, string courseCode)
        {
            try
            {
                var professor = FindProfessorById(professorId);
                var course = FindCourseByCode(courseCode);

                if (professor == null)
                {
                    Console.WriteLine("Преподаватель не найден.");
                    return;
                }

                if (course == null)
                {
                    Console.WriteLine("Курс не найден.");
                    return;
                }

                professor.AssignToCourse(course);
                Console.WriteLine($"Преподаватель {professor.FirstName} {professor.LastName} назначен на курс {course.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при назначении преподавателя: {ex.Message}");
            }
        }

        public void DisplayStudentCourses(int studentId)
        {
            var student = FindStudentById(studentId);
            if (student == null)
            {
                Console.WriteLine("Студент не найден.");
                return;
            }

            Console.WriteLine(student.GetCoursesInfo());
        }

        public void DisplayCourseStudents(string courseCode)
        {
            var course = FindCourseByCode(courseCode);
            if (course == null)
            {
                Console.WriteLine("Курс не найден.");
                return;
            }

            Console.WriteLine(course.GetEnrolledStudentsInfo());
        }

        public void DisplayProfessorCourses(int professorId)
        {
            var professor = FindProfessorById(professorId);
            if (professor == null)
            {
                Console.WriteLine("Преподаватель не найден.");
                return;
            }

            Console.WriteLine(professor.GetTeachingCoursesInfo());
        }

        public void DisplayStudentsByMajor(string major)
        {
            var students = _students.Where(s => s.Major.Equals(major, StringComparison.OrdinalIgnoreCase))
                                   .OrderBy(s => s.LastName)
                                   .ThenBy(s => s.FirstName);

            if (!students.Any())
            {
                Console.WriteLine($"Нет студентов на специальности '{major}'");
                return;
            }

            Console.WriteLine($"\n=== СТУДЕНТЫ СПЕЦИАЛЬНОСТИ '{major}' ===");
            foreach (var student in students)
            {
                Console.WriteLine(student.GetFullInfo());
            }
        }

        public void DisplayProfessorsByDepartment(string department)
        {
            var professors = _professors.Where(p => p.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                                       .OrderBy(p => p.LastName)
                                       .ThenBy(p => p.FirstName);

            if (!professors.Any())
            {
                Console.WriteLine($"Нет преподавателей на кафедре '{department}'");
                return;
            }

            Console.WriteLine($"\n=== ПРЕПОДАВАТЕЛИ КАФЕДРЫ '{department}' ===");
            foreach (var professor in professors)
            {
                Console.WriteLine(professor.GetFullInfo());
            }
        }

        public void DisplayCoursesByStatus(CourseStatus status)
        {
            var courses = _courses.Where(c => c.Status == status)
                                 .OrderBy(c => c.Name);

            if (!courses.Any())
            {
                Console.WriteLine($"Нет курсов со статусом '{status}'");
                return;
            }

            Console.WriteLine($"\n=== КУРСЫ СО СТАТУСОМ '{status}' ===");
            foreach (var course in courses)
            {
                Console.WriteLine(course.GetFullInfo());
                Console.WriteLine("---");
            }
        }

        public void DisplayTopStudents(int count = 5)
        {
            var topStudents = _students.Where(s => s.GPA > 0)
                                      .OrderByDescending(s => s.GPA)
                                      .Take(count);

            if (!topStudents.Any())
            {
                Console.WriteLine("Нет студентов с GPA для формирования рейтинга");
                return;
            }

            Console.WriteLine($"\n=== ТОП-{count} СТУДЕНТОВ ПО GPA ===");
            int rank = 1;
            foreach (var student in topStudents)
            {
                Console.WriteLine($"{rank}. {student.FirstName} {student.LastName} - GPA: {student.GPA:F2} | Специальность: {student.Major}");
                rank++;
            }
        }

        public void DisplayCoursesWithAvailableSlots()
        {
            var availableCourses = _courses.Where(c => c.EnrolledStudents.Count < c.MaxStudents && c.Status == CourseStatus.Active)
                                          .OrderBy(c => c.Name);

            if (!availableCourses.Any())
            {
                Console.WriteLine("Нет курсов со свободными местами");
                return;
            }

            Console.WriteLine("\n=== КУРСЫ СО СВОБОДНЫМИ МЕСТАМИ ===");
            foreach (var course in availableCourses)
            {
                int availableSlots = course.MaxStudents - course.EnrolledStudents.Count;
                Console.WriteLine($"{course.Name} ({course.Code}) - Свободно мест: {availableSlots}/{course.MaxStudents}");
            }
        }

        public void DisplayStudentStatistics()
        {
            if (!_students.Any())
            {
                Console.WriteLine("Нет студентов для статистики");
                return;
            }

            Console.WriteLine("\n=== СТАТИСТИКА СТУДЕНТОВ ===");
            Console.WriteLine($"Общее количество студентов: {_students.Count}");
            Console.WriteLine($"Средний GPA: {_students.Average(s => s.GPA):F2}");
            Console.WriteLine($"Максимальный GPA: {_students.Max(s => s.GPA):F2}");
            Console.WriteLine($"Минимальный GPA: {_students.Min(s => s.GPA):F2}");

            var majors = _students.GroupBy(s => s.Major)
                                 .Select(g => new { Major = g.Key, Count = g.Count() })
                                 .OrderByDescending(x => x.Count);

            Console.WriteLine("\nРаспределение по специальностям:");
            foreach (var major in majors)
            {
                Console.WriteLine($"  {major.Major}: {major.Count} студентов");
            }
        }

        public void DisplayUniversityStatistics()
        {
            Console.WriteLine("\n=== СТАТИСТИКА УНИВЕРСИТЕТА ===");
            Console.WriteLine($"Всего студентов: {_students.Count}");
            Console.WriteLine($"Всего преподавателей: {_professors.Count}");
            Console.WriteLine($"Всего курсов: {_courses.Count}");
            Console.WriteLine($"Активных курсов: {_courses.Count(c => c.Status == CourseStatus.Active)}");

            if (_professors.Any())
            {
                Console.WriteLine($"Средняя зарплата преподавателей: {_professors.Average(p => p.Salary):C}");
            }

            var totalEnrollments = _courses.Sum(c => c.EnrolledStudents.Count);
            Console.WriteLine($"Всего записей на курсы: {totalEnrollments}");
        }
    }

    class Program
    {
        private static UniversityManager _universityManager;

        static void Main(string[] args)
        {
            _universityManager = new UniversityManager();
            DisplayWelcomeMessage();
            MainMenu();
        }

        static void DisplayWelcomeMessage()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("СИСТЕМА УПРАВЛЕНИЯ УНИВЕРСИТЕТОМ");
            Console.WriteLine("====================================\n");
            Console.WriteLine("Добро пожаловать! Система загружена с тестовыми данными.\n");
        }

        static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
                Console.WriteLine("1. Управление студентами");
                Console.WriteLine("2. Управление преподавателями");
                Console.WriteLine("3. Управление курсами");
                Console.WriteLine("4. Операции");
                Console.WriteLine("5. Просмотр всех данных");
                Console.WriteLine("6. Статистика и аналитика");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        StudentMenu();
                        break;
                    case "2":
                        ProfessorMenu();
                        break;
                    case "3":
                        CourseMenu();
                        break;
                    case "4":
                        OperationsMenu();
                        break;
                    case "5":
                        DisplayAllDataMenu();
                        break;
                    case "6":
                        StatisticsMenu();
                        break;
                    case "0":
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void StudentMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== УПРАВЛЕНИЕ СТУДЕНТАМИ ===");
                Console.WriteLine("1. Добавить студента");
                Console.WriteLine("2. Просмотреть всех студентов");
                Console.WriteLine("3. Найти студента по ID");
                Console.WriteLine("4. Просмотреть курсы студента");
                Console.WriteLine("5. Студенты по специальности");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        _universityManager.DisplayAllStudents();
                        break;
                    case "3":
                        FindStudentById();
                        break;
                    case "4":
                        DisplayStudentCourses();
                        break;
                    case "5":
                        DisplayStudentsByMajor();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void AddStudent()
        {
            try
            {
                Console.WriteLine("\n--- Добавление нового студента ---");

                Console.Write("Имя: ");
                var firstName = Console.ReadLine();

                Console.Write("Фамилия: ");
                var lastName = Console.ReadLine();

                Console.Write("Возраст: ");
                if (!int.TryParse(Console.ReadLine(), out int age))
                {
                    Console.WriteLine("Некорректный возраст.");
                    return;
                }

                Console.Write("Email: ");
                var email = Console.ReadLine();

                Console.Write("ID студента: ");
                var studentId = Console.ReadLine();

                Console.Write("Специальность: ");
                var major = Console.ReadLine();

                Console.Write("Год обучения: ");
                if (!int.TryParse(Console.ReadLine(), out int yearOfStudy))
                {
                    Console.WriteLine("Некорректный год обучения.");
                    return;
                }

                Console.Write("GPA (по умолчанию 0.0): ");
                var gpaInput = Console.ReadLine();
                double gpa = 0.0;
                if (!string.IsNullOrWhiteSpace(gpaInput) && !double.TryParse(gpaInput, out gpa))
                {
                    Console.WriteLine("Некорректный GPA. Использовано значение по умолчанию 0.0");
                }

                _universityManager.AddStudent(firstName, lastName, age, email, studentId, major, yearOfStudy, gpa);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void FindStudentById()
        {
            Console.Write("Введите ID студента: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var student = _universityManager.FindStudentById(id);
                if (student != null)
                {
                    Console.WriteLine($"Найден студент: {student.GetFullInfo()}");
                }
                else
                {
                    Console.WriteLine("Студент с таким ID не найден.");
                }
            }
            else
            {
                Console.WriteLine("Некорректный ID.");
            }
        }

        static void DisplayStudentCourses()
        {
            Console.Write("Введите ID студента: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                _universityManager.DisplayStudentCourses(id);
            }
            else
            {
                Console.WriteLine("Некорректный ID.");
            }
        }

        static void DisplayStudentsByMajor()
        {
            Console.Write("Введите специальность: ");
            var major = Console.ReadLine();
            _universityManager.DisplayStudentsByMajor(major);
        }

        static void ProfessorMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== УПРАВЛЕНИЕ ПРЕПОДАВАТЕЛЯМИ ===");
                Console.WriteLine("1. Добавить преподавателя");
                Console.WriteLine("2. Просмотреть всех преподавателей");
                Console.WriteLine("3. Найти преподавателя по ID");
                Console.WriteLine("4. Просмотреть курсы преподавателя");
                Console.WriteLine("5. Преподаватели по кафедре");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddProfessor();
                        break;
                    case "2":
                        _universityManager.DisplayAllProfessors();
                        break;
                    case "3":
                        FindProfessorById();
                        break;
                    case "4":
                        DisplayProfessorCourses();
                        break;
                    case "5":
                        DisplayProfessorsByDepartment();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void AddProfessor()
        {
            try
            {
                Console.WriteLine("\n--- Добавление нового преподавателя ---");

                Console.Write("Имя: ");
                var firstName = Console.ReadLine();

                Console.Write("Фамилия: ");
                var lastName = Console.ReadLine();

                Console.Write("Возраст: ");
                if (!int.TryParse(Console.ReadLine(), out int age))
                {
                    Console.WriteLine("Некорректный возраст.");
                    return;
                }

                Console.Write("Email: ");
                var email = Console.ReadLine();

                Console.Write("ID сотрудника: ");
                var employeeId = Console.ReadLine();

                Console.Write("Кафедра: ");
                var department = Console.ReadLine();

                Console.WriteLine("Академическая степень:");
                Console.WriteLine("1. Бакалавр");
                Console.WriteLine("2. Магистр");
                Console.WriteLine("3. Доктор");
                Console.WriteLine("4. Профессор");
                Console.Write("Выберите степень (1-4): ");
                var degreeChoice = Console.ReadLine();

                AcademicDegree degree = degreeChoice switch
                {
                    "1" => AcademicDegree.Bachelor,
                    "2" => AcademicDegree.Master,
                    "3" => AcademicDegree.Doctor,
                    "4" => AcademicDegree.Professor,
                    _ => AcademicDegree.Bachelor
                };

                Console.Write("Зарплата: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal salary))
                {
                    Console.WriteLine("Некорректная зарплата.");
                    return;
                }

                _universityManager.AddProfessor(firstName, lastName, age, email, employeeId, department, degree, salary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void FindProfessorById()
        {
            Console.Write("Введите ID преподавателя: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var professor = _universityManager.FindProfessorById(id);
                if (professor != null)
                {
                    Console.WriteLine($"Найден преподаватель: {professor.GetFullInfo()}");
                }
                else
                {
                    Console.WriteLine("Преподаватель с таким ID не найден.");
                }
            }
            else
            {
                Console.WriteLine("Некорректный ID.");
            }
        }

        static void DisplayProfessorCourses()
        {
            Console.Write("Введите ID преподавателя: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                _universityManager.DisplayProfessorCourses(id);
            }
            else
            {
                Console.WriteLine("Некорректный ID.");
            }
        }

        static void DisplayProfessorsByDepartment()
        {
            Console.Write("Введите название кафедры: ");
            var department = Console.ReadLine();
            _universityManager.DisplayProfessorsByDepartment(department);
        }

        static void CourseMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== УПРАВЛЕНИЕ КУРСАМИ ===");
                Console.WriteLine("1. Создать курс");
                Console.WriteLine("2. Просмотреть все курсы");
                Console.WriteLine("3. Найти курс по коду");
                Console.WriteLine("4. Просмотреть студентов курса");
                Console.WriteLine("5. Курсы со свободными местами");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddCourse();
                        break;
                    case "2":
                        _universityManager.DisplayAllCourses();
                        break;
                    case "3":
                        FindCourseByCode();
                        break;
                    case "4":
                        DisplayCourseStudents();
                        break;
                    case "5":
                        _universityManager.DisplayCoursesWithAvailableSlots();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void AddCourse()
        {
            try
            {
                Console.WriteLine("\n--- Создание нового курса ---");

                Console.Write("Название курса: ");
                var name = Console.ReadLine();

                Console.Write("Код курса: ");
                var code = Console.ReadLine();

                Console.Write("Количество кредитов: ");
                if (!int.TryParse(Console.ReadLine(), out int credits))
                {
                    Console.WriteLine("Некорректное количество кредитов.");
                    return;
                }

                Console.Write("Максимальное количество студентов: ");
                if (!int.TryParse(Console.ReadLine(), out int maxStudents))
                {
                    Console.WriteLine("Некорректное количество студентов.");
                    return;
                }

                Console.Write("Описание (опционально): ");
                var description = Console.ReadLine();

                _universityManager.AddCourse(name, code, credits, maxStudents, description);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void FindCourseByCode()
        {
            Console.Write("Введите код курса: ");
            var code = Console.ReadLine();
            var course = _universityManager.FindCourseByCode(code);
            if (course != null)
            {
                Console.WriteLine(course.GetFullInfo());
            }
            else
            {
                Console.WriteLine("Курс с таким кодом не найден.");
            }
        }

        static void DisplayCourseStudents()
        {
            Console.Write("Введите код курса: ");
            var code = Console.ReadLine();
            _universityManager.DisplayCourseStudents(code);
        }

        static void OperationsMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== ОПЕРАЦИИ ===");
                Console.WriteLine("1. Записать студента на курс");
                Console.WriteLine("2. Назначить преподавателя на курс");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        EnrollStudentInCourse();
                        break;
                    case "2":
                        AssignProfessorToCourse();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void EnrollStudentInCourse()
        {
            Console.Write("Введите ID студента: ");
            if (!int.TryParse(Console.ReadLine(), out int studentId))
            {
                Console.WriteLine("Некорректный ID студента.");
                return;
            }

            Console.Write("Введите код курса: ");
            var courseCode = Console.ReadLine();

            _universityManager.EnrollStudentInCourse(studentId, courseCode);
        }

        static void AssignProfessorToCourse()
        {
            Console.Write("Введите ID преподавателя: ");
            if (!int.TryParse(Console.ReadLine(), out int professorId))
            {
                Console.WriteLine("Некорректный ID преподавателя.");
                return;
            }

            Console.Write("Введите код курса: ");
            var courseCode = Console.ReadLine();

            _universityManager.AssignProfessorToCourse(professorId, courseCode);
        }

        static void DisplayAllDataMenu()
        {
            Console.WriteLine("\n=== ВСЕ ДАННЫЕ СИСТЕМЫ ===");
            _universityManager.DisplayAllStudents();
            _universityManager.DisplayAllProfessors();
            _universityManager.DisplayAllCourses();
        }

        static void StatisticsMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== СТАТИСТИКА И АНАЛИТИКА ===");
                Console.WriteLine("1. Топ студентов по GPA");
                Console.WriteLine("2. Статистика студентов");
                Console.WriteLine("3. Общая статистика университета");
                Console.WriteLine("4. Курсы со свободными местами");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.Write("Количество студентов в топе (по умолчанию 5): ");
                        if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
                        {
                            _universityManager.DisplayTopStudents(count);
                        }
                        else
                        {
                            _universityManager.DisplayTopStudents();
                        }
                        break;
                    case "2":
                        _universityManager.DisplayStudentStatistics();
                        break;
                    case "3":
                        _universityManager.DisplayUniversityStatistics();
                        break;
                    case "4":
                        _universityManager.DisplayCoursesWithAvailableSlots();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }
    }
}