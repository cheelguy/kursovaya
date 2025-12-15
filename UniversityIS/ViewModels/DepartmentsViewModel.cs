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
    // Группа кафедр, объединенных по факультету
    // Используется для отображения кафедр с группировкой по факультетам
    public class FacultyGroup
    {
        public string FacultyName { get; set; } = string.Empty;
        public ObservableCollection<Department> Departments { get; set; } = new();
    }

    // ViewModel для управления кафедрами
    // Позволяет добавлять, редактировать и удалять кафедры
    // Группирует кафедры по факультетам для удобного отображения
    public class DepartmentsViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private Department? _selectedDepartment;
        private string _name = string.Empty;
        private string _head = string.Empty;
        private Faculty? _selectedFaculty;
        private string _errorMessage = string.Empty;
        private ObservableCollection<FacultyGroup> _groupedDepartments = new();
        
        public DepartmentsViewModel(DataService dataService)
        {
            _dataService = dataService;
            
            AddCommand = ReactiveCommand.Create(AddDepartment, outputScheduler: RxApp.MainThreadScheduler);
            UpdateCommand = ReactiveCommand.Create(UpdateDepartment, outputScheduler: RxApp.MainThreadScheduler);
            DeleteCommand = ReactiveCommand.Create(DeleteDepartment, outputScheduler: RxApp.MainThreadScheduler);
            
            // Подписываемся на изменения в коллекциях для обновления группировки
            _dataService.Departments.CollectionChanged += (s, e) => UpdateGroupedDepartments();
            _dataService.Faculties.CollectionChanged += (s, e) => UpdateGroupedDepartments();
            
            UpdateGroupedDepartments();
        }
        
        public ObservableCollection<Department> Departments => _dataService.Departments;
        public ObservableCollection<Faculty> Faculties => _dataService.Faculties;

        public ObservableCollection<FacultyGroup> GroupedDepartments
        {
            get => _groupedDepartments;
            set => this.RaiseAndSetIfChanged(ref _groupedDepartments, value);
        }
        
        public Department? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedDepartment, value);
                if (value != null)
                {
                    Name = value.Name;
                    Head = value.Head;
                    SelectedFaculty = _dataService.GetFaculty(value.FacultyId);
                }
            }
        }
        
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        public string Head
        {
            get => _head;
            set => this.RaiseAndSetIfChanged(ref _head, value);
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
        
        private void AddDepartment()
        {
            ErrorMessage = string.Empty;

            // Валидация факультета
            if (SelectedFaculty == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Факультет", "not_selected");
                return;
            }

            // Валидация названия кафедры
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название кафедры", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название кафедры", "invalid_name");
                return;
            }

            // Валидация ФИО заведующего
            if (string.IsNullOrWhiteSpace(Head))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("ФИО заведующего", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(Head))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("ФИО заведующего", "invalid_name");
                return;
            }
            
            var department = new Department
            {
                Name = Name,
                Head = Head,
                FacultyId = SelectedFaculty.Id
            };
            
            _dataService.Departments.Add(department);
            
            ClearFields();
        }
        
        private void UpdateDepartment()
        {
            ErrorMessage = string.Empty;

            if (SelectedDepartment == null)
            {
                ErrorMessage = "Выберите кафедру для обновления.";
                return;
            }

            // Валидация факультета
            if (SelectedFaculty == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Факультет", "not_selected");
                return;
            }

            // Валидация названия кафедры
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название кафедры", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название кафедры", "invalid_name");
                return;
            }

            // Валидация ФИО заведующего
            if (string.IsNullOrWhiteSpace(Head))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("ФИО заведующего", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(Head))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("ФИО заведующего", "invalid_name");
                return;
            }
            
            SelectedDepartment.Name = Name;
            SelectedDepartment.Head = Head;
            SelectedDepartment.FacultyId = SelectedFaculty.Id;
            
            var index = Departments.IndexOf(SelectedDepartment);
            if (index >= 0)
            {
                Departments[index] = SelectedDepartment;
            }
        }
        
        private void DeleteDepartment()
        {
            if (SelectedDepartment == null) return;
            
            _dataService.Departments.Remove(SelectedDepartment);
            Departments.Remove(SelectedDepartment);
            
            ClearFields();
        }
        
        private void ClearFields()
        {
            Name = string.Empty;
            Head = string.Empty;
            SelectedFaculty = null;
            SelectedDepartment = null;
            ErrorMessage = string.Empty;
        }

        private void UpdateGroupedDepartments()
        {
            var groups = new ObservableCollection<FacultyGroup>();
            
            // Группируем кафедры по факультетам
            var departmentsByFaculty = _dataService.Departments
                .GroupBy(d => d.FacultyId)
                .OrderBy(g => _dataService.GetFaculty(g.Key)?.Name ?? "Неизвестный факультет");
            
            foreach (var group in departmentsByFaculty)
            {
                var faculty = _dataService.GetFaculty(group.Key);
                var facultyGroup = new FacultyGroup
                {
                    FacultyName = faculty?.Name ?? "Неизвестный факультет",
                    Departments = new ObservableCollection<Department>(group.OrderBy(d => d.Name))
                };
                groups.Add(facultyGroup);
            }
            
            GroupedDepartments = groups;
        }
    }
}
