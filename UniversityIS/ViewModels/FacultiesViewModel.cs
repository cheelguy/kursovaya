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
    // ViewModel для управления факультетами
    // Позволяет добавлять, редактировать и удалять факультеты
    public class FacultiesViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private Faculty? _selectedFaculty;
        private string _name = string.Empty;
        private string _dean = string.Empty;
        private string _errorMessage = string.Empty;
        
        public FacultiesViewModel(DataService dataService)
        {
            _dataService = dataService;
            
            AddCommand = ReactiveCommand.Create(AddFaculty, outputScheduler: RxApp.MainThreadScheduler);
            UpdateCommand = ReactiveCommand.Create(UpdateFaculty, outputScheduler: RxApp.MainThreadScheduler);
            DeleteCommand = ReactiveCommand.Create(DeleteFaculty, outputScheduler: RxApp.MainThreadScheduler);
        }
        
        public ObservableCollection<Faculty> Faculties => _dataService.Faculties;
        
        public Faculty? SelectedFaculty
        {
            get => _selectedFaculty;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFaculty, value);
                if (value != null)
                {
                    Name = value.Name;
                    Dean = value.Dean;
                }
            }
        }
        
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        public string Dean
        {
            get => _dean;
            set => this.RaiseAndSetIfChanged(ref _dean, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }
        
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        
        // Добавляет новый факультет в базу данных
        // Выполняет валидацию всех полей перед добавлением
        private void AddFaculty()
        {
            ErrorMessage = string.Empty;

            // Валидация названия факультета
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название факультета", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название факультета", "invalid_name");
                return;
            }

            // Валидация ФИО декана
            if (string.IsNullOrWhiteSpace(Dean))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("ФИО декана", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(Dean))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("ФИО декана", "invalid_name");
                return;
            }
            
            // Создаем новый факультет и добавляем в коллекцию
            var faculty = new Faculty
            {
                Name = Name,
                Dean = Dean
            };
            
            _dataService.Faculties.Add(faculty);
            
            ClearFields();
        }
        
        private void UpdateFaculty()
        {
            ErrorMessage = string.Empty;

            if (SelectedFaculty == null)
            {
                ErrorMessage = "Выберите факультет для обновления.";
                return;
            }

            // Валидация названия факультета
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название факультета", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название факультета", "invalid_name");
                return;
            }

            // Валидация ФИО декана
            if (string.IsNullOrWhiteSpace(Dean))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("ФИО декана", "empty");
                return;
            }

            if (!ValidationHelper.IsValidName(Dean))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("ФИО декана", "invalid_name");
                return;
            }
            
            SelectedFaculty.Name = Name;
            SelectedFaculty.Dean = Dean;
            
            // Обновляем отображение
            var index = Faculties.IndexOf(SelectedFaculty);
            if (index >= 0)
            {
                Faculties[index] = SelectedFaculty;
            }
        }
        
        private void DeleteFaculty()
        {
            if (SelectedFaculty == null) return;
            
            _dataService.Faculties.Remove(SelectedFaculty);
            Faculties.Remove(SelectedFaculty);
            
            ClearFields();
        }
        
        private void ClearFields()
        {
            Name = string.Empty;
            Dean = string.Empty;
            SelectedFaculty = null;
            ErrorMessage = string.Empty;
        }
    }
}
