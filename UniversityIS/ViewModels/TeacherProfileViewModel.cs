using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using UniversityIS.Models;
using UniversityIS.Services;

namespace UniversityIS.ViewModels
{
    // ViewModel для профиля преподавателя
    // Управляет списком дисциплин, которые ведет преподаватель
    // Позволяет добавлять и удалять дисциплины из профиля преподавателя
    public class TeacherProfileViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private readonly Teacher _teacher;
        private Discipline? _selectedDisciplineToAdd;
        private TeacherDiscipline? _selectedTeacherDiscipline;
        private string _errorMessage = string.Empty;
        private ObservableCollection<Discipline> _availableDisciplines = new();
        private ObservableCollection<Discipline> _teacherDisciplines = new();

        public TeacherProfileViewModel(DataService dataService, Teacher teacher)
        {
            _dataService = dataService;
            _teacher = teacher;

            AddDisciplineCommand = ReactiveCommand.Create(AddDiscipline, outputScheduler: RxApp.MainThreadScheduler);
            RemoveDisciplineCommand = ReactiveCommand.Create(RemoveDiscipline, outputScheduler: RxApp.MainThreadScheduler);

            // Подписываемся на изменения для обновления списков
            _dataService.TeacherDisciplines.CollectionChanged += (s, e) => LoadTeacherDisciplines();
            _dataService.Disciplines.CollectionChanged += (s, e) => LoadAvailableDisciplines();

            LoadTeacherDisciplines();
            LoadAvailableDisciplines();
        }

        public string TeacherName => _teacher.FullName;
        public string Position => _teacher.Position.ToString();
        public string Department
        {
            get
            {
                var dept = _dataService.GetDepartment(_teacher.DepartmentId);
                return dept?.Name ?? "Кафедра не найдена";
            }
        }

        public ObservableCollection<Discipline> AvailableDisciplines
        {
            get => _availableDisciplines;
            set => this.RaiseAndSetIfChanged(ref _availableDisciplines, value);
        }

        public ObservableCollection<Discipline> TeacherDisciplines
        {
            get => _teacherDisciplines;
            set => this.RaiseAndSetIfChanged(ref _teacherDisciplines, value);
        }

        public Discipline? SelectedDisciplineToAdd
        {
            get => _selectedDisciplineToAdd;
            set => this.RaiseAndSetIfChanged(ref _selectedDisciplineToAdd, value);
        }

        public TeacherDiscipline? SelectedTeacherDiscipline
        {
            get => _selectedTeacherDiscipline;
            set => this.RaiseAndSetIfChanged(ref _selectedTeacherDiscipline, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public ReactiveCommand<Unit, Unit> AddDisciplineCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveDisciplineCommand { get; }

        private void LoadTeacherDisciplines()
        {
            var disciplineIds = _dataService.TeacherDisciplines
                .Where(td => td.TeacherId == _teacher.Id)
                .Select(td => td.DisciplineId)
                .ToList();

            var disciplines = disciplineIds
                .Select(id => _dataService.GetDiscipline(id))
                .Where(d => d != null)
                .OrderBy(d => d!.Name)
                .ToList();

            TeacherDisciplines = new ObservableCollection<Discipline>(disciplines!);
            LoadAvailableDisciplines();
        }

        private void LoadAvailableDisciplines()
        {
            var teacherDisciplineIds = _dataService.TeacherDisciplines
                .Where(td => td.TeacherId == _teacher.Id)
                .Select(td => td.DisciplineId)
                .ToHashSet();

            var available = _dataService.Disciplines
                .Where(d => !teacherDisciplineIds.Contains(d.Id))
                .OrderBy(d => d.Name)
                .ToList();

            AvailableDisciplines = new ObservableCollection<Discipline>(available);
        }

        private void AddDiscipline()
        {
            ErrorMessage = string.Empty;

            if (SelectedDisciplineToAdd == null)
            {
                ErrorMessage = "Выберите дисциплину для добавления.";
                return;
            }

            // Проверяем, не добавлена ли уже эта дисциплина
            var exists = _dataService.TeacherDisciplines.Any(td =>
                td.TeacherId == _teacher.Id &&
                td.DisciplineId == SelectedDisciplineToAdd.Id);

            if (exists)
            {
                ErrorMessage = "Эта дисциплина уже добавлена для данного преподавателя.";
                return;
            }

            var teacherDiscipline = new TeacherDiscipline
            {
                TeacherId = _teacher.Id,
                DisciplineId = SelectedDisciplineToAdd.Id
            };

            _dataService.TeacherDisciplines.Add(teacherDiscipline);
            SelectedDisciplineToAdd = null;
        }

        private void RemoveDiscipline()
        {
            ErrorMessage = string.Empty;

            if (SelectedTeacherDiscipline == null)
            {
                ErrorMessage = "Выберите дисциплину для удаления.";
                return;
            }

            var toRemove = _dataService.TeacherDisciplines.FirstOrDefault(td =>
                td.TeacherId == _teacher.Id &&
                td.DisciplineId == SelectedTeacherDiscipline.Id);

            if (toRemove != null)
            {
                _dataService.TeacherDisciplines.Remove(toRemove);
                SelectedTeacherDiscipline = null;
            }
        }
    }
}
