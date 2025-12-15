namespace UniversityIS.Models
{
    // Методы расширения для enum'ов
    public static class EnumExtensions
    {
        
        // Получить русское название для TeacherPosition
        
        public static string ToRussianString(this TeacherPosition position)
        {
            return position switch
            {
                TeacherPosition.Assistant => "Ассистент",
                TeacherPosition.Lecturer => "Преподаватель",
                TeacherPosition.SeniorLecturer => "Старший преподаватель",
                TeacherPosition.AssociateProfessor => "Доцент",
                TeacherPosition.Professor => "Профессор",
                _ => position.ToString()
            };
        }
        
        
        // Получить русское название для AcademicDegree
        
        public static string ToRussianString(this AcademicDegree degree)
        {
            return degree switch
            {
                AcademicDegree.None => "Без степени",
                AcademicDegree.CandidateOfSciences => "Кандидат наук",
                AcademicDegree.DoctorOfSciences => "Доктор наук",
                _ => degree.ToString()
            };
        }
        
        
        // Получить русское название для AcademicTitle
        
        public static string ToRussianString(this AcademicTitle title)
        {
            return title switch
            {
                AcademicTitle.None => "Без звания",
                AcademicTitle.AssociateProfessor => "Доцент",
                AcademicTitle.Professor => "Профессор",
                _ => title.ToString()
            };
        }
        
        
        // Получить русское название для LessonType
        
        public static string ToRussianString(this LessonType lessonType)
        {
            return lessonType switch
            {
                LessonType.Lecture => "Лекции",
                LessonType.Seminar => "Семинары",
                LessonType.Laboratory => "Лабораторные работы",
                LessonType.Consultation => "Консультации",
                LessonType.CourseWork => "Курсовые работы",
                LessonType.CourseProject => "Курсовые проекты",
                _ => lessonType.ToString()
            };
        }
        
        
        // Получить русское название для ControlForm
        
        public static string ToRussianString(this ControlForm controlForm)
        {
            return controlForm switch
            {
                ControlForm.Pass => "Зачёт",
                ControlForm.DifferentiatedPass => "Дифференцированный зачёт",
                ControlForm.Exam => "Экзамен",
                _ => controlForm.ToString()
            };
        }
    }
}
