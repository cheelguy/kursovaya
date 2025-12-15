using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using UniversityIS.Models;
using UniversityIS.Services;
using UniversityIS.Views;

namespace UniversityIS.ViewModels
{
    // Обертка для дисциплины с текстом оценки
    // Используется для отображения дисциплины вместе с оценкой студента в профиле
    public class DisciplineWithGrade
    {
        public DisciplineWithGrade(Discipline discipline, string gradeText)
        {
            Discipline = discipline;
            GradeText = gradeText;
        }

        public Discipline Discipline { get; }
        public string GradeText { get; }
    }

    // Группа дисциплин, объединенных по семестру
    // Используется для отображения пройденных предметов с группировкой по семестрам
    public class SemesterGroup
    {
        public SemesterGroup(int semester, IEnumerable<DisciplineWithGrade> disciplines)
        {
            Semester = semester;
            Disciplines = new ObservableCollection<DisciplineWithGrade>(disciplines);
        }

        public int Semester { get; }
        public string SemesterName => $"{Semester} семестр ({(Semester + 1) / 2} курс)";
        public ObservableCollection<DisciplineWithGrade> Disciplines { get; }
    }

    // ViewModel для профиля студента
    // Отображает информацию о студенте, его группе и дисциплинах
    // Разделяет дисциплины на пройденные, текущие и будущие
    public class StudentProfileViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private readonly Student _student;
        private Models.Group? _group;
        private int _currentSemester;
        private ObservableCollection<SemesterGroup> _completedDisciplines = new();
        private ObservableCollection<DisciplineWithGrade> _currentDisciplines = new();
        private ObservableCollection<DisciplineWithGrade> _futureDisciplines = new();

        public StudentProfileViewModel(DataService dataService, Student student)
        {
            _dataService = dataService;
            _student = student;
            
            CloseCommand = ReactiveCommand.Create(() => { });
            OpenGradeInputCommand = ReactiveCommand.Create<DisciplineWithGrade>(OpenGradeInput, outputScheduler: RxApp.MainThreadScheduler);
            
            LoadStudentData();
            
            // Подписываемся на изменения оценок для обновления отображения
            _dataService.Grades.CollectionChanged += (s, e) => LoadStudentData();
        }

        public Student Student => _student;

        public string FullName => $"{_student.LastName} {_student.FirstName} {_student.MiddleName}".Trim();
        
        public string GroupName => _group?.Number ?? "Группа не найдена";
        
        public int CurrentSemester
        {
            get => _currentSemester;
            set => this.RaiseAndSetIfChanged(ref _currentSemester, value);
        }

        public ObservableCollection<SemesterGroup> CompletedDisciplines
        {
            get => _completedDisciplines;
            set => this.RaiseAndSetIfChanged(ref _completedDisciplines, value);
        }

        public ObservableCollection<DisciplineWithGrade> CurrentDisciplines
        {
            get => _currentDisciplines;
            set => this.RaiseAndSetIfChanged(ref _currentDisciplines, value);
        }

        public ObservableCollection<DisciplineWithGrade> FutureDisciplines
        {
            get => _futureDisciplines;
            set => this.RaiseAndSetIfChanged(ref _futureDisciplines, value);
        }

        public ReactiveCommand<Unit, Unit> CloseCommand { get; }
        public ReactiveCommand<DisciplineWithGrade, Unit> OpenGradeInputCommand { get; }

        // Возвращает текстовое представление оценки студента по указанной дисциплине
        // Если оценка не выставлена, возвращает "Оценка не выставлена"
        public string GetGradeForDiscipline(Guid disciplineId)
        {
            var grade = _dataService.Grades.FirstOrDefault(g => 
                g.StudentId == _student.Id && g.DisciplineId == disciplineId);
            
            return grade != null ? $"{grade.TotalPoints} б. - {grade.Grade}" : "Оценка не выставлена";
        }

        // Открывает окно для ввода/редактирования оценки по выбранной дисциплине
        // Окно открывается модально поверх главного окна приложения
        private async void OpenGradeInput(DisciplineWithGrade disciplineWithGrade)
        {
            var viewModel = new GradeInputViewModel(_dataService, _student, disciplineWithGrade.Discipline);
            var window = new GradeInputWindow
            {
                DataContext = viewModel
            };
            
            // Получаем главное окно приложения для модального отображения
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await window.ShowDialog(desktop.MainWindow);
            }
        }

        // Загружает и обрабатывает данные студента для отображения в профиле
        // Определяет текущий семестр на основе времени года и курса
        // Разделяет дисциплины на пройденные, текущие и будущие
        private void LoadStudentData()
        {
            // Получаем группу студента
            _group = _dataService.GetGroup(_student.GroupId);
            if (_group == null) return;

            // Вычисляем текущий семестр группы с учетом времени года
            // Сентябрь (9) - Январь (1): нечетный семестр (1, 3, 5, 7, 9)
            // Февраль (2) - Август (8): четный семестр (2, 4, 6, 8, 10)
            var currentMonth = DateTime.Now.Month;
            bool isFirstHalfOfYear = currentMonth >= 9 || currentMonth <= 1; // сентябрь-январь
            
            if (isFirstHalfOfYear)
            {
                // Первая половина учебного года - нечетный семестр
                CurrentSemester = _group.Course * 2 - 1;
            }
            else
            {
                // Вторая половина учебного года - четный семестр
                CurrentSemester = _group.Course * 2;
            }

            // Получаем все дисциплины группы и сортируем по семестру и названию
            var groupDisciplines = _dataService.Disciplines
                .Where(d => d.GroupId == _group.Id)
                .OrderBy(d => d.Semester)
                .ThenBy(d => d.Name)
                .ToList();

            // Разделяем дисциплины на три категории: пройденные, текущие и будущие
            var completedDiscs = groupDisciplines.Where(d => d.Semester < CurrentSemester).ToList();
            
            // Группируем пройденные предметы по семестрам с оценками
            // Каждый семестр отображается отдельной группой
            var completedGrouped = completedDiscs
                .GroupBy(d => d.Semester)
                .OrderBy(g => g.Key)
                .Select(g => new SemesterGroup(g.Key, 
                    g.OrderBy(d => d.Name)
                     .Select(d => new DisciplineWithGrade(d, GetGradeForDiscipline(d.Id)))))
                .ToList();
            
            CompletedDisciplines = new ObservableCollection<SemesterGroup>(completedGrouped);

            // Текущие дисциплины - только те, что в текущем семестре
            CurrentDisciplines = new ObservableCollection<DisciplineWithGrade>(
                groupDisciplines.Where(d => d.Semester == CurrentSemester)
                    .Select(d => new DisciplineWithGrade(d, GetGradeForDiscipline(d.Id)))
            );

            // Будущие дисциплины - те, что в семестрах после текущего
            FutureDisciplines = new ObservableCollection<DisciplineWithGrade>(
                groupDisciplines.Where(d => d.Semester > CurrentSemester)
                    .Select(d => new DisciplineWithGrade(d, GetGradeForDiscipline(d.Id)))
            );
        }
    }
}
