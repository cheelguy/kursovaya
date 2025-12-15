using System;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using UniversityIS.Models;
using UniversityIS.Services;

namespace UniversityIS.ViewModels
{
    // ViewModel для окна ввода оценки студента
    // Управляет вводом баллов за семестр и экзамен/зачет
    // Автоматически рассчитывает итоговую оценку по правилам вуза
    public class GradeInputViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private readonly Student _student;
        private readonly Discipline _discipline;
        private int _semesterPoints;
        private int _examPoints;
        private int _totalPoints;
        private string _grade = string.Empty;
        private string _errorMessage = string.Empty;
        private int _maxSemesterPoints;
        private int _maxExamPoints;

        public GradeInputViewModel(DataService dataService, Student student, Discipline discipline)
        {
            _dataService = dataService;
            _student = student;
            _discipline = discipline;

            // Определяем максимальные баллы в зависимости от формы контроля
            if (discipline.ControlForm == ControlForm.Exam)
            {
                MaxSemesterPoints = 60;
                MaxExamPoints = 40;
                ControlFormName = "Экзамен";
            }
            else // Credit или DifferentiatedCredit
            {
                MaxSemesterPoints = 80;
                MaxExamPoints = 20;
                ControlFormName = discipline.ControlForm == ControlForm.Pass ? "Зачет" : "Дифференцированный зачет";
            }

            // Загружаем существующую оценку, если есть
            LoadExistingGrade();

            SaveCommand = ReactiveCommand.Create(SaveGrade, outputScheduler: RxApp.MainThreadScheduler);
            CalculateCommand = ReactiveCommand.Create(Calculate, outputScheduler: RxApp.MainThreadScheduler);
            
            // Подписываемся на изменение баллов для автоматического расчета
            this.WhenAnyValue(x => x.SemesterPoints, x => x.ExamPoints)
                .Subscribe(_ => Calculate());
        }

        public string DisciplineName => _discipline.Name;
        public string StudentName => $"{_student.LastName} {_student.FirstName} {_student.MiddleName}".Trim();
        public string ControlFormName { get; }

        public int MaxSemesterPoints
        {
            get => _maxSemesterPoints;
            set => this.RaiseAndSetIfChanged(ref _maxSemesterPoints, value);
        }

        public int MaxExamPoints
        {
            get => _maxExamPoints;
            set => this.RaiseAndSetIfChanged(ref _maxExamPoints, value);
        }

        public int SemesterPoints
        {
            get => _semesterPoints;
            set => this.RaiseAndSetIfChanged(ref _semesterPoints, Math.Clamp(value, 0, MaxSemesterPoints));
        }

        public int ExamPoints
        {
            get => _examPoints;
            set => this.RaiseAndSetIfChanged(ref _examPoints, Math.Clamp(value, 0, MaxExamPoints));
        }

        public int TotalPoints
        {
            get => _totalPoints;
            set => this.RaiseAndSetIfChanged(ref _totalPoints, value);
        }

        public string Grade
        {
            get => _grade;
            set => this.RaiseAndSetIfChanged(ref _grade, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CalculateCommand { get; }

        // Загружает существующую оценку студента по этой дисциплине, если она есть
        // Заполняет поля ввода уже имеющимися баллами
        private void LoadExistingGrade()
        {
            var existingGrade = _dataService.Grades.FirstOrDefault(g => 
                g.StudentId == _student.Id && g.DisciplineId == _discipline.Id);

            if (existingGrade != null)
            {
                SemesterPoints = existingGrade.SemesterPoints;
                ExamPoints = existingGrade.ExamPoints;
                Calculate();
            }
        }

        // Рассчитывает итоговую оценку на основе введенных баллов
        // Шкала оценивания зависит от формы контроля (экзамен/зачет/дифзачет)
        private void Calculate()
        {
            TotalPoints = SemesterPoints + ExamPoints;

            // Для экзамена и дифференцированного зачета используется 5-балльная шкала
            if (_discipline.ControlForm == ControlForm.Exam || _discipline.ControlForm == ControlForm.DifferentiatedPass)
            {
                if (TotalPoints < 50)
                    Grade = "Неудовлетворительно (2) - Долг";
                else if (TotalPoints <= 72)
                    Grade = "Удовлетворительно (3)";
                else if (TotalPoints <= 86)
                    Grade = "Хорошо (4)";
                else
                    Grade = "Отлично (5)";
            }
            else // Для обычного зачета - зачет/незачет
            {
                Grade = TotalPoints >= 50 ? "Зачет" : "Незачет";
            }
        }

        // Сохраняет оценку в базу данных
        // Если оценка уже существует - обновляет ее, иначе создает новую
        private void SaveGrade()
        {
            ErrorMessage = string.Empty;

            var existingGrade = _dataService.Grades.FirstOrDefault(g => 
                g.StudentId == _student.Id && g.DisciplineId == _discipline.Id);

            if (existingGrade != null)
            {
                // Обновляем существующую оценку
                existingGrade.SemesterPoints = SemesterPoints;
                existingGrade.ExamPoints = ExamPoints;
                existingGrade.CalculateGrade(_discipline.ControlForm);
            }
            else
            {
                // Создаем новую оценку
                var newGrade = new StudentGrade
                {
                    StudentId = _student.Id,
                    DisciplineId = _discipline.Id,
                    SemesterPoints = SemesterPoints,
                    ExamPoints = ExamPoints
                };
                newGrade.CalculateGrade(_discipline.ControlForm);
                _dataService.Grades.Add(newGrade);
            }

            _dataService.SaveAllData();
        }
    }
}
