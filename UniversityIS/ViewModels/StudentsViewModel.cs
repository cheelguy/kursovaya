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
    // Группа студентов, объединенных по учебной группе
    // Используется для отображения студентов с группировкой по группам
    public class StudentGroupGrouping
    {
        public StudentGroupGrouping(Models.Group group, IEnumerable<Student> students)
        {
            Group = group;
            Students = new ObservableCollection<Student>(students);
        }

        public Models.Group Group { get; }
        public string GroupName => Group.Number;
        public ObservableCollection<Student> Students { get; }
    }

    // Группа студентов, объединенных по факультету
    // Содержит вложенные группы по учебным группам
    public class StudentFacultyGrouping
    {
        public StudentFacultyGrouping(string facultyName, IEnumerable<StudentGroupGrouping> groups)
        {
            FacultyName = facultyName;
            Groups = new ObservableCollection<StudentGroupGrouping>(groups);
        }

        public string FacultyName { get; }
        public ObservableCollection<StudentGroupGrouping> Groups { get; }
    }

    // ViewModel для управления студентами
    // Позволяет добавлять, редактировать и удалять студентов
    // Группирует студентов по факультетам и группам для удобного отображения
    public class StudentsViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private readonly Action<Student>? _showProfileAction;
        private Student? _selectedStudent;
        private string _lastName = string.Empty;
        private string _firstName = string.Empty;
        private string _middleName = string.Empty;
        private string _recordBookNumber = string.Empty;
        private double _gpa = 0.0;
        private Group? _selectedGroup;
        private string _errorMessage = string.Empty;
        private ObservableCollection<StudentFacultyGrouping>? _groupedStudents;
        
        public StudentsViewModel(DataService dataService, Action<Student>? showProfileAction = null)
        {
            _dataService = dataService;
            _showProfileAction = showProfileAction;
            
            // Подписываемся на изменения для обновления группировки
            _dataService.Students.CollectionChanged += (s, e) => UpdateGroupedStudents();
            _dataService.Groups.CollectionChanged += (s, e) => UpdateGroupedStudents();
            _dataService.Faculties.CollectionChanged += (s, e) => UpdateGroupedStudents();
            
            UpdateGroupedStudents(); // Первоначальная группировка
            
            AddCommand = ReactiveCommand.Create(AddStudent, outputScheduler: RxApp.MainThreadScheduler);
            UpdateCommand = ReactiveCommand.Create(UpdateStudent, outputScheduler: RxApp.MainThreadScheduler);
            DeleteCommand = ReactiveCommand.Create(DeleteStudent, outputScheduler: RxApp.MainThreadScheduler);
            ShowProfileCommand = ReactiveCommand.Create(ShowProfile, outputScheduler: RxApp.MainThreadScheduler);
        }
        
        public ObservableCollection<Student> Students => _dataService.Students;
        public ObservableCollection<Group> Groups => _dataService.Groups;

        public ObservableCollection<StudentFacultyGrouping>? GroupedStudents
        {
            get => _groupedStudents;
            set => this.RaiseAndSetIfChanged(ref _groupedStudents, value);
        }

        // Обновляет группировку студентов по факультетам и группам
        // Вызывается автоматически при изменении данных студентов, групп или факультетов
        private void UpdateGroupedStudents()
        {
            var grouped = _dataService.Students
                .GroupBy(s => s.GroupId)
                .Select(groupStudents =>
                {
                    var group = _dataService.GetGroup(groupStudents.Key);
                    if (group == null) return null;

                    var faculty = _dataService.GetFaculty(group.FacultyId);
                    var facultyName = faculty?.Name ?? "Неизвестный факультет";

                    return new
                    {
                        FacultyName = facultyName,
                        Group = group,
                        Students = groupStudents.OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
                    };
                })
                .Where(x => x != null)
                .GroupBy(x => x!.FacultyName)
                .Select(facultyGroup =>
                {
                    var groupGroups = facultyGroup
                        .OrderBy(x => x!.Group.Number)
                        .Select(x => new StudentGroupGrouping(x!.Group, x.Students))
                        .ToList();

                    return new StudentFacultyGrouping(facultyGroup.Key, groupGroups);
                })
                .OrderBy(fg => fg.FacultyName)
                .ToList();

            GroupedStudents = new ObservableCollection<StudentFacultyGrouping>(grouped);
        }
        
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedStudent, value);
                if (value != null)
                {
                    LastName = value.LastName;
                    FirstName = value.FirstName;
                    MiddleName = value.MiddleName;
                    RecordBookNumber = value.RecordBookNumber;
                    GPA = value.GPA;
                    SelectedGroup = _dataService.GetGroup(value.GroupId);
                }
            }
        }
        
        public string LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }
        
        public string FirstName
        {
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }
        
        public string MiddleName
        {
            get => _middleName;
            set => this.RaiseAndSetIfChanged(ref _middleName, value);
        }
        
        public string RecordBookNumber
        {
            get => _recordBookNumber;
            set => this.RaiseAndSetIfChanged(ref _recordBookNumber, value);
        }
        
        public double GPA
        {
            get => _gpa;
            set => this.RaiseAndSetIfChanged(ref _gpa, value);
        }
        
        public Group? SelectedGroup
        {
            get => _selectedGroup;
            set => this.RaiseAndSetIfChanged(ref _selectedGroup, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }
        
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowProfileCommand { get; }
        
        private void AddStudent()
        {
            ErrorMessage = string.Empty;

            // Валидация группы
            if (SelectedGroup == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Группа", "not_selected");
                return;
            }

            // Валидация фамилии
            if (string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Фамилия", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(LastName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Фамилия", "invalid_name");
                return;
            }

            // Валидация имени
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Имя", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(FirstName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Имя", "invalid_name");
                return;
            }

            // Валидация отчества (может быть пустым)
            if (!string.IsNullOrWhiteSpace(MiddleName) && !ValidationHelper.IsValidName(MiddleName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Отчество", "invalid_name");
                return;
            }
            
            var student = new Student
            {
                LastName = LastName,
                FirstName = FirstName,
                MiddleName = MiddleName,
                RecordBookNumber = RecordBookNumber,
                GPA = GPA,
                GroupId = SelectedGroup.Id
            };
            
            _dataService.Students.Add(student);
            
            ClearFields();
        }
        
        private void UpdateStudent()
        {
            ErrorMessage = string.Empty;

            if (SelectedStudent == null)
            {
                ErrorMessage = "Выберите студента для обновления.";
                return;
            }

            // Валидация группы
            if (SelectedGroup == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Группа", "not_selected");
                return;
            }

            // Валидация фамилии
            if (string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Фамилия", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(LastName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Фамилия", "invalid_name");
                return;
            }

            // Валидация имени
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Имя", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(FirstName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Имя", "invalid_name");
                return;
            }

            // Валидация отчества (может быть пустым)
            if (!string.IsNullOrWhiteSpace(MiddleName) && !ValidationHelper.IsValidName(MiddleName))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Отчество", "invalid_name");
                return;
            }
            
            SelectedStudent.LastName = LastName;
            SelectedStudent.FirstName = FirstName;
            SelectedStudent.MiddleName = MiddleName;
            SelectedStudent.RecordBookNumber = RecordBookNumber;
            SelectedStudent.GPA = GPA;
            SelectedStudent.GroupId = SelectedGroup.Id;
            
            var index = Students.IndexOf(SelectedStudent);
            if (index >= 0)
            {
                Students[index] = SelectedStudent;
            }
        }
        
        private void DeleteStudent()
        {
            if (SelectedStudent == null) return;
            
            _dataService.Students.Remove(SelectedStudent);
            Students.Remove(SelectedStudent);
            
            ClearFields();
        }
        
        private void ClearFields()
        {
            LastName = string.Empty;
            FirstName = string.Empty;
            MiddleName = string.Empty;
            RecordBookNumber = string.Empty;
            GPA = 0.0;
            SelectedGroup = null;
            SelectedStudent = null;
            ErrorMessage = string.Empty;
        }

        private void ShowProfile()
        {
            if (SelectedStudent == null)
            {
                ErrorMessage = "Выберите студента для просмотра профиля.";
                return;
            }

            _showProfileAction?.Invoke(SelectedStudent);
        }
    }
}
