using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using UniversityIS.Models;
using UniversityIS.Services;
using UniversityIS.Helpers;

namespace UniversityIS.ViewModels
{
    // ViewModel для управления преподавателями
    // Позволяет добавлять, редактировать и удалять преподавателей
    // Управляет информацией о должности, степени, звании и научной деятельности
    public class TeachersViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private readonly Action<Teacher>? _showProfileAction;
        private Teacher? _selectedTeacher;
        private string _lastName = string.Empty;
        private string _firstName = string.Empty;
        private string _middleName = string.Empty;
        private TeacherPosition _position = TeacherPosition.Assistant;
        private AcademicDegree _degree = AcademicDegree.None;
        private AcademicTitle _title = AcademicTitle.None;
        private bool _isPostgraduate = false;
        private bool _leadsResearchTopics = false;
        private bool _leadsResearchDirections = false;
        private Department? _selectedDepartment;
        private EnumItem<TeacherPosition>? _selectedPositionItem;
        private EnumItem<AcademicDegree>? _selectedDegreeItem;
        private EnumItem<AcademicTitle>? _selectedTitleItem;
        private string _errorMessage = string.Empty;
        
        public TeachersViewModel(DataService dataService, Action<Teacher>? showProfileAction = null)
        {
            _dataService = dataService;
            _showProfileAction = showProfileAction;
            
            Positions = new ObservableCollection<EnumItem<TeacherPosition>>(EnumHelper.GetTeacherPositions());
            Degrees = new ObservableCollection<EnumItem<AcademicDegree>>(EnumHelper.GetAcademicDegrees());
            Titles = new ObservableCollection<EnumItem<AcademicTitle>>(EnumHelper.GetAcademicTitles());
            
            AddCommand = ReactiveCommand.Create(AddTeacher, outputScheduler: RxApp.MainThreadScheduler);
            UpdateCommand = ReactiveCommand.Create(UpdateTeacher, outputScheduler: RxApp.MainThreadScheduler);
            DeleteCommand = ReactiveCommand.Create(DeleteTeacher, outputScheduler: RxApp.MainThreadScheduler);
            ShowProfileCommand = ReactiveCommand.Create(ShowProfile, outputScheduler: RxApp.MainThreadScheduler);
        }
        
        public ObservableCollection<Teacher> Teachers => _dataService.Teachers;
        public ObservableCollection<Department> Departments => _dataService.Departments;
        public ObservableCollection<EnumItem<TeacherPosition>> Positions { get; }
        public ObservableCollection<EnumItem<AcademicDegree>> Degrees { get; }
        public ObservableCollection<EnumItem<AcademicTitle>> Titles { get; }
        
