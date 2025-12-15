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
    // ViewModel для управления дипломными работами
    // Позволяет добавлять, редактировать и удалять дипломные работы
    // Валидирует, что научный руководитель имеет право руководить дипломными работами
    public class ThesisWorksViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private ThesisWork? _selectedThesisWork;
        private string _title = string.Empty;
        private Student? _selectedStudent;
        private Teacher? _selectedSupervisor;
        private int _year = DateTime.Now.Year;
        private int? _grade;
        private string _errorMessage = string.Empty;
        
        public ThesisWorksViewModel(DataService dataService)
        {
            _dataService = dataService;
            
            AddCommand = ReactiveCommand.Create(AddThesisWork, outputScheduler: RxApp.MainThreadScheduler);
            UpdateCommand = ReactiveCommand.Create(UpdateThesisWork, outputScheduler: RxApp.MainThreadScheduler);
            DeleteCommand = ReactiveCommand.Create(DeleteThesisWork, outputScheduler: RxApp.MainThreadScheduler);
        }
        
        public ObservableCollection<ThesisWork> ThesisWorks => _dataService.ThesisWorks;
        public ObservableCollection<Student> Students => _dataService.Students;
        
        // Только преподаватели, которые руководят научными темами или направлениями
        public ObservableCollection<Teacher> Supervisors => new ObservableCollection<Teacher>(
            _dataService.Teachers.Where(t => t.LeadsResearchTopics || t.LeadsResearchDirections)
        );
        
        public ThesisWork? SelectedThesisWork
        {
            get => _selectedThesisWork;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedThesisWork, value);
                if (value != null)
                {
                    Title = value.Title;
                    SelectedStudent = _dataService.GetStudent(value.StudentId);
                    SelectedSupervisor = _dataService.GetTeacher(value.SupervisorId);
                    Year = value.Year;
                    Grade = value.Grade;
                }
            }
        }
        
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
        
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set => this.RaiseAndSetIfChanged(ref _selectedStudent, value);
        }
        
        public Teacher? SelectedSupervisor
        {
            get => _selectedSupervisor;
            set => this.RaiseAndSetIfChanged(ref _selectedSupervisor, value);
        }
        
        public int Year
        {
            get => _year;
            set => this.RaiseAndSetIfChanged(ref _year, value);
        }
        
        public int? Grade
        {
            get => _grade;
            set => this.RaiseAndSetIfChanged(ref _grade, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }
        
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        
        private void AddThesisWork()
        {
            ErrorMessage = string.Empty;

            // Валидация студента
            if (SelectedStudent == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Студент", "not_selected");
                return;
            }

            // Валидация научного руководителя
            if (SelectedSupervisor == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Научный руководитель", "not_selected");
                return;
            }

            // Проверка: преподаватель должен руководить научными темами или направлениями
            if (!SelectedSupervisor.LeadsResearchTopics && !SelectedSupervisor.LeadsResearchDirections)
            {
                ErrorMessage = "Выбранный преподаватель не может быть научным руководителем. " +
                               "Научный руководитель должен иметь отметку \"Руководит научными темами\" " +
                               "или \"Руководит научными направлениями\".";
                return;
            }

            // Валидация темы работы
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Тема работы", "empty");
                return;
            }

            if (!ValidationHelper.IsValidNameWithNumbers(Title))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Тема работы", "invalid_name_with_numbers");
                return;
            }

            // Валидация года защиты
            if (!ValidationHelper.IsValidYear(Year.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Год защиты", "invalid_year");
                return;
            }

            // Валидация оценки (может быть пустой, но если не пустая - должна быть от 2 до 5)
            if (Grade.HasValue && (Grade.Value < 2 || Grade.Value > 5))
            {
                ErrorMessage = "Оценка должна быть числом от 2 до 5.";
                return;
            }
            
            var thesisWork = new ThesisWork
            {
                Title = Title,
                StudentId = SelectedStudent.Id,
                SupervisorId = SelectedSupervisor.Id,
                Year = Year,
                Grade = Grade
            };
            
            _dataService.ThesisWorks.Add(thesisWork);
            
            ClearFields();
        }
        
        private void UpdateThesisWork()
        {
            ErrorMessage = string.Empty;

            if (SelectedThesisWork == null)
            {
                ErrorMessage = "Выберите дипломную работу для обновления.";
                return;
            }

            // Валидация студента
            if (SelectedStudent == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Студент", "not_selected");
                return;
            }

            // Валидация научного руководителя
            if (SelectedSupervisor == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Научный руководитель", "not_selected");
                return;
            }

            // Проверка: преподаватель должен руководить научными темами или направлениями
            if (!SelectedSupervisor.LeadsResearchTopics && !SelectedSupervisor.LeadsResearchDirections)
            {
                ErrorMessage = "Выбранный преподаватель не может быть научным руководителем. " +
                               "Научный руководитель должен иметь отметку \"Руководит научными темами\" " +
                               "или \"Руководит научными направлениями\".";
                return;
            }

            // Валидация темы работы
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Тема работы", "empty");
                return;
            }

            if (!ValidationHelper.IsValidNameWithNumbers(Title))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Тема работы", "invalid_name_with_numbers");
                return;
            }

            // Валидация года защиты
            if (!ValidationHelper.IsValidYear(Year.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Год защиты", "invalid_year");
                return;
            }

            // Валидация оценки (может быть пустой, но если не пустая - должна быть от 2 до 5)
            if (Grade.HasValue && (Grade.Value < 2 || Grade.Value > 5))
            {
                ErrorMessage = "Оценка должна быть числом от 2 до 5.";
                return;
            }
            
            SelectedThesisWork.Title = Title;
            SelectedThesisWork.StudentId = SelectedStudent.Id;
            SelectedThesisWork.SupervisorId = SelectedSupervisor.Id;
            SelectedThesisWork.Year = Year;
            SelectedThesisWork.Grade = Grade;
            
            var index = ThesisWorks.IndexOf(SelectedThesisWork);
            if (index >= 0)
            {
                ThesisWorks[index] = SelectedThesisWork;
            }
        }
        
        private void DeleteThesisWork()
        {
            if (SelectedThesisWork == null) return;
            
            _dataService.ThesisWorks.Remove(SelectedThesisWork);
            ThesisWorks.Remove(SelectedThesisWork);
            
            ClearFields();
        }
        
        private void ClearFields()
        {
            Title = string.Empty;
            SelectedStudent = null;
            SelectedSupervisor = null;
            Year = DateTime.Now.Year;
            Grade = null;
            SelectedThesisWork = null;
            ErrorMessage = string.Empty;
        }
    }
}
