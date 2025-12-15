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
    // Группа дисциплин, объединенных по группе
    // Используется для отображения дисциплин с группировкой по группам
    public class GroupDisciplines
    {
        public string GroupName { get; set; } = string.Empty;
        public ObservableCollection<Discipline> Disciplines { get; set; } = new();
    }

    // ViewModel для управления дисциплинами
    // Позволяет добавлять, редактировать и удалять дисциплины
    // Автоматически вычисляет курс на основе семестра
    // Группирует дисциплины по группам для удобного отображения
    public class DisciplinesViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private Discipline? _selectedDiscipline;
        private string _name = string.Empty;
        private int _course = 1;
        private int _semester = 1;
        private int _lectureHours = 0;
        private int _seminarHours = 0;
        private int _laboratoryHours = 0;
        private ControlForm _controlForm = ControlForm.Exam;
        private EnumItem<ControlForm>? _selectedControlFormItem;
        private string _errorMessage = string.Empty;
        private Models.Group? _selectedGroup;
        private ObservableCollection<GroupDisciplines> _groupedDisciplines = new();
        
        public DisciplinesViewModel(DataService dataService)
        {
            _dataService = dataService;
            ControlForms = new ObservableCollection<EnumItem<ControlForm>>(EnumHelper.GetControlForms());
            
            AddCommand = ReactiveCommand.Create(AddDiscipline, outputScheduler: RxApp.MainThreadScheduler);
            UpdateCommand = ReactiveCommand.Create(UpdateDiscipline, outputScheduler: RxApp.MainThreadScheduler);
            DeleteCommand = ReactiveCommand.Create(DeleteDiscipline, outputScheduler: RxApp.MainThreadScheduler);
            
            // Подписываемся на изменения для обновления группировки
            _dataService.Disciplines.CollectionChanged += (s, e) => UpdateGroupedDisciplines();
            _dataService.Groups.CollectionChanged += (s, e) => UpdateGroupedDisciplines();
            
            UpdateGroupedDisciplines();
        }
        
        public ObservableCollection<Discipline> Disciplines => _dataService.Disciplines;
        public ObservableCollection<Models.Group> Groups => _dataService.Groups;
        public ObservableCollection<EnumItem<ControlForm>> ControlForms { get; }

        public ObservableCollection<GroupDisciplines> GroupedDisciplines
        {
            get => _groupedDisciplines;
            set => this.RaiseAndSetIfChanged(ref _groupedDisciplines, value);
        }
        
        public Discipline? SelectedDiscipline
        {
            get => _selectedDiscipline;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedDiscipline, value);
                if (value != null)
                {
                    Name = value.Name;
                    Course = value.Course;
                    Semester = value.Semester;
                    LectureHours = value.LectureHours;
                    SeminarHours = value.SeminarHours;
                    LaboratoryHours = value.LaboratoryHours;
                    ControlForm = value.ControlForm;
                    SelectedControlFormItem = ControlForms.FirstOrDefault(x => x.Value == value.ControlForm);
                    SelectedGroup = _dataService.GetGroup(value.GroupId);
                }
            }
        }
        
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        public int Course
        {
            get => _course;
            set => this.RaiseAndSetIfChanged(ref _course, value);
        }
        
        public int Semester
        {
            get => _semester;
            set => this.RaiseAndSetIfChanged(ref _semester, value);
        }
        
        public int LectureHours
        {
            get => _lectureHours;
            set => this.RaiseAndSetIfChanged(ref _lectureHours, value);
        }
        
        public int SeminarHours
        {
            get => _seminarHours;
            set => this.RaiseAndSetIfChanged(ref _seminarHours, value);
        }
        
        public int LaboratoryHours
        {
            get => _laboratoryHours;
            set => this.RaiseAndSetIfChanged(ref _laboratoryHours, value);
        }
        
        public ControlForm ControlForm
        {
            get => _controlForm;
            set => this.RaiseAndSetIfChanged(ref _controlForm, value);
        }
        
        public EnumItem<ControlForm>? SelectedControlFormItem
        {
            get => _selectedControlFormItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedControlFormItem, value);
                if (value != null)
                    ControlForm = value.Value;
            }
        }

        public Models.Group? SelectedGroup
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
        
        private void AddDiscipline()
        {
            ErrorMessage = string.Empty;

            // Валидация группы
            if (SelectedGroup == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Группа", "not_selected");
                return;
            }

            // Валидация названия дисциплины
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название дисциплины", "empty");
                return;
            }

            if (!ValidationHelper.IsValidNameWithNumbers(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название дисциплины", "invalid_name_with_numbers");
                return;
            }

            // Валидация курса
            if (!ValidationHelper.IsValidCourse(Course.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Курс", "invalid_course");
                return;
            }

            // Валидация семестра
            if (Semester < 1 || Semester > 12)
            {
                ErrorMessage = "Поле \"Семестр\" должно быть числом от 1 до 12.";
                return;
            }

            // Валидация часов лекций
            if (LectureHours < 0 || LectureHours > 1000)
            {
                ErrorMessage = "Поле \"Часы лекций\" должно быть от 0 до 1000.";
                return;
            }

            // Валидация часов семинаров
            if (SeminarHours < 0 || SeminarHours > 1000)
            {
                ErrorMessage = "Поле \"Часы семинаров\" должно быть от 0 до 1000.";
                return;
            }

            // Валидация часов лабораторных
            if (LaboratoryHours < 0 || LaboratoryHours > 1000)
            {
                ErrorMessage = "Поле \"Часы лабораторных\" должно быть от 0 до 1000.";
                return;
            }

            // Проверка, что хотя бы один тип занятий имеет часы
            if (LectureHours == 0 && SeminarHours == 0 && LaboratoryHours == 0)
            {
                ErrorMessage = "Хотя бы один тип занятий должен иметь ненулевое количество часов.";
                return;
            }
            
            var discipline = new Discipline
            {
                Name = Name,
                LectureHours = LectureHours,
                SeminarHours = SeminarHours,
                LaboratoryHours = LaboratoryHours,
                ControlForm = ControlForm,
                GroupId = SelectedGroup.Id
            };
            
            // Устанавливаем семестр и автоматически вычисляем курс
            discipline.SetSemester(Semester);
            
            _dataService.Disciplines.Add(discipline);
            
            ClearFields();
        }
        
        private void UpdateDiscipline()
        {
            ErrorMessage = string.Empty;

            if (SelectedDiscipline == null)
            {
                ErrorMessage = "Выберите дисциплину для обновления.";
                return;
            }

            // Валидация группы
            if (SelectedGroup == null)
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Группа", "not_selected");
                return;
            }

            // Валидация названия дисциплины
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название дисциплины", "empty");
                return;
            }

            if (!ValidationHelper.IsValidNameWithNumbers(Name))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Название дисциплины", "invalid_name_with_numbers");
                return;
            }

            // Валидация семестра
            if (!ValidationHelper.IsValidSemester(Semester.ToString()))
            {
                ErrorMessage = ValidationHelper.GetErrorMessage("Семестр", "invalid_semester");
                return;
            }

            // Валидация часов лекций
            if (LectureHours < 0 || LectureHours > 1000)
            {
                ErrorMessage = "Поле \"Часы лекций\" должно быть от 0 до 1000.";
                return;
            }

            // Валидация часов семинаров
            if (SeminarHours < 0 || SeminarHours > 1000)
            {
                ErrorMessage = "Поле \"Часы семинаров\" должно быть от 0 до 1000.";
                return;
            }

            // Валидация часов лабораторных
            if (LaboratoryHours < 0 || LaboratoryHours > 1000)
            {
                ErrorMessage = "Поле \"Часы лабораторных\" должно быть от 0 до 1000.";
                return;
            }

            // Проверка, что хотя бы один тип занятий имеет часы
            if (LectureHours == 0 && SeminarHours == 0 && LaboratoryHours == 0)
            {
                ErrorMessage = "Хотя бы один тип занятий должен иметь ненулевое количество часов.";
                return;
            }
            
            SelectedDiscipline.Name = Name;
            SelectedDiscipline.LectureHours = LectureHours;
            SelectedDiscipline.SeminarHours = SeminarHours;
            SelectedDiscipline.LaboratoryHours = LaboratoryHours;
            SelectedDiscipline.ControlForm = ControlForm;
            SelectedDiscipline.GroupId = SelectedGroup.Id;
            
            // Устанавливаем семестр и автоматически вычисляем курс
            SelectedDiscipline.SetSemester(Semester);
            
            var index = Disciplines.IndexOf(SelectedDiscipline);
            if (index >= 0)
            {
                Disciplines[index] = SelectedDiscipline;
            }
        }
        
        private void DeleteDiscipline()
        {
            if (SelectedDiscipline == null) return;
            
            _dataService.Disciplines.Remove(SelectedDiscipline);
            Disciplines.Remove(SelectedDiscipline);
            
            ClearFields();
        }
        
        private void ClearFields()
        {
            Name = string.Empty;
            Course = 1;
            Semester = 1;
            LectureHours = 0;
            SeminarHours = 0;
            LaboratoryHours = 0;
            ControlForm = ControlForm.Exam;
            SelectedGroup = null;
            SelectedDiscipline = null;
            ErrorMessage = string.Empty;
        }

        private void UpdateGroupedDisciplines()
        {
            var groups = new ObservableCollection<GroupDisciplines>();
            
            // Группируем дисциплины по группам
            var disciplinesByGroup = _dataService.Disciplines
                .GroupBy(d => d.GroupId)
                .OrderBy(g => _dataService.GetGroup(g.Key)?.Number ?? "Неизвестная группа");
            
            foreach (var group in disciplinesByGroup)
            {
                var groupObj = _dataService.GetGroup(group.Key);
                if (groupObj == null) continue;
                
                var groupDisciplines = new GroupDisciplines
                {
                    GroupName = groupObj.Number,
                    Disciplines = new ObservableCollection<Discipline>(group.OrderBy(d => d.Name))
                };
                groups.Add(groupDisciplines);
            }
            
            GroupedDisciplines = groups;
        }
    }
}
