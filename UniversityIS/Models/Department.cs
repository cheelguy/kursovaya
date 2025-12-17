using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Кафедра
    public class Department : ModelBase
    {
        private string _name = string.Empty;
        private string _head = string.Empty;
        private Guid _facultyId;
        
        // Название кафедры
        public string Name 
        { 
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        // Заведующий кафедрой
        public string Head 
        { 
            get => _head;
            set => this.RaiseAndSetIfChanged(ref _head, value);
        }
        
        // Идентификатор факультета, к которому относится кафедра
        public Guid FacultyId 
        { 
            get => _facultyId;
            set => this.RaiseAndSetIfChanged(ref _facultyId, value);
        }
        
        // Список преподавателей кафедры
        public List<Guid> TeacherIds { get; set; }
        
        public Department()
        {
            Name = string.Empty;
            Head = string.Empty;
            FacultyId = Guid.Empty;
            TeacherIds = new List<Guid>();
        }
        
        public override string ToString()
        {
            return Name;
        }
        
        // Сохранение в строку для текстового файла
        public override string ToFileString()
        {
            return $"{Id}|{Name}|{Head}|{FacultyId}|{string.Join(",", TeacherIds)}";
        }
        
        // Загрузка из строки текстового файла
        public static Department FromFileString(string line)
        {
            var parts = line.Split('|');
            var department = new Department
            {
                Id = Guid.Parse(parts[0]),
                Name = parts[1],
                Head = parts[2],
                FacultyId = Guid.Parse(parts[3])
            };
            
            if (parts.Length > 4 && !string.IsNullOrEmpty(parts[4]))
                department.TeacherIds = parts[4].Split(',').Select(Guid.Parse).ToList();
            
            return department;
        }
    }
}
