using System;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Студент
    public class Student : ReactiveObject
    {
        private Guid _id;
        private string _lastName = string.Empty;
        private string _firstName = string.Empty;
        private string _middleName = string.Empty;
        private Guid _groupId;
        private string _recordBookNumber = string.Empty;
        private double _gpa;
        
        // Уникальный идентификатор
        public Guid Id 
        { 
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }
        
        // Фамилия
        public string LastName 
        { 
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }
        
        // Имя
        public string FirstName 
        { 
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }
        
        // Отчество
        public string MiddleName 
        { 
            get => _middleName;
            set => this.RaiseAndSetIfChanged(ref _middleName, value);
        }
        
        // Идентификатор группы
        public Guid GroupId 
        { 
            get => _groupId;
            set => this.RaiseAndSetIfChanged(ref _groupId, value);
        }
        
        // Номер зачётной книжки
        public string RecordBookNumber 
        { 
            get => _recordBookNumber;
            set => this.RaiseAndSetIfChanged(ref _recordBookNumber, value);
        }
        
        // Средний балл
        public double GPA 
        { 
            get => _gpa;
            set => this.RaiseAndSetIfChanged(ref _gpa, value);
        }
        
        public Student()
        {
            Id = Guid.NewGuid();
            LastName = string.Empty;
            FirstName = string.Empty;
            MiddleName = string.Empty;
            GroupId = Guid.Empty;
            RecordBookNumber = string.Empty;
            GPA = 0.0;
        }
        
        // Полное имя студента
        public string FullName => $"{LastName} {FirstName} {MiddleName}";
        
        public override string ToString()
        {
            return FullName;
        }
        
        // Сохранение в строку для текстового файла
        public string ToFileString()
        {
            return $"{Id}|{LastName}|{FirstName}|{MiddleName}|{GroupId}|{RecordBookNumber}|{GPA}";
        }
        
        // Загрузка из строки текстового файла
        public static Student FromFileString(string line)
        {
            var parts = line.Split('|');
            return new Student
            {
                Id = Guid.Parse(parts[0]),
                LastName = parts[1],
                FirstName = parts[2],
                MiddleName = parts[3],
                GroupId = Guid.Parse(parts[4]),
                RecordBookNumber = parts[5],
                GPA = double.Parse(parts[6])
            };
        }
    }
}
