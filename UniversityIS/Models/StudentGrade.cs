using System;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Модель оценки студента по дисциплине
    // Хранит баллы за семестр и экзамен/зачет, автоматически рассчитывает итоговую оценку
    // Система оценивания: экзамен (60+40), зачет (80+20), дифзачет (80+20)
    public class StudentGrade : ReactiveObject
    {
        private Guid _id;
        private Guid _studentId;
        private Guid _disciplineId;
        private int _semesterPoints;
        private int _examPoints;
        private int _totalPoints;
        private string _grade = string.Empty;

        public StudentGrade()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public Guid StudentId
        {
            get => _studentId;
            set => this.RaiseAndSetIfChanged(ref _studentId, value);
        }

        public Guid DisciplineId
        {
            get => _disciplineId;
            set => this.RaiseAndSetIfChanged(ref _disciplineId, value);
        }

        // Баллы за работу в семестре
        // Максимум: 60 для экзамена, 80 для зачета/дифзачета
        public int SemesterPoints
        {
            get => _semesterPoints;
            set => this.RaiseAndSetIfChanged(ref _semesterPoints, value);
        }

        // Баллы на экзамене или зачете
        // Максимум: 40 для экзамена, 20 для зачета/дифзачета
        public int ExamPoints
        {
            get => _examPoints;
            set => this.RaiseAndSetIfChanged(ref _examPoints, value);
        }

        // Общая сумма баллов (семестр + экзамен/зачет)
        // Используется для определения итоговой оценки
        public int TotalPoints
        {
            get => _totalPoints;
            set => this.RaiseAndSetIfChanged(ref _totalPoints, value);
        }

        // Итоговая оценка в текстовом виде
        // Для экзамена/дифзачета: "Отлично (5)", "Хорошо (4)", "Удовлетворительно (3)", "Неудовлетворительно (2)"
        // Для зачета: "Зачет" или "Незачет"
        public string Grade
        {
            get => _grade;
            set => this.RaiseAndSetIfChanged(ref _grade, value);
        }

        // Рассчитывает итоговую оценку на основе набранных баллов и формы контроля
        // Шкала оценивания:
        // - Экзамен/Дифзачет: 0-49 (2), 50-72 (3), 73-86 (4), 87-100 (5)
        // - Зачет: 0-49 (незачет), 50-100 (зачет)
        public void CalculateGrade(ControlForm controlForm)
        {
            TotalPoints = SemesterPoints + ExamPoints;

            if (controlForm == ControlForm.Exam || controlForm == ControlForm.DifferentiatedPass)
            {
                // Для экзамена и дифференцированного зачета
                if (TotalPoints < 50)
                    Grade = "Неудовлетворительно (2)";
                else if (TotalPoints <= 72)
                    Grade = "Удовлетворительно (3)";
                else if (TotalPoints <= 86)
                    Grade = "Хорошо (4)";
                else
                    Grade = "Отлично (5)";
            }
            else // Credit (зачет)
            {
                Grade = TotalPoints >= 50 ? "Зачет" : "Незачет";
            }
        }

        public string ToFileString()
        {
            return $"{Id}|{StudentId}|{DisciplineId}|{SemesterPoints}|{ExamPoints}|{TotalPoints}|{Grade}";
        }

        public static StudentGrade FromFileString(string line)
        {
            var parts = line.Split('|');
            return new StudentGrade
            {
                Id = Guid.Parse(parts[0]),
                StudentId = Guid.Parse(parts[1]),
                DisciplineId = Guid.Parse(parts[2]),
                SemesterPoints = int.Parse(parts[3]),
                ExamPoints = int.Parse(parts[4]),
                TotalPoints = int.Parse(parts[5]),
                Grade = parts[6]
            };
        }

        public override string ToString()
        {
            return $"{TotalPoints} баллов - {Grade}";
        }
    }
}
