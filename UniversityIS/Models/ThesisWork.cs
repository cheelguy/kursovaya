using System;
using ReactiveUI;

namespace UniversityIS.Models
{
    
    // Дипломная работа
    
    public class ThesisWork : ReactiveObject
    {
        private Guid _id;
        private string _title = string.Empty;
        private Guid _studentId;
        private Guid _supervisorId;
        private int _year;
        private string _grade = string.Empty;
        
        
        // Уникальный идентификатор
        
        public Guid Id 
        { 
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }
        
        
        // Название работы
        
        public string Title 
        { 
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
        
        
        // Идентификатор студента
        
        public Guid StudentId 
        { 
            get => _studentId;
            set => this.RaiseAndSetIfChanged(ref _studentId, value);
        }
        
        
        // Идентификатор научного руководителя
        
        public Guid SupervisorId 
        { 
            get => _supervisorId;
            set => this.RaiseAndSetIfChanged(ref _supervisorId, value);
        }
        
        
        // Год защиты
        
        public int Year 
        { 
            get => _year;
            set => this.RaiseAndSetIfChanged(ref _year, value);
        }
        
        
        // Оценка (2-5, null если не выставлена)
        
        public int? Grade 
        { 
            get => string.IsNullOrEmpty(_grade) ? null : int.Parse(_grade);
            set => this.RaiseAndSetIfChanged(ref _grade, value?.ToString() ?? string.Empty);
        }
        
        public ThesisWork()
        {
            Id = Guid.NewGuid();
            Title = string.Empty;
            StudentId = Guid.Empty;
            SupervisorId = Guid.Empty;
            Year = DateTime.Now.Year;
            Grade = null;
        }
        
        
        // Сохранение в строку для текстового файла
        
        public string ToFileString()
        {
            return $"{Id}|{Title}|{StudentId}|{SupervisorId}|{Year}|{_grade}";
        }
        
        
        // Загрузка из строки текстового файла
        
        public static ThesisWork FromFileString(string line)
        {
            var parts = line.Split('|');
            var work = new ThesisWork
            {
                Id = Guid.Parse(parts[0]),
                Title = parts[1],
                StudentId = Guid.Parse(parts[2]),
                SupervisorId = Guid.Parse(parts[3]),
                Year = int.Parse(parts[4])
            };
            
            // Парсим оценку: если пустая строка или не число - null, иначе int
            if (!string.IsNullOrWhiteSpace(parts[5]) && int.TryParse(parts[5], out int gradeValue))
            {
                work.Grade = gradeValue;
            }
            else
            {
                work.Grade = null;
            }
            
            return work;
        }
    }
}
