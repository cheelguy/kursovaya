using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using UniversityIS.Models;
using UniversityIS.Services;
using UniversityIS.Helpers;

namespace UniversityIS.ViewModels
{
    // Группа учебных групп, объединенных по курсу
    // Используется для отображения групп с группировкой по курсам
    public class CourseGrouping
    {
        public CourseGrouping(int course, IEnumerable<Group> groups)
        {
            Course = course;
            Groups = new ObservableCollection<Group>(groups);
        }

        public int Course { get; }
        public string CourseName => $"{Course} курс";
        public ObservableCollection<Group> Groups { get; }
    }

    // Группа учебных групп, объединенных по факультету
    // Содержит вложенные группы по курсам
    public class FacultyGrouping
    {
        public FacultyGrouping(string facultyName, IEnumerable<CourseGrouping> courses)
        {
            FacultyName = facultyName;
            Courses = new ObservableCollection<CourseGrouping>(courses);
        }

        public string FacultyName { get; }
        public ObservableCollection<CourseGrouping> Courses { get; }
    }

    // ViewModel для управления учебными группами
    // Позволяет добавлять, редактировать и удалять группы
    // Группирует группы по факультетам и курсам для удобного отображения
    public class GroupsViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private Group? _selectedGroup;
        private string _number = string.Empty;
        private int _yearOfAdmission = DateTime.Now.Year;
        private int _course = 1;
        private Faculty? _selectedFaculty;
        private string _errorMessage = string.Empty;
        private ObservableCollection<FacultyGrouping>? _groupedGroups;
        
        public GroupsViewModel(DataService dataService)
        {
            _dataService = dataService;
            
            // Подписываемся на изменения для обновления группировки
            _dataService.Groups.CollectionChanged += (s, e) => UpdateGroupedGroups();
            _dataService.Faculties.CollectionChanged += (s, e) => UpdateGroupedGroups();
            
            UpdateGroupedGroups(); // Первоначальная группировка
            
            AddCommand = ReactiveCommand.Create(AddGroup, outputScheduler: RxApp.MainThreadScheduler);
            UpdateCommand = ReactiveCommand.Create(UpdateGroup, outputScheduler: RxApp.MainThreadScheduler);
            DeleteCommand = ReactiveCommand.Create(DeleteGroup, outputScheduler: RxApp.MainThreadScheduler);
        }
        
        public ObservableCollection<Group> Groups => _dataService.Groups;
        public ObservableCollection<Faculty> Faculties => _dataService.Faculties;

        public ObservableCollection<FacultyGrouping>? GroupedGroups
        {
            get => _groupedGroups;
            set => this.RaiseAndSetIfChanged(ref _groupedGroups, value);
        }

        private void UpdateGroupedGroups()
        {
            var grouped = _dataService.Groups
                .GroupBy(g => g.FacultyId)
                .Select(facultyGroup =>
                {
                    var faculty = _dataService.GetFaculty(facultyGroup.Key);
                    var facultyName = faculty?.Name ?? "Неизвестный факультет";

                    var courseGroups = facultyGroup
                        .GroupBy(g => g.Course)
                        .OrderBy(cg => cg.Key)
                        .Select(courseGroup => new CourseGrouping(courseGroup.Key, courseGroup.OrderBy(g => g.Number)))
                        .ToList();

                    return new FacultyGrouping(facultyName, courseGroups);
                })
                .OrderBy(fg => fg.FacultyName)
                .ToList();

            GroupedGroups = new ObservableCollection<FacultyGrouping>(grouped);
        }
        
        public Group? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedGroup, value);
                if (value != null)
                {
                    Number = value.Number;
                    YearOfAdmission = value.YearOfAdmission;
                    Course = value.Course;
                    SelectedFaculty = _dataService.GetFaculty(value.FacultyId);
                }
            }
        }
        
        public string Number
        {
            get => _number;
            set => this.RaiseAndSetIfChanged(ref _number, value);
        }
        
        public int YearOfAdmission
        {
            get => _yearOfAdmission;
            set => this.RaiseAndSetIfChanged(ref _yearOfAdmission, value);
        }
        
        public int Course
        {
            get => _course;
            set => this.RaiseAndSetIfChanged(ref _course, value);
        }
        
        public Faculty? SelectedFaculty
        {
            get => _selectedFaculty;
            set => this.RaiseAndSetIfChanged(ref _selectedFaculty, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }
        
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        
        private void AddGroup()
        {
            ErrorMessage = string.Empty;

            // Валидация факультета
            if (SelectedFaculty == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Факультет", "not_selected");
                return;
            }

            // Валидация номера группы
            if (string.IsNullOrWhiteSpace(Number))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Номер группы", "empty");
                return;
            }

            if (!ValidationHelper.IsValidNameWithNumbers(Number))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Номер группы", "invalid_name_with_numbers");
                return;
            }

            // Валидация года поступления
            if (!ValidationHelper.IsValidYear(YearOfAdmission.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Год поступления", "invalid_year");
                return;
            }

            // Валидация курса
            if (!ValidationHelper.IsValidCourse(Course.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Курс", "invalid_course");
                return;
            }
            
            var group = new Group
            {
                Number = Number,
                YearOfAdmission = YearOfAdmission,
                Course = Course,
                FacultyId = SelectedFaculty.Id
            };
            
            _dataService.Groups.Add(group);
            
            ClearFields();
        }
        
        private void UpdateGroup()
        {
            ErrorMessage = string.Empty;

            if (SelectedGroup == null)
            {
                ErrorMessage = "Выберите группу для обновления.";
                return;
            }

            // Валидация факультета
            if (SelectedFaculty == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Факультет", "not_selected");
                return;
            }

            // Валидация номера группы
            if (string.IsNullOrWhiteSpace(Number))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Номер группы", "empty");
                return;
            }

            if (!ValidationHelper.IsValidNameWithNumbers(Number))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Номер группы", "invalid_name_with_numbers");
                return;
            }

            // Валидация года поступления
            if (!ValidationHelper.IsValidYear(YearOfAdmission.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Год поступления", "invalid_year");
                return;
            }

            // Валидация курса
            if (!ValidationHelper.IsValidCourse(Course.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Курс", "invalid_course");
                return;
            }
            
            SelectedGroup.Number = Number;
            SelectedGroup.YearOfAdmission = YearOfAdmission;
            SelectedGroup.Course = Course;
            SelectedGroup.FacultyId = SelectedFaculty.Id;
            
            var index = Groups.IndexOf(SelectedGroup);
            if (index >= 0)
            {
                Groups[index] = SelectedGroup;
            }
        }
        
        private void DeleteGroup()
        {
            if (SelectedGroup == null) return;
            
            _dataService.Groups.Remove(SelectedGroup);
            Groups.Remove(SelectedGroup);
            
            ClearFields();
        }
        
        private void ClearFields()
        {
            Number = string.Empty;
            YearOfAdmission = DateTime.Now.Year;
            Course = 1;
            SelectedFaculty = null;
            SelectedGroup = null;
            ErrorMessage = string.Empty;
        }
    }
}
