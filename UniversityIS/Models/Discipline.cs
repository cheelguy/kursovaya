using System;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Модель учебной дисциплины
    // Содержит информацию о предмете: название, курс, семестр, часы, форма контроля
    // Дисциплина привязана к конкретной группе
    public class Discipline : ModelBase
    {
        private string _name = string.Empty;
        private int _course;
        private int _semester;
        private int _lectureHours;
        private int _seminarHours;
        private int _laboratoryHours;
        private ControlForm _controlForm;
        private Guid _groupId;
        
        
        // Название дисциплины
        
        public string Name 
        { 
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        
        // Курс, на котором читается дисциплина
        
        public int Course 
        { 
            get => _course;
            set => this.RaiseAndSetIfChanged(ref _course, value);
        }
        
        
        // Семестр
        
        public int Semester 
        { 
            get => _semester;
            set => this.RaiseAndSetIfChanged(ref _semester, value);
        }
        
        
        // Количество часов лекций
        
        public int LectureHours 
        { 
            get => _lectureHours;
            set => this.RaiseAndSetIfChanged(ref _lectureHours, value);
        }
        
        
        // Количество часов семинаров
        
        public int SeminarHours 
        { 
            get => _seminarHours;
            set => this.RaiseAndSetIfChanged(ref _seminarHours, value);
        }
        
        
        // Количество часов лабораторных работ
        
        public int LaboratoryHours 
        { 
            get => _laboratoryHours;
            set => this.RaiseAndSetIfChanged(ref _laboratoryHours, value);
        }
        
        
        // Форма контроля
        
        public ControlForm ControlForm 
        { 
            get => _controlForm;
            set => this.RaiseAndSetIfChanged(ref _controlForm, value);
        }

        
        // Идентификатор группы
        
        public Guid GroupId 
        { 
            get => _groupId;
            set => this.RaiseAndSetIfChanged(ref _groupId, value);
        }
        
        public Discipline()
        {
            Name = string.Empty;
            Course = 1;
            Semester = 1;
            LectureHours = 0;
            SeminarHours = 0;
            LaboratoryHours = 0;
            ControlForm = ControlForm.Exam;
        }

        // Вычисляет номер курса на основе номера семестра
        // Формула: 1-2 семестр = 1 курс, 3-4 = 2 курс, 5-6 = 3 курс и т.д.
        public static int CalculateCourseFromSemester(int semester)
        {
            return (semester + 1) / 2;
        }

        // Устанавливает семестр и автоматически пересчитывает курс
        // Это гарантирует, что курс всегда соответствует семестру
        public void SetSemester(int semester)
        {
            Semester = semester;
            Course = CalculateCourseFromSemester(semester);
        }
        
        public override string ToString()
        {
            return $"{Name} ({Course} курс, {Semester} сем.)";
        }
        
        // Преобразует объект дисциплины в строку для сохранения в текстовый файл
        // Формат: Id|Name|Course|Semester|LectureHours|SeminarHours|LaboratoryHours|ControlForm|GroupId
        public override string ToFileString()
        {
            return $"{Id}|{Name}|{Course}|{Semester}|{LectureHours}|{SeminarHours}|{LaboratoryHours}|{(int)ControlForm}|{GroupId}";
        }
        
        // Создает объект дисциплины из строки, загруженной из текстового файла
        // Поддерживает обратную совместимость со старым форматом (без GroupId)
        public static Discipline FromFileString(string line)
        {
            var parts = line.Split('|');
            return new Discipline
            {
                Id = Guid.Parse(parts[0]),
                Name = parts[1],
                Course = int.Parse(parts[2]),
                Semester = int.Parse(parts[3]),
                LectureHours = int.Parse(parts[4]),
                SeminarHours = int.Parse(parts[5]),
                LaboratoryHours = int.Parse(parts[6]),
                ControlForm = (ControlForm)int.Parse(parts[7]),
                GroupId = parts.Length > 8 ? Guid.Parse(parts[8]) : Guid.Empty // Обратная совместимость
            };
        }
    }
}
