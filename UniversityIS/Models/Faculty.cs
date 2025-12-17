using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Модель факультета университета
    // Содержит информацию о факультете: название, декан, список групп и кафедр
    public class Faculty : ModelBase
    {
        private string _name = string.Empty;
        private string _dean = string.Empty;
        
        // Название факультета
        public string Name 
        { 
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        // ФИО декана факультета
        public string Dean 
        { 
            get => _dean;
            set => this.RaiseAndSetIfChanged(ref _dean, value);
        }
        
        // Список идентификаторов групп, принадлежащих факультету
        public List<Guid> GroupIds { get; set; }
        
        // Список идентификаторов кафедр, принадлежащих факультету
        public List<Guid> DepartmentIds { get; set; }
        
        public Faculty()
        {
            Name = string.Empty;
            Dean = string.Empty;
            GroupIds = new List<Guid>();
            DepartmentIds = new List<Guid>();
        }
        
        public override string ToString()
        {
            return Name;
        }
        
        // Преобразует объект факультета в строку для сохранения в текстовый файл
        // Формат: Id|Name|Dean|GroupIds|DepartmentIds
        public override string ToFileString()
        {
            return $"{Id}|{Name}|{Dean}|{string.Join(",", GroupIds)}|{string.Join(",", DepartmentIds)}";
        }
        
        // Создает объект факультета из строки, загруженной из текстового файла
        // Парсит строку формата: Id|Name|Dean|GroupIds|DepartmentIds
        public static Faculty FromFileString(string line)
        {
            var parts = line.Split('|');
            var faculty = new Faculty
            {
                Id = Guid.Parse(parts[0]),
                Name = parts[1],
                Dean = parts[2]
            };
            
            if (!string.IsNullOrEmpty(parts[3]))
                faculty.GroupIds = parts[3].Split(',').Select(Guid.Parse).ToList();
            
            if (parts.Length > 4 && !string.IsNullOrEmpty(parts[4]))
                faculty.DepartmentIds = parts[4].Split(',').Select(Guid.Parse).ToList();
            
            return faculty;
        }
    }
}
