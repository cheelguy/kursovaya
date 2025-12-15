using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using ReactiveUI;
using UniversityIS.Models;
using UniversityIS.Services;
using UniversityIS.Helpers;

namespace UniversityIS.ViewModels
{
    // ViewModel для управления учебной нагрузкой преподавателей
    // Позволяет назначать преподавателям дисциплины для конкретных групп
    // Валидирует правила: один преподаватель на лекции/семинары, несколько на лабораторные
    public class WorkLoadsViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private WorkLoad? _selectedWorkLoad;
        private Teacher? _selectedTeacher;
        private Discipline? _selectedDiscipline;
        private Models.Group? _selectedGroup;
        private LessonType _lessonType = LessonType.Lecture;
        private int _hours = 0;
        private string _academicYear = $"{DateTime.Now.Year}/{DateTime.Now.Year + 1}";
        private int _semester = 1;
        private EnumItem<LessonType>? _selectedLessonTypeItem;
        private string _errorMessage = string.Empty;
        private ObservableCollection<Discipline> _availableDisciplines = new();
        
        public WorkLoadsViewModel(DataService dataService)
        {
            _dataService = dataService;
            LessonTypes = new ObservableCollection<EnumItem<LessonType>>(EnumHelper.GetLessonTypes());
            
            AddCommand = ReactiveCommand.Create(AddWorkLoad, outputScheduler: RxApp.MainThreadScheduler);
            UpdateCommand = ReactiveCommand.Create(UpdateWorkLoad, outputScheduler: RxApp.MainThreadScheduler);
            DeleteCommand = ReactiveCommand.Create(DeleteWorkLoad, outputScheduler: RxApp.MainThreadScheduler);

            // Подписываемся на изменения для обновления списка дисциплин
            _dataService.TeacherDisciplines.CollectionChanged += (s, e) => UpdateAvailableDisciplines();
            this.WhenAnyValue(x => x.SelectedTeacher).Subscribe(_ => UpdateAvailableDisciplines());
            
            UpdateAvailableDisciplines();
        }
        
        public ObservableCollection<WorkLoad> WorkLoads => _dataService.WorkLoads;
        public ObservableCollection<Teacher> Teachers => _dataService.Teachers;
        public ObservableCollection<Discipline> AvailableDisciplines
        {
            get => _availableDisciplines;
            set => this.RaiseAndSetIfChanged(ref _availableDisciplines, value);
        }
        public ObservableCollection<Models.Group> Groups => _dataService.Groups;
        public ObservableCollection<EnumItem<LessonType>> LessonTypes { get; }
        
