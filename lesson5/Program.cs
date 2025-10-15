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
}