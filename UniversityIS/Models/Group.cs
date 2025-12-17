using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Учебная группа
    public class Group : ModelBase
    {
        private string _number = string.Empty;
        private int _yearOfAdmission;
        private int _course;
        private Guid _facultyId;
        
        // Номер группы
        public string Number 
        { 
            get => _number;
            set => this.RaiseAndSetIfChanged(ref _number, value);
        }
        
        // Год набора
        public int YearOfAdmission 
        { 
            get => _yearOfAdmission;
            set => this.RaiseAndSetIfChanged(ref _yearOfAdmission, value);
        }
        
        // Курс
        public int Course 
        { 
            get => _course;
            set => this.RaiseAndSetIfChanged(ref _course, value);
        }
        
        // Идентификатор факультета
        public Guid FacultyId 
        { 
            get => _facultyId;
            set => this.RaiseAndSetIfChanged(ref _facultyId, value);
        }
        
        // Список студентов группы
        public List<Guid> StudentIds { get; set; }
        
        public Group()
        {
            Number = string.Empty;
            YearOfAdmission = DateTime.Now.Year;
            Course = 1;
            FacultyId = Guid.Empty;
            StudentIds = new List<Guid>();
        }
        
        public override string ToString()
        {
            return $"{Number} ({Course} курс, {YearOfAdmission})";
        }
        
        // Сохранение в строку для текстового файла
        public override string ToFileString()
        {
            return $"{Id}|{Number}|{YearOfAdmission}|{Course}|{FacultyId}|{string.Join(",", StudentIds)}";
        }
        
        // Загрузка из строки текстового файла
        public static Group FromFileString(string line)
        {
            var parts = line.Split('|');
            var group = new Group
            {
                Id = Guid.Parse(parts[0]),
                Number = parts[1],
                YearOfAdmission = int.Parse(parts[2]),
                Course = int.Parse(parts[3]),
                FacultyId = Guid.Parse(parts[4])
            };
            
            if (parts.Length > 5 && !string.IsNullOrEmpty(parts[5]))
                group.StudentIds = parts[5].Split(',').Select(Guid.Parse).ToList();
            
            return group;
        }
    }
}
