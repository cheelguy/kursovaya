using System.Reactive;
using ReactiveUI;
using UniversityIS.Models;
using UniversityIS.Services;

namespace UniversityIS.ViewModels;

// Главная ViewModel приложения
// Управляет навигацией между разными разделами и координирует работу всех ViewModel
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly DataService _dataService;
    
    // Текущая открытая страница (раздел приложения)
    private ViewModelBase _currentPage;
    
    // Инициализация главного окна: загрузка данных и создание всех ViewModel
    public MainWindowViewModel()
    {
        _dataService = new DataService();
        _dataService.LoadAllData();
        
        // Инициализация ViewModels для разделов
        FacultiesViewModel = new FacultiesViewModel(_dataService);
        DepartmentsViewModel = new DepartmentsViewModel(_dataService);
        GroupsViewModel = new GroupsViewModel(_dataService);
        StudentsViewModel = new StudentsViewModel(_dataService, ShowStudentProfile);
        TeachersViewModel = new TeachersViewModel(_dataService, ShowTeacherProfile);
        DisciplinesViewModel = new DisciplinesViewModel(_dataService);
        WorkLoadsViewModel = new WorkLoadsViewModel(_dataService);
        ThesisWorksViewModel = new ThesisWorksViewModel(_dataService);
        
        // По умолчанию открываем факультеты
        _currentPage = FacultiesViewModel;
        
        // Команды навигации с указанием главного потока UI
        ShowFacultiesCommand = ReactiveCommand.Create(() => { CurrentPage = FacultiesViewModel; }, outputScheduler: RxApp.MainThreadScheduler);
        ShowDepartmentsCommand = ReactiveCommand.Create(() => { CurrentPage = DepartmentsViewModel; }, outputScheduler: RxApp.MainThreadScheduler);
        ShowGroupsCommand = ReactiveCommand.Create(() => { CurrentPage = GroupsViewModel; }, outputScheduler: RxApp.MainThreadScheduler);
        ShowStudentsCommand = ReactiveCommand.Create(() => { CurrentPage = StudentsViewModel; }, outputScheduler: RxApp.MainThreadScheduler);
        ShowTeachersCommand = ReactiveCommand.Create(() => { CurrentPage = TeachersViewModel; }, outputScheduler: RxApp.MainThreadScheduler);
        ShowDisciplinesCommand = ReactiveCommand.Create(() => { CurrentPage = DisciplinesViewModel; }, outputScheduler: RxApp.MainThreadScheduler);
        ShowWorkLoadsCommand = ReactiveCommand.Create(() => { CurrentPage = WorkLoadsViewModel; }, outputScheduler: RxApp.MainThreadScheduler);
        ShowThesisWorksCommand = ReactiveCommand.Create(() => { CurrentPage = ThesisWorksViewModel; }, outputScheduler: RxApp.MainThreadScheduler);
        
        SaveDataCommand = ReactiveCommand.Create(() => 
        { 
            _dataService.SaveAllData();
        }, outputScheduler: RxApp.MainThreadScheduler);
    }
    
    // Вызывается при закрытии приложения для сохранения всех данных
    public void OnClosing()
    {
        _dataService.SaveAllData();
    }

    // Открывает профиль выбранного студента с информацией о его предметах и оценках
    public void ShowStudentProfile(Student student)
    {
        var profileViewModel = new StudentProfileViewModel(_dataService, student);
        CurrentPage = profileViewModel;
    }

    // Открывает профиль выбранного преподавателя с информацией о его дисциплинах
    public void ShowTeacherProfile(Teacher teacher)
    {
        var profileViewModel = new TeacherProfileViewModel(_dataService, teacher);
        CurrentPage = profileViewModel;
    }
    
    // Текущая открытая страница в главном окне
    // При изменении автоматически обновляется содержимое главного окна
    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }
    
    // ViewModel для всех разделов приложения
    public FacultiesViewModel FacultiesViewModel { get; }
    public DepartmentsViewModel DepartmentsViewModel { get; }
    public GroupsViewModel GroupsViewModel { get; }
    public StudentsViewModel StudentsViewModel { get; }
    public TeachersViewModel TeachersViewModel { get; }
    public DisciplinesViewModel DisciplinesViewModel { get; }
    public WorkLoadsViewModel WorkLoadsViewModel { get; }
    public ThesisWorksViewModel ThesisWorksViewModel { get; }
    
    // Команды для навигации между разделами приложения
    // Все команды выполняются в главном потоке UI для безопасности
    public ReactiveCommand<Unit, Unit> ShowFacultiesCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowDepartmentsCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowGroupsCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowStudentsCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowTeachersCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowDisciplinesCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowWorkLoadsCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowThesisWorksCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveDataCommand { get; }
}