        public Teacher? SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTeacher, value);
                if (value != null)
                {
                    LastName = value.LastName;
                    FirstName = value.FirstName;
                    MiddleName = value.MiddleName;
                    Position = value.Position;
                    Degree = value.Degree;
                    Title = value.Title;
                    SelectedPositionItem = Positions.FirstOrDefault(x => x.Value == value.Position);
                    SelectedDegreeItem = Degrees.FirstOrDefault(x => x.Value == value.Degree);
                    SelectedTitleItem = Titles.FirstOrDefault(x => x.Value == value.Title);
                    IsPostgraduate = value.IsPostgraduate;
                    LeadsResearchTopics = value.LeadsResearchTopics;
                    LeadsResearchDirections = value.LeadsResearchDirections;
                    SelectedDepartment = _dataService.GetDepartment(value.DepartmentId);
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
        
        public TeacherPosition Position
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }
        
        public AcademicDegree Degree
        {
            get => _degree;
            set => this.RaiseAndSetIfChanged(ref _degree, value);
        }
        
        public AcademicTitle Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
        
        public EnumItem<TeacherPosition>? SelectedPositionItem
        {
            get => _selectedPositionItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPositionItem, value);
                if (value != null)
                    Position = value.Value;
            }
        }
        
        public EnumItem<AcademicDegree>? SelectedDegreeItem
        {
            get => _selectedDegreeItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedDegreeItem, value);
                if (value != null)
                    Degree = value.Value;
            }
        }
        
        public EnumItem<AcademicTitle>? SelectedTitleItem
        {
            get => _selectedTitleItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTitleItem, value);
                if (value != null)
                    Title = value.Value;
            }
        }
        
        public bool IsPostgraduate
        {
            get => _isPostgraduate;
            set => this.RaiseAndSetIfChanged(ref _isPostgraduate, value);
        }
        
        public bool LeadsResearchTopics
        {
            get => _leadsResearchTopics;
            set => this.RaiseAndSetIfChanged(ref _leadsResearchTopics, value);
        }
        
        public bool LeadsResearchDirections
        {
            get => _leadsResearchDirections;
            set => this.RaiseAndSetIfChanged(ref _leadsResearchDirections, value);
        }
        
        public Department? SelectedDepartment
        {
            get => _selectedDepartment;
            set => this.RaiseAndSetIfChanged(ref _selectedDepartment, value);
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
        
        private void AddTeacher()
        {
            ErrorMessage = string.Empty;

            // Валидация кафедры
            if (SelectedDepartment == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Кафедра", "not_selected");
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
            
            var teacher = new Teacher
            {
                LastName = LastName,
                FirstName = FirstName,
                MiddleName = MiddleName,
                Position = Position,
                Degree = Degree,
                Title = Title,
                IsPostgraduate = IsPostgraduate,
                LeadsResearchTopics = LeadsResearchTopics,
                LeadsResearchDirections = LeadsResearchDirections,
                DepartmentId = SelectedDepartment.Id
            };
            
            _dataService.Teachers.Add(teacher);
            
            ClearFields();
        }
        
        private void UpdateTeacher()
        {
            ErrorMessage = string.Empty;

            if (SelectedTeacher == null)
            {
                ErrorMessage = "Выберите преподавателя для обновления.";
                return;
            }

            // Валидация кафедры
            if (SelectedDepartment == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Кафедра", "not_selected");
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
            
            SelectedTeacher.LastName = LastName;
            SelectedTeacher.FirstName = FirstName;
            SelectedTeacher.MiddleName = MiddleName;
            SelectedTeacher.Position = Position;
            SelectedTeacher.Degree = Degree;
            SelectedTeacher.Title = Title;
            SelectedTeacher.IsPostgraduate = IsPostgraduate;
            SelectedTeacher.LeadsResearchTopics = LeadsResearchTopics;
            SelectedTeacher.LeadsResearchDirections = LeadsResearchDirections;
            SelectedTeacher.DepartmentId = SelectedDepartment.Id;
            
            var index = Teachers.IndexOf(SelectedTeacher);
            if (index >= 0)
            {
                Teachers[index] = SelectedTeacher;
            }
        }
        
        private void DeleteTeacher()
        {
            if (SelectedTeacher == null) return;
            
            _dataService.Teachers.Remove(SelectedTeacher);
            Teachers.Remove(SelectedTeacher);
            
            ClearFields();
        }
        
        private void ClearFields()
        {
            LastName = string.Empty;
            FirstName = string.Empty;
            MiddleName = string.Empty;
            Position = TeacherPosition.Assistant;
            Degree = AcademicDegree.None;
            Title = AcademicTitle.None;
            IsPostgraduate = false;
            LeadsResearchTopics = false;
            LeadsResearchDirections = false;
            SelectedDepartment = null;
            SelectedTeacher = null;
            ErrorMessage = string.Empty;
        }

        private void ShowProfile()
        {
            if (SelectedTeacher == null)
            {
                ErrorMessage = "Выберите преподавателя для просмотра профиля.";
                return;
            }

            _showProfileAction?.Invoke(SelectedTeacher);
        }
    }
}
