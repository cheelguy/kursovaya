using System;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Преподаватель
    public class Teacher : ModelBase
    {
        private string _lastName = string.Empty;
        private string _firstName = string.Empty;
        private string _middleName = string.Empty;
        private TeacherPosition _position;
        private AcademicDegree _degree;
        private AcademicTitle _title;
        private Guid _departmentId;
        private bool _isPostgraduate;
        private bool _leadsResearchTopics;
        private bool _leadsResearchDirections;
        
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
        
        // Должность
        public TeacherPosition Position 
        { 
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }
        
        // Учёная степень
        public AcademicDegree Degree 
        { 
            get => _degree;
            set => this.RaiseAndSetIfChanged(ref _degree, value);
        }
        
        // Учёное звание
        public AcademicTitle Title 
        { 
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
        
        // Идентификатор кафедры
        public Guid DepartmentId 
        { 
            get => _departmentId;
            set => this.RaiseAndSetIfChanged(ref _departmentId, value);
        }
        
        // Обучается в аспирантуре
        public bool IsPostgraduate 
        { 
            get => _isPostgraduate;
            set => this.RaiseAndSetIfChanged(ref _isPostgraduate, value);
        }
        
        // Руководит научными темами
        public bool LeadsResearchTopics 
        { 
            get => _leadsResearchTopics;
            set => this.RaiseAndSetIfChanged(ref _leadsResearchTopics, value);
        }
        
        // Руководит научными направлениями
        public bool LeadsResearchDirections 
        { 
            get => _leadsResearchDirections;
            set => this.RaiseAndSetIfChanged(ref _leadsResearchDirections, value);
        }
        
        public Teacher()
        {
            LastName = string.Empty;
            FirstName = string.Empty;
            MiddleName = string.Empty;
            Position = TeacherPosition.Assistant;
            Degree = AcademicDegree.None;
            Title = AcademicTitle.None;
            DepartmentId = Guid.Empty;
            IsPostgraduate = false;
            LeadsResearchTopics = false;
            LeadsResearchDirections = false;
        }
        
        // Полное имя преподавателя
        public string FullName => $"{LastName} {FirstName} {MiddleName}";
        
        // Полная информация о преподавателе с должностью и званиями
        public string FullInfo
        {
            get
            {
                var info = FullName;
                if (Degree != AcademicDegree.None)
                    info += $", {GetDegreeString(Degree)}";
                if (Title != AcademicTitle.None)
                    info += $", {GetTitleString(Title)}";
                info += $", {GetPositionString(Position)}";
                return info;
            }
        }
        
        public override string ToString()
        {
            return FullInfo;
        }
        
        // Проверка возможности занимать текущую должность с текущим званием
        public bool IsPositionValid()
        {
            // Доцент может быть только со званием доцента или выше
            if (Position == TeacherPosition.AssociateProfessor && 
                Title != AcademicTitle.AssociateProfessor && Title != AcademicTitle.Professor)
                return false;
            
            // Профессор может быть только со званием профессора
            if (Position == TeacherPosition.Professor && Title != AcademicTitle.Professor)
                return false;
            
            return true;
        }
        
        // Может ли преподаватель читать лекции
        public bool CanTeachLectures()
        {
            return Position != TeacherPosition.Assistant;
        }
        
        // Может ли преподаватель проводить лабораторные работы
        public bool CanTeachLaboratory()
        {
            return Position != TeacherPosition.Professor;
        }
        
        // Сохранение в строку для текстового файла
        public override string ToFileString()
        {
            return $"{Id}|{LastName}|{FirstName}|{MiddleName}|{(int)Position}|{(int)Degree}|" +
                   $"{(int)Title}|{DepartmentId}|{IsPostgraduate}|{LeadsResearchTopics}|{LeadsResearchDirections}";
        }
        
        // Загрузка из строки текстового файла
        public static Teacher FromFileString(string line)
        {
            var parts = line.Split('|');
            return new Teacher
            {
                Id = Guid.Parse(parts[0]),
                LastName = parts[1],
                FirstName = parts[2],
                MiddleName = parts[3],
                Position = (TeacherPosition)int.Parse(parts[4]),
                Degree = (AcademicDegree)int.Parse(parts[5]),
                Title = (AcademicTitle)int.Parse(parts[6]),
                DepartmentId = Guid.Parse(parts[7]),
                IsPostgraduate = bool.Parse(parts[8]),
                LeadsResearchTopics = bool.Parse(parts[9]),
                LeadsResearchDirections = bool.Parse(parts[10])
            };
        }
        
        private static string GetPositionString(TeacherPosition position)
        {
            return position switch
            {
                TeacherPosition.Assistant => "ассистент",
                TeacherPosition.Lecturer => "преподаватель",
                TeacherPosition.SeniorLecturer => "ст. преподаватель",
                TeacherPosition.AssociateProfessor => "доцент",
                TeacherPosition.Professor => "профессор",
                _ => ""
            };
        }
        
        private static string GetDegreeString(AcademicDegree degree)
        {
            return degree switch
            {
                AcademicDegree.CandidateOfSciences => "к.н.",
                AcademicDegree.DoctorOfSciences => "д.н.",
                _ => ""
            };
        }
        
        private static string GetTitleString(AcademicTitle title)
        {
            return title switch
            {
                AcademicTitle.AssociateProfessor => "доцент",
                AcademicTitle.Professor => "профессор",
                _ => ""
            };
        }
    }
}