        public WorkLoad? SelectedWorkLoad
        {
            get => _selectedWorkLoad;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedWorkLoad, value);
                if (value != null)
                {
                    SelectedTeacher = _dataService.GetTeacher(value.TeacherId);
                    SelectedDiscipline = _dataService.GetDiscipline(value.DisciplineId);
                    SelectedGroup = _dataService.GetGroup(value.GroupId);
                    LessonType = value.LessonType;
                    SelectedLessonTypeItem = LessonTypes.FirstOrDefault(x => x.Value == value.LessonType);
                    Hours = value.Hours;
                    AcademicYear = value.AcademicYear;
                    Semester = value.Semester;
                }
            }
        }
        
        public Teacher? SelectedTeacher
        {
            get => _selectedTeacher;
            set => this.RaiseAndSetIfChanged(ref _selectedTeacher, value);
        }
        
        public Discipline? SelectedDiscipline
        {
            get => _selectedDiscipline;
            set => this.RaiseAndSetIfChanged(ref _selectedDiscipline, value);
        }
        
        public Models.Group? SelectedGroup
        {
            get => _selectedGroup;
            set => this.RaiseAndSetIfChanged(ref _selectedGroup, value);
        }
        
        public LessonType LessonType
        {
            get => _lessonType;
            set => this.RaiseAndSetIfChanged(ref _lessonType, value);
        }
        
        public EnumItem<LessonType>? SelectedLessonTypeItem
        {
            get => _selectedLessonTypeItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedLessonTypeItem, value);
                if (value != null)
                    LessonType = value.Value;
            }
        }
        
        public int Hours
        {
            get => _hours;
            set => this.RaiseAndSetIfChanged(ref _hours, value);
        }
        
        public string AcademicYear
        {
            get => _academicYear;
            set => this.RaiseAndSetIfChanged(ref _academicYear, value);
        }
        
        public int Semester
        {
            get => _semester;
            set => this.RaiseAndSetIfChanged(ref _semester, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }
        
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        
        private void AddWorkLoad()
        {
            ErrorMessage = string.Empty;

            // Валидация преподавателя
            if (SelectedTeacher == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Преподаватель", "not_selected");
                return;
            }

            // Валидация дисциплины
            if (SelectedDiscipline == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Дисциплина", "not_selected");
                return;
            }

            // Валидация группы
            if (SelectedGroup == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Группа", "not_selected");
                return;
            }

            // Валидация часов
            if (!ValidationHelper.IsValidHours(Hours.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Часы", "invalid_hours");
                return;
            }

            // Валидация учебного года (формат: 2023/2024)
            if (!Regex.IsMatch(AcademicYear, @"^\d{4}/\d{4}$"))
            {
                ErrorMessage = "Поле \"Учебный год\" должно иметь формат ГГГГ/ГГГГ (например: 2023/2024).";
                return;
            }

            // Валидация семестра
            if (Semester < 1 || Semester > 12)
            {
                ErrorMessage = "Поле \"Семестр\" должно быть числом от 1 до 12.";
                return;
            }

            // Проверка: дисциплина должна быть у выбранной группы
            if (SelectedDiscipline.GroupId != SelectedGroup.Id)
            {
                ErrorMessage = "Выбранная дисциплина не относится к данной группе.";
                return;
            }

            // Проверка правила: один преподаватель на лекции/семинары
            if (LessonType == LessonType.Lecture || LessonType == LessonType.Seminar)
            {
                var existingWorkLoad = _dataService.WorkLoads.FirstOrDefault(wl =>
                    wl.DisciplineId == SelectedDiscipline.Id &&
                    wl.GroupId == SelectedGroup.Id &&
                    wl.Semester == Semester &&
                    (wl.LessonType == LessonType.Lecture || wl.LessonType == LessonType.Seminar) &&
                    wl.TeacherId != SelectedTeacher.Id);

                if (existingWorkLoad != null)
                {
                    var lessonTypeName = LessonType == LessonType.Lecture ? "лекции" : "семинары";
                    ErrorMessage = $"У этой группы уже есть преподаватель на {lessonTypeName} по данной дисциплине.";
                    return;
                }
            }
            
            var workLoad = new WorkLoad
            {
                TeacherId = SelectedTeacher.Id,
                DisciplineId = SelectedDiscipline.Id,
                GroupId = SelectedGroup.Id,
                LessonType = LessonType,
                Hours = Hours,
                AcademicYear = AcademicYear,
                Semester = Semester
            };
            
            _dataService.WorkLoads.Add(workLoad);
            
            ClearFields();
        }
        
        private void UpdateWorkLoad()
        {
            ErrorMessage = string.Empty;

            if (SelectedWorkLoad == null)
            {
                ErrorMessage = "Выберите нагрузку для обновления.";
                return;
            }

            // Валидация преподавателя
            if (SelectedTeacher == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Преподаватель", "not_selected");
                return;
            }

            // Валидация дисциплины
            if (SelectedDiscipline == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Дисциплина", "not_selected");
                return;
            }

            // Валидация группы
            if (SelectedGroup == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Группа", "not_selected");
                return;
            }

            // Валидация часов
            if (!ValidationHelper.IsValidHours(Hours.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Часы", "invalid_hours");
                return;
            }

            // Валидация учебного года (формат: 2023/2024)
            if (!Regex.IsMatch(AcademicYear, @"^\d{4}/\d{4}$"))
            {
                ErrorMessage = "Поле \"Учебный год\" должно иметь формат ГГГГ/ГГГГ (например: 2023/2024).";
                return;
            }

            // Валидация семестра
            if (Semester < 1 || Semester > 12)
            {
                ErrorMessage = "Поле \"Семестр\" должно быть числом от 1 до 12.";
                return;
            }

            // Проверка: дисциплина должна быть у выбранной группы
            if (SelectedDiscipline.GroupId != SelectedGroup.Id)
            {
                ErrorMessage = "Выбранная дисциплина не относится к данной группе.";
                return;
            }

            // Проверка правила: один преподаватель на лекции/семинары
            if (LessonType == LessonType.Lecture || LessonType == LessonType.Seminar)
            {
                var existingWorkLoad = _dataService.WorkLoads.FirstOrDefault(wl =>
                    wl.Id != SelectedWorkLoad.Id && // Исключаем текущую запись
                    wl.DisciplineId == SelectedDiscipline.Id &&
                    wl.GroupId == SelectedGroup.Id &&
                    wl.Semester == Semester &&
                    (wl.LessonType == LessonType.Lecture || wl.LessonType == LessonType.Seminar) &&
                    wl.TeacherId != SelectedTeacher.Id);

                if (existingWorkLoad != null)
                {
                    var lessonTypeName = LessonType == LessonType.Lecture ? "лекции" : "семинары";
                    ErrorMessage = $"У этой группы уже есть преподаватель на {lessonTypeName} по данной дисциплине.";
                    return;
                }
            }
            
            SelectedWorkLoad.TeacherId = SelectedTeacher.Id;
            SelectedWorkLoad.DisciplineId = SelectedDiscipline.Id;
            SelectedWorkLoad.GroupId = SelectedGroup.Id;
            SelectedWorkLoad.LessonType = LessonType;
            SelectedWorkLoad.Hours = Hours;
            SelectedWorkLoad.AcademicYear = AcademicYear;
            SelectedWorkLoad.Semester = Semester;
            
            var index = WorkLoads.IndexOf(SelectedWorkLoad);
            if (index >= 0)
            {
                WorkLoads[index] = SelectedWorkLoad;
            }
        }

        // Обновляет список доступных дисциплин в зависимости от выбранного преподавателя
        // Показывает только те дисциплины, которые преподаватель может вести
        // Вызывается автоматически при изменении выбранного преподавателя
        private void UpdateAvailableDisciplines()
        {
            if (SelectedTeacher == null)
            {
                AvailableDisciplines = new ObservableCollection<Discipline>();
                return;
            }

            // Получаем идентификаторы дисциплин, которые ведет выбранный преподаватель
            var teacherDisciplineIds = _dataService.TeacherDisciplines
                .Where(td => td.TeacherId == SelectedTeacher.Id)
                .Select(td => td.DisciplineId)
                .ToHashSet();

            // Фильтруем все дисциплины, оставляя только те, что есть у преподавателя
            var disciplines = _dataService.Disciplines
                .Where(d => teacherDisciplineIds.Contains(d.Id))
                .OrderBy(d => d.Name)
                .ToList();

            AvailableDisciplines = new ObservableCollection<Discipline>(disciplines);
        }
        
        // Удаляет выбранную учебную нагрузку из базы данных
        private void DeleteWorkLoad()
        {
            if (SelectedWorkLoad == null) return;
            
            _dataService.WorkLoads.Remove(SelectedWorkLoad);
            WorkLoads.Remove(SelectedWorkLoad);
            
            ClearFields();
        }
        
        private void ClearFields()
        {
            SelectedTeacher = null;
            SelectedDiscipline = null;
            SelectedGroup = null;
            LessonType = LessonType.Lecture;
            Hours = 0;
            AcademicYear = $"{DateTime.Now.Year}/{DateTime.Now.Year + 1}";
            Semester = 1;
            SelectedWorkLoad = null;
            ErrorMessage = string.Empty;
        }
    }
}
