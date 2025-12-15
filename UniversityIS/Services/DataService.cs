using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UniversityIS.Models;

namespace UniversityIS.Services
{
    // Центральный сервис для работы с данными приложения
    // Отвечает за загрузку и сохранение всех данных в текстовые файлы
    // Все данные хранятся в папке Data в формате текстовых файлов
    public class DataService
    {
        private static readonly string DataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        
        private const string FacultiesFile = "faculties.txt";
        private const string DepartmentsFile = "departments.txt";
        private const string GroupsFile = "groups.txt";
        private const string StudentsFile = "students.txt";
        private const string TeachersFile = "teachers.txt";
        private const string DisciplinesFile = "disciplines.txt";
        private const string WorkLoadsFile = "workloads.txt";
        private const string ThesisWorksFile = "thesisworks.txt";
        private const string GradesFile = "grades.txt";
        private const string TeacherDisciplinesFile = "teacherdisciplines.txt";
        
        // Коллекции всех сущностей приложения
        // Используются ObservableCollection для автоматического обновления UI при изменениях
        public ObservableCollection<Faculty> Faculties { get; private set; }
        public ObservableCollection<Department> Departments { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }
        public ObservableCollection<Student> Students { get; private set; }
        public ObservableCollection<Teacher> Teachers { get; private set; }
        public ObservableCollection<Discipline> Disciplines { get; private set; }
        public ObservableCollection<WorkLoad> WorkLoads { get; private set; }
        public ObservableCollection<ThesisWork> ThesisWorks { get; private set; }
        public ObservableCollection<StudentGrade> Grades { get; private set; }
        public ObservableCollection<TeacherDiscipline> TeacherDisciplines { get; private set; }
        
        public DataService()
        {
            Faculties = new ObservableCollection<Faculty>();
            Departments = new ObservableCollection<Department>();
            Groups = new ObservableCollection<Group>();
            Students = new ObservableCollection<Student>();
            Teachers = new ObservableCollection<Teacher>();
            Disciplines = new ObservableCollection<Discipline>();
            WorkLoads = new ObservableCollection<WorkLoad>();
            ThesisWorks = new ObservableCollection<ThesisWork>();
            Grades = new ObservableCollection<StudentGrade>();
            TeacherDisciplines = new ObservableCollection<TeacherDiscipline>();
            
            EnsureDataDirectoryExists();
        }
        
        // Создает папку Data для хранения файлов, если она не существует
        private void EnsureDataDirectoryExists()
        {
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }
        }
        
        // Загружает все данные из текстовых файлов в память
        // Вызывается при запуске приложения
        public void LoadAllData()
        {
            LoadFaculties();
            LoadDepartments();
            LoadGroups();
            LoadStudents();
            LoadTeachers();
            LoadDisciplines();
            LoadWorkLoads();
            LoadThesisWorks();
            LoadGrades();
            LoadTeacherDisciplines();
        }
        
        // Сохраняет все данные из памяти в текстовые файлы
        // Вызывается при закрытии приложения или по запросу пользователя
        public void SaveAllData()
        {
            SaveTeacherDisciplines();
            SaveGrades();
            SaveFaculties();
            SaveDepartments();
            SaveGroups();
            SaveStudents();
            SaveTeachers();
            SaveDisciplines();
            SaveWorkLoads();
            SaveThesisWorks();
        }
        
        
        // Получить путь к папке с данными
        
        public string GetDataPath() => DataDirectory;
        
        #region Faculties
        
