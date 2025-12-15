using System;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Модель учебной нагрузки преподавателя
    // Описывает, какой преподаватель ведет какой предмет у какой группы
    // Содержит информацию о типе занятий (лекции, семинары, лабораторные) и количестве часов
    public class WorkLoad : ReactiveObject
    {
        private Guid _id;
        private Guid _teacherId;
        private Guid _disciplineId;
        private Guid _groupId;
        private LessonType _lessonType;
        private int _hours;
        private string _academicYear = string.Empty;
        private int _semester;
        
        
        // Уникальный идентификатор
        
        public Guid Id 
        { 
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }
        
        
        // Идентификатор преподавателя
        
        public Guid TeacherId 
        { 
            get => _teacherId;
            set => this.RaiseAndSetIfChanged(ref _teacherId, value);
        }
        
        
        // Идентификатор дисциплины
        
        public Guid DisciplineId 
        { 
            get => _disciplineId;
            set => this.RaiseAndSetIfChanged(ref _disciplineId, value);
        }
        
        
        // Идентификатор группы
        
        public Guid GroupId 
        { 
            get => _groupId;
            set => this.RaiseAndSetIfChanged(ref _groupId, value);
        }
        
        
        // Вид занятий
        
        public LessonType LessonType 
        { 
            get => _lessonType;
            set => this.RaiseAndSetIfChanged(ref _lessonType, value);
        }
        
        
        // Количество часов
        
        public int Hours 
        { 
            get => _hours;
            set => this.RaiseAndSetIfChanged(ref _hours, value);
        }
        
        
        // Учебный год
        
        public string AcademicYear 
        { 
            get => _academicYear;
            set => this.RaiseAndSetIfChanged(ref _academicYear, value);
        }
        
        
        // Семестр
        
        public int Semester 
        { 
            get => _semester;
            set => this.RaiseAndSetIfChanged(ref _semester, value);
        }
        
        public WorkLoad()
        {
            Id = Guid.NewGuid();
            TeacherId = Guid.Empty;
            DisciplineId = Guid.Empty;
            GroupId = Guid.Empty;
            LessonType = LessonType.Lecture;
            Hours = 0;
            AcademicYear = $"{DateTime.Now.Year}/{DateTime.Now.Year + 1}";
            Semester = 1;
        }
        
        
        // Сохранение в строку для текстового файла
        
        public string ToFileString()
        {
            return $"{Id}|{TeacherId}|{DisciplineId}|{GroupId}|{(int)LessonType}|{Hours}|{AcademicYear}|{Semester}";
        }
        
        
        // Загрузка из строки текстового файла
        
        public static WorkLoad FromFileString(string line)
        {
            var parts = line.Split('|');
            return new WorkLoad
            {
                Id = Guid.Parse(parts[0]),
                TeacherId = Guid.Parse(parts[1]),
                DisciplineId = Guid.Parse(parts[2]),
                GroupId = Guid.Parse(parts[3]),
                LessonType = (LessonType)int.Parse(parts[4]),
                Hours = int.Parse(parts[5]),
                AcademicYear = parts[6],
                Semester = int.Parse(parts[7])
            };
        }
        
        public static string GetLessonTypeString(LessonType lessonType)
        {
            return lessonType switch
            {
                LessonType.Lecture => "Лекции",
                LessonType.Seminar => "Семинары",
                LessonType.Laboratory => "Лабораторные",
                LessonType.Consultation => "Консультации",
                LessonType.CourseWork => "Курсовая работа",
                LessonType.CourseProject => "Курсовой проект",
                _ => ""
            };
        }
    }
}