        // Загружает список факультетов из текстового файла
        // Пропускает пустые и некорректные строки
        private void LoadFaculties()
        {
            var path = Path.Combine(DataDirectory, FacultiesFile);
            if (!File.Exists(path))
            {
                return;
            }
            
            Faculties.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        Faculties.Add(Faculty.FromFileString(line));
                    }
                    catch
                    {
                        // Игнорируем ошибочные строки для стабильности работы
                    }
                }
            }
        }
        
        // Сохраняет список факультетов в текстовый файл
        private void SaveFaculties()
        {
            var path = Path.Combine(DataDirectory, FacultiesFile);
            var lines = Faculties.Select(f => f.ToFileString());
            File.WriteAllLines(path, lines);
        }
        
        #endregion
        
        #region Departments
        
        // Загружает список кафедр из текстового файла
        private void LoadDepartments()
        {
            var path = Path.Combine(DataDirectory, DepartmentsFile);
            if (!File.Exists(path))
            {
                return;
            }
            
            Departments.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        Departments.Add(Department.FromFileString(line));
                    }
                    catch
                    {
                        // Игнорируем ошибочные строки
                    }
                }
            }
        }
        
        private void SaveDepartments()
        {
            var path = Path.Combine(DataDirectory, DepartmentsFile);
            var lines = Departments.Select(d => d.ToFileString());
            File.WriteAllLines(path, lines);
        }
        
        #endregion
        
        #region Groups
        
        private void LoadGroups()
        {
            var path = Path.Combine(DataDirectory, GroupsFile);
            if (!File.Exists(path))
            {
                return;
            }
            
            Groups.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        Groups.Add(Group.FromFileString(line));
                    }
                    catch
                    {
                        // Игнорируем ошибочные строки
                    }
                }
            }
        }
        
        private void SaveGroups()
        {
            var path = Path.Combine(DataDirectory, GroupsFile);
            var lines = Groups.Select(g => g.ToFileString());
            File.WriteAllLines(path, lines);
        }
        
        #endregion
        
        #region Students
        
        private void LoadStudents()
        {
            var path = Path.Combine(DataDirectory, StudentsFile);
            if (!File.Exists(path))
            {
                return;
            }
            
            Students.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        Students.Add(Student.FromFileString(line));
                    }
                    catch
                    {
                        // Игнорируем ошибочные строки
                    }
                }
            }
        }
        
        private void SaveStudents()
        {
            var path = Path.Combine(DataDirectory, StudentsFile);
            var lines = Students.Select(s => s.ToFileString());
            File.WriteAllLines(path, lines);
        }
        
        #endregion
        
        #region Teachers
        
        private void LoadTeachers()
        {
            var path = Path.Combine(DataDirectory, TeachersFile);
            if (!File.Exists(path))
            {
                return;
            }
            
            Teachers.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        Teachers.Add(Teacher.FromFileString(line));
                    }
                    catch
                    {
                        // Игнорируем ошибочные строки
                    }
                }
            }
        }
        
        private void SaveTeachers()
        {
            var path = Path.Combine(DataDirectory, TeachersFile);
            var lines = Teachers.Select(t => t.ToFileString());
            File.WriteAllLines(path, lines);
        }
        
        #endregion
        
        #region Disciplines
        
        private void LoadDisciplines()
        {
            var path = Path.Combine(DataDirectory, DisciplinesFile);
            if (!File.Exists(path))
            {
                return;
            }
            
            Disciplines.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        Disciplines.Add(Discipline.FromFileString(line));
                    }
                    catch
                    {
                        // Игнорируем ошибочные строки
                    }
                }
            }
        }
        
        private void SaveDisciplines()
        {
            var path = Path.Combine(DataDirectory, DisciplinesFile);
            var lines = Disciplines.Select(d => d.ToFileString());
            File.WriteAllLines(path, lines);
        }
        
        #endregion
        
        #region WorkLoads
        
        private void LoadWorkLoads()
        {
            var path = Path.Combine(DataDirectory, WorkLoadsFile);
            if (!File.Exists(path))
            {
                return;
            }
            
            WorkLoads.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        WorkLoads.Add(WorkLoad.FromFileString(line));
                    }
                    catch
                    {
                        // Игнорируем ошибочные строки
                    }
                }
            }
        }
        
        private void SaveWorkLoads()
        {
            var path = Path.Combine(DataDirectory, WorkLoadsFile);
            var lines = WorkLoads.Select(w => w.ToFileString());
            File.WriteAllLines(path, lines);
        }
        
        #endregion
        
        #region ThesisWorks
        
        private void LoadThesisWorks()
        {
            var path = Path.Combine(DataDirectory, ThesisWorksFile);
            if (!File.Exists(path))
            {
                return;
            }
            
            ThesisWorks.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        ThesisWorks.Add(ThesisWork.FromFileString(line));
                    }
                    catch
                    {
                        // Игнорируем ошибочные строки
                    }
                }
            }
        }
        
        private void SaveThesisWorks()
        {
            var path = Path.Combine(DataDirectory, ThesisWorksFile);
            var lines = ThesisWorks.Select(t => t.ToFileString());
            File.WriteAllLines(path, lines);
        }

        private void LoadGrades()
        {
            var path = Path.Combine(DataDirectory, GradesFile);
            if (!File.Exists(path)) return;

            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    Grades.Add(StudentGrade.FromFileString(line));
                }
            }
        }

        private void SaveGrades()
        {
            var path = Path.Combine(DataDirectory, GradesFile);
            var lines = Grades.Select(g => g.ToFileString());
            File.WriteAllLines(path, lines);
        }

        private void LoadTeacherDisciplines()
        {
            var path = Path.Combine(DataDirectory, TeacherDisciplinesFile);
            if (!File.Exists(path)) return;

            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    TeacherDisciplines.Add(TeacherDiscipline.FromFileString(line));
                }
            }
        }

        private void SaveTeacherDisciplines()
        {
            var path = Path.Combine(DataDirectory, TeacherDisciplinesFile);
            var lines = TeacherDisciplines.Select(td => td.ToFileString());
            File.WriteAllLines(path, lines);
        }
        
        #endregion
        
        #region Helper Methods
        
        // Вспомогательные методы для поиска сущностей по ID
        // Используются во всех ViewModel для получения связанных объектов
        
        // Находит факультет по его уникальному идентификатору
        public Faculty? GetFaculty(Guid id)
        {
            return Faculties.FirstOrDefault(f => f.Id == id);
        }
        
        // Находит кафедру по ее уникальному идентификатору
        public Department? GetDepartment(Guid id)
        {
            return Departments.FirstOrDefault(d => d.Id == id);
        }
        
        // Находит учебную группу по ее уникальному идентификатору
        public Group? GetGroup(Guid id)
        {
            return Groups.FirstOrDefault(g => g.Id == id);
        }
        
        // Находит студента по его уникальному идентификатору
        public Student? GetStudent(Guid id)
        {
            return Students.FirstOrDefault(s => s.Id == id);
        }
        
        // Находит преподавателя по его уникальному идентификатору
        public Teacher? GetTeacher(Guid id)
        {
            return Teachers.FirstOrDefault(t => t.Id == id);
        }
        
        // Находит дисциплину по ее уникальному идентификатору
        public Discipline? GetDiscipline(Guid id)
        {
            return Disciplines.FirstOrDefault(d => d.Id == id);
        }
        
        // Возвращает список всех кафедр, принадлежащих указанному факультету
        public List<Department> GetDepartmentsByFaculty(Guid facultyId)
        {
            return Departments.Where(d => d.FacultyId == facultyId).ToList();
        }
        
        // Возвращает список всех групп, принадлежащих указанному факультету
        public List<Group> GetGroupsByFaculty(Guid facultyId)
        {
            return Groups.Where(g => g.FacultyId == facultyId).ToList();
        }
        
        // Возвращает список всех студентов указанной группы
        public List<Student> GetStudentsByGroup(Guid groupId)
        {
            return Students.Where(s => s.GroupId == groupId).ToList();
        }
        
        // Возвращает список всех преподавателей указанной кафедры
        public List<Teacher> GetTeachersByDepartment(Guid departmentId)
        {
            return Teachers.Where(t => t.DepartmentId == departmentId).ToList();
        }
        
        // Возвращает список всей учебной нагрузки указанного преподавателя
        public List<WorkLoad> GetWorkLoadsByTeacher(Guid teacherId)
        {
            return WorkLoads.Where(w => w.TeacherId == teacherId).ToList();
        }
        
        // Возвращает список всех дипломных работ, которыми руководит указанный преподаватель
        public List<ThesisWork> GetThesisWorksBySupervisor(Guid supervisorId)
        {
            return ThesisWorks.Where(t => t.SupervisorId == supervisorId).ToList();
        }
        
        #endregion
    }
}
