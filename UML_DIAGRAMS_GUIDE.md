# Руководство по UML диаграммам проекта

## Описание взаимосвязей классов для построения UML диаграмм

---

## ВВОДНАЯ ЧАСТЬ

### Иерархия наследования моделей данных

Классы Faculty, Department, Group, Student, Teacher, Discipline, WorkLoad, ThesisWork, StudentGrade и TeacherDiscipline - классы моделей данных приложения. Все эти классы используют механизм реактивных свойств для автоматического уведомления об изменениях. В классах Faculty, Department, Group, Student, Teacher и Discipline переопределён метод ToString() для возврата строкового представления объекта. Все модели данных реализуют методы ToFileString() и FromFileString() для сериализации и десериализации данных в текстовый формат. Класс Faculty содержит списки идентификаторов кафедр и групп, класс Department содержит список идентификаторов преподавателей, класс Group содержит список идентификаторов студентов. Класс Student содержит информацию о студенте и его группе, класс Teacher содержит информацию о преподавателе, его должности, учёной степени и звании. Класс Discipline содержит информацию о дисциплине и связана с группой. Класс WorkLoad связывает преподавателя, дисциплину и группу для учёта учебной нагрузки. Класс StudentGrade хранит оценки студентов по дисциплинам. Класс ThesisWork хранит информацию о дипломных работах студентов и их научных руководителях. Класс TeacherDiscipline реализует связь многие-ко-многим между преподавателями и дисциплинами. На диаграмме представлена структура и взаимосвязи этих классов. (рисунок 1)

### Иерархия наследования ViewModels

Класс ViewModelBase - базовый класс для всех ViewModel в приложении. Класс содержит необходимый набор методов и свойств для поддержки реактивного программирования и автоматического обновления пользовательского интерфейса. Классы MainWindowViewModel, FacultiesViewModel, DepartmentsViewModel, GroupsViewModel, StudentsViewModel, TeachersViewModel, DisciplinesViewModel, WorkLoadsViewModel, ThesisWorksViewModel, StudentProfileViewModel, TeacherProfileViewModel и GradeInputViewModel унаследованы от него. Все ViewModel используют механизм реактивных команд (ReactiveCommand) для обработки действий пользователя и автоматического обновления интерфейса при изменении данных. Класс MainWindowViewModel управляет навигацией между разделами и координирует работу всех ViewModel. Классы FacultiesViewModel, DepartmentsViewModel, GroupsViewModel, StudentsViewModel, TeachersViewModel, DisciplinesViewModel, WorkLoadsViewModel и ThesisWorksViewModel реализуют логику управления соответствующими сущностями (добавление, редактирование, удаление). Класс StudentProfileViewModel отображает профиль студента с его дисциплинами и оценками. Класс TeacherProfileViewModel управляет списком дисциплин преподавателя. Класс GradeInputViewModel управляет вводом оценок студентов. В классах ViewModel переопределяются свойства и методы для работы с конкретными типами данных, при этом сохраняется единый подход к управлению состоянием через базовый класс. На диаграмме представлена иерархия наследования этих классов. (рисунок 1)

### Иерархия наследования представлений

Класс MainWindow - класс главного окна приложения. Класс содержит логику навигации между разделами и обработку события закрытия окна для автосохранения данных. Класс GradeInputWindow - класс модального окна для ввода оценок студентов. Оба класса наследуются от базового класса Window из библиотеки Avalonia.

Классы FacultiesView, DepartmentsView, GroupsView, StudentsView, TeachersView, DisciplinesView, WorkLoadsView, ThesisWorksView, StudentProfileView и TeacherProfileView - классы представлений для отдельных разделов приложения. Все эти классы отвечают за отображение данных соответствующих ViewModel. Каждый класс View содержит разметку интерфейса в формате AXAML и минимальную логику инициализации в коде. Класс FacultiesView отображает раздел управления факультетами, класс DepartmentsView - раздел управления кафедрами, класс GroupsView - раздел управления группами, класс StudentsView - раздел управления студентами, класс TeachersView - раздел управления преподавателями, класс DisciplinesView - раздел управления дисциплинами, класс WorkLoadsView - раздел управления учебной нагрузкой, класс ThesisWorksView - раздел управления дипломными работами, класс StudentProfileView - профиль студента, класс TeacherProfileView - профиль преподавателя. На диаграмме представлена структура этих классов. (рисунок 1)

### Классы сервисов и вспомогательных компонентов

Класс DataService - центральный сервис для работы с данными приложения. Класс содержит наблюдаемые коллекции всех сущностей приложения (Faculties, Departments, Groups, Students, Teachers, Disciplines, WorkLoads, ThesisWorks, Grades, TeacherDisciplines) и управляет их загрузкой и сохранением в текстовые файлы. Класс реализует методы LoadAllData() и SaveAllData() для загрузки и сохранения всех данных, а также методы поиска сущностей по идентификаторам (GetFaculty, GetDepartment, GetGroup, GetStudent, GetTeacher, GetDiscipline) и методы фильтрации (GetDepartmentsByFaculty, GetGroupsByFaculty, GetStudentsByGroup, GetTeachersByDepartment, GetWorkLoadsByTeacher, GetThesisWorksBySupervisor). Все ViewModel используют DataService для доступа к данным. На диаграмме представлены зависимости между DataService и другими классами. (рисунок 1)

Класс ValidationHelper - вспомогательный класс для валидации вводимых данных. Класс содержит статические методы проверки корректности различных типов данных: IsValidName(), IsValidNameWithNumbers(), IsValidYear(), IsValidCourse(), IsValidSemester(), IsValidHours(), а также метод GetErrorMessage() для возврата понятных сообщений об ошибках валидации. Все ViewModel используют ValidationHelper для проверки корректности вводимых пользователем данных.

Класс EnumHelper - вспомогательный класс для создания списков enum значений с русскими названиями. Класс содержит статические методы GetTeacherPositions(), GetAcademicDegrees(), GetAcademicTitles(), GetLessonTypes() и GetControlForms(), которые возвращают списки обёрток EnumItem<T> с локализованными названиями для отображения в ComboBox.

Класс EnumExtensions - класс методов расширения для enum'ов. Класс содержит методы расширения ToRussianString() для всех перечислений (TeacherPosition, AcademicDegree, AcademicTitle, LessonType, ControlForm), которые преобразуют значения enum в русские строки для отображения в интерфейсе.

Класс ViewLocator - класс для автоматического сопоставления ViewModel с соответствующими View. Класс реализует интерфейс IDataTemplate и содержит методы Build() и Match() для динамического создания представлений на основе переданного ViewModel.

Класс EnumItem<T> - обобщённый класс-обёртка для enum значений с русским названием. Класс содержит свойства Value (значение enum) и Display (русское название для отображения) и используется для отображения enum в ComboBox с локализованными названиями.

---

## 1. ИЕРАРХИЯ НАСЛЕДОВАНИЯ

### 1.1 Базовые классы

```
ViewModelBase
    │
    ├── MainWindowViewModel
    ├── FacultiesViewModel
    ├── DepartmentsViewModel
    ├── GroupsViewModel
    ├── StudentsViewModel
    ├── TeachersViewModel
    ├── DisciplinesViewModel
    ├── WorkLoadsViewModel
    ├── ThesisWorksViewModel
    ├── StudentProfileViewModel
    ├── TeacherProfileViewModel
    └── GradeInputViewModel

Models (модели данных)
    ├── Faculty
    ├── Department
    ├── Group
    ├── Student
    ├── Teacher
    ├── Discipline
    ├── WorkLoad
    ├── ThesisWork
    ├── StudentGrade
    └── TeacherDiscipline
```

### 1.2 Представления (Views)

```
MainWindow
GradeInputWindow

FacultiesView
DepartmentsView
GroupsView
StudentsView
TeachersView
DisciplinesView
WorkLoadsView
ThesisWorksView
StudentProfileView
TeacherProfileView
```

### 1.3 Сервисы и вспомогательные классы

```
DataService
    ├── управляет → ObservableCollection<Faculty>
    ├── управляет → ObservableCollection<Department>
    ├── управляет → ObservableCollection<Group>
    ├── управляет → ObservableCollection<Student>
    ├── управляет → ObservableCollection<Teacher>
    ├── управляет → ObservableCollection<Discipline>
    ├── управляет → ObservableCollection<WorkLoad>
    ├── управляет → ObservableCollection<ThesisWork>
    ├── управляет → ObservableCollection<StudentGrade>
    └── управляет → ObservableCollection<TeacherDiscipline>

ValidationHelper (статические методы)
EnumHelper (статические методы)
EnumExtensions (методы расширения)
ViewLocator
EnumItem<T> (обобщённый класс)
```

### 1.4 Перечисления (Enums)

```
TeacherPosition
    ├── Assistant
    ├── Lecturer
    ├── SeniorLecturer
    ├── AssociateProfessor
    └── Professor

AcademicDegree
    ├── None
    ├── CandidateOfSciences
    └── DoctorOfSciences

AcademicTitle
    ├── None
    ├── AssociateProfessor
    └── Professor

LessonType
    ├── Lecture
    ├── Seminar
    ├── Laboratory
    ├── Consultation
    ├── CourseWork
    └── CourseProject

ControlForm
    ├── Pass
    ├── DifferentiatedPass
    └── Exam
```

---

## 2. КОМПОЗИЦИЯ И АГРЕГАЦИЯ

### 2.1 Структура данных (модели)

```
Faculty (Факультет)
    │
    ├── содержит → Department[] (Кафедры) [1 ко многим]
    │   │
    │   └── содержит → Teacher[] (Преподаватели) [1 ко многим]
    │       │
    │       ├── ведет → WorkLoad[] (Учебная нагрузка) [1 ко многим]
    │       ├── связан → TeacherDiscipline[] (Дисциплины преподавателя) [1 ко многим]
    │       └── руководит → ThesisWork[] (Дипломные работы) [1 ко многим]
    │
    └── содержит → Group[] (Группы) [1 ко многим]
        │
        ├── содержит → Student[] (Студенты) [1 ко многим]
        │   │
        │   ├── имеет → StudentGrade[] (Оценки) [1 ко многим]
        │   └── имеет → ThesisWork (Дипломная работа) [1 к 1]
        │
        └── содержит → Discipline[] (Дисциплины) [1 ко многим]
            │
            ├── оценивается → StudentGrade[] (Оценки студентов) [1 ко многим]
            ├── преподается → WorkLoad[] (Нагрузка преподавателей) [1 ко многим]
            └── связана → TeacherDiscipline[] (Связь с преподавателями) [1 ко многим]
```

### 2.2 Типы связей

**Агрегация (слабая связь):**
- `Faculty` → `Department` (факультет содержит кафедры, но кафедра может существовать без факультета)
- `Faculty` → `Group` (факультет содержит группы)
- `Department` → `Teacher` (кафедра содержит преподавателей)
- `Group` → `Student` (группа содержит студентов)
- `Group` → `Discipline` (группа содержит дисциплины)

**Композиция (сильная связь):**
- `Student` → `StudentGrade` (оценка не может существовать без студента)
- `Discipline` → `StudentGrade` (оценка не может существовать без дисциплины)
- `Teacher` → `WorkLoad` (нагрузка не может существовать без преподавателя)
- `Discipline` → `WorkLoad` (нагрузка не может существовать без дисциплины)
- `Student` → `ThesisWork` (дипломная работа не может существовать без студента)
- `Teacher` → `ThesisWork` (дипломная работа не может существовать без научного руководителя)

**Ассоциация (связь использования):**
- `Teacher` ↔ `Discipline` через `TeacherDiscipline` (многие ко многим)
- `Teacher` ↔ `Discipline` через `WorkLoad` (многие ко многим)

---

## 3. ЗАВИСИМОСТИ МЕЖДУ КЛАССАМИ

### 3.1 Слой ViewModels

```
MainWindowViewModel
    │
    ├── использует → DataService
    ├── содержит → FacultiesViewModel
    ├── содержит → DepartmentsViewModel
    ├── содержит → GroupsViewModel
    ├── содержит → StudentsViewModel
    ├── содержит → TeachersViewModel
    ├── содержит → DisciplinesViewModel
    ├── содержит → WorkLoadsViewModel
    ├── содержит → ThesisWorksViewModel
    │
    ├── создает → StudentProfileViewModel
    └── создает → TeacherProfileViewModel

FacultiesViewModel
    ├── использует → DataService
    └── работает с → Faculty

DepartmentsViewModel
    ├── использует → DataService
    ├── работает с → Department
    └── использует → Faculty

GroupsViewModel
    ├── использует → DataService
    ├── работает с → Group
    └── использует → Faculty

StudentsViewModel
    ├── использует → DataService
    ├── работает с → Student
    ├── использует → Group
    └── создает → StudentProfileViewModel

TeachersViewModel
    ├── использует → DataService
    ├── работает с → Teacher
    ├── использует → Department
    ├── использует → TeacherPosition (enum)
    ├── использует → AcademicDegree (enum)
    ├── использует → AcademicTitle (enum)
    └── создает → TeacherProfileViewModel

DisciplinesViewModel
    ├── использует → DataService
    ├── работает с → Discipline
    ├── использует → Group
    └── использует → ControlForm (enum)

WorkLoadsViewModel
    ├── использует → DataService
    ├── работает с → WorkLoad
    ├── использует → Teacher
    ├── использует → Discipline
    ├── использует → Group
    └── использует → LessonType (enum)

ThesisWorksViewModel
    ├── использует → DataService
    ├── работает с → ThesisWork
    ├── использует → Student
    └── использует → Teacher

StudentProfileViewModel
    ├── использует → DataService
    ├── работает с → Student
    ├── использует → Group
    ├── использует → Discipline
    ├── использует → StudentGrade
    └── создает → GradeInputViewModel

TeacherProfileViewModel
    ├── использует → DataService
    ├── работает с → Teacher
    ├── использует → Department
    ├── использует → Discipline
    └── использует → TeacherDiscipline

GradeInputViewModel
    ├── использует → DataService
    ├── работает с → Student
    ├── работает с → Discipline
    └── работает с → StudentGrade
```

### 3.2 Слой Services

```
DataService
    │
    ├── управляет → ObservableCollection<Faculty>
    ├── управляет → ObservableCollection<Department>
    ├── управляет → ObservableCollection<Group>
    ├── управляет → ObservableCollection<Student>
    ├── управляет → ObservableCollection<Teacher>
    ├── управляет → ObservableCollection<Discipline>
    ├── управляет → ObservableCollection<WorkLoad>
    ├── управляет → ObservableCollection<ThesisWork>
    ├── управляет → ObservableCollection<StudentGrade>
    ├── управляет → ObservableCollection<TeacherDiscipline>
    │
    ├── методы поиска:
    │   ├── GetFaculty(Guid)
    │   ├── GetDepartment(Guid)
    │   ├── GetGroup(Guid)
    │   ├── GetStudent(Guid)
    │   ├── GetTeacher(Guid)
    │   └── GetDiscipline(Guid)
    │
    └── методы фильтрации:
        ├── GetDepartmentsByFaculty(Guid)
        ├── GetGroupsByFaculty(Guid)
        ├── GetStudentsByGroup(Guid)
        ├── GetTeachersByDepartment(Guid)
        ├── GetWorkLoadsByTeacher(Guid)
        └── GetThesisWorksBySupervisor(Guid)
```

### 3.3 Слой Views

```
MainWindow
    ├── использует → MainWindowViewModel
    └── отображает → ViewModelBase (через ViewLocator)

FacultiesView
    └── использует → FacultiesViewModel

DepartmentsView
    └── использует → DepartmentsViewModel

GroupsView
    └── использует → GroupsViewModel

StudentsView
    └── использует → StudentsViewModel

TeachersView
    └── использует → TeachersViewModel

DisciplinesView
    └── использует → DisciplinesViewModel

WorkLoadsView
    └── использует → WorkLoadsViewModel

ThesisWorksView
    └── использует → ThesisWorksViewModel

StudentProfileView
    └── использует → StudentProfileViewModel

TeacherProfileView
    └── использует → TeacherProfileViewModel

GradeInputWindow
    └── использует → GradeInputViewModel
```

### 3.4 Вспомогательные классы

```
ViewLocator
    ├── использует → ViewModelBase
    └── создает → UserControl / Window

EnumHelper
    ├── использует → TeacherPosition
    ├── использует → AcademicDegree
    ├── использует → AcademicTitle
    ├── использует → LessonType
    ├── использует → ControlForm
    └── создает → EnumItem<T>

EnumExtensions
    ├── расширяет → TeacherPosition
    ├── расширяет → AcademicDegree
    ├── расширяет → AcademicTitle
    ├── расширяет → LessonType
    └── расширяет → ControlForm

ValidationHelper
    └── статические методы валидации (не зависит от других классов)
```

---

## 4. СТРУКТУРА ДЛЯ UML ДИАГРАММ КЛАССОВ

### 4.1 Основные пакеты (Packages)

```
UniversityIS
    │
    ├── Models (Модели данных)
    │   ├── Faculty
    │   ├── Department
    │   ├── Group
    │   ├── Student
    │   ├── Teacher
    │   ├── Discipline
    │   ├── WorkLoad
    │   ├── ThesisWork
    │   ├── StudentGrade
    │   ├── TeacherDiscipline
    │   ├── TeacherPosition (enum)
    │   ├── AcademicDegree (enum)
    │   ├── AcademicTitle (enum)
    │   ├── LessonType (enum)
    │   ├── ControlForm (enum)
    │   ├── EnumHelper
    │   └── EnumExtensions
    │
    ├── ViewModels (Логика представления)
    │   ├── ViewModelBase
    │   ├── MainWindowViewModel
    │   ├── FacultiesViewModel
    │   ├── DepartmentsViewModel
    │   ├── GroupsViewModel
    │   ├── StudentsViewModel
    │   ├── TeachersViewModel
    │   ├── DisciplinesViewModel
    │   ├── WorkLoadsViewModel
    │   ├── ThesisWorksViewModel
    │   ├── StudentProfileViewModel
    │   ├── TeacherProfileViewModel
    │   └── GradeInputViewModel
    │
    ├── Views (Представления)
    │   ├── MainWindow
    │   ├── FacultiesView
    │   ├── DepartmentsView
    │   ├── GroupsView
    │   ├── StudentsView
    │   ├── TeachersView
    │   ├── DisciplinesView
    │   ├── WorkLoadsView
    │   ├── ThesisWorksView
    │   ├── StudentProfileView
    │   ├── TeacherProfileView
    │   └── GradeInputWindow
    │
    ├── Services (Сервисы)
    │   └── DataService
    │
    └── Helpers (Вспомогательные классы)
        ├── ValidationHelper
        └── ViewLocator
```

### 4.2 Ключевые связи для диаграммы классов

**Наследование (Inheritance):**
- Все ViewModel → ViewModelBase

**Агрегация (Aggregation):**
- Faculty ◇── Department (1 ко многим)
- Faculty ◇── Group (1 ко многим)
- Department ◇── Teacher (1 ко многим)
- Group ◇── Student (1 ко многим)
- Group ◇── Discipline (1 ко многим)

**Композиция (Composition):**
- Student ◆── StudentGrade (1 ко многим)
- Discipline ◆── StudentGrade (1 ко многим)
- Teacher ◆── WorkLoad (1 ко многим)
- Discipline ◆── WorkLoad (1 ко многим)
- Student ◆── ThesisWork (1 к 1)
- Teacher ◆── ThesisWork (1 ко многим, как руководитель)

**Ассоциация (Association):**
- Teacher ←→ Discipline через TeacherDiscipline (многие ко многим)
- Teacher ←→ Discipline через WorkLoad (многие ко многим)

**Зависимость (Dependency):**
- ViewModels → DataService
- ViewModels → Models
- Views → ViewModels
- ViewModels → Enums

---

## 5. СТРУКТУРА ДЛЯ UML ДИАГРАММ КОМПОНЕНТОВ

### 5.1 Компоненты системы

```
[Модели данных]
    ├── Faculty
    ├── Department
    ├── Group
    ├── Student
    ├── Teacher
    ├── Discipline
    ├── WorkLoad
    ├── ThesisWork
    ├── StudentGrade
    └── TeacherDiscipline

[Сервисы]
    └── DataService
        ├── загружает/сохраняет → [Модели данных]
        └── предоставляет доступ → [ViewModels]

[ViewModels]
    ├── MainWindowViewModel
    ├── FacultiesViewModel
    ├── DepartmentsViewModel
    ├── GroupsViewModel
    ├── StudentsViewModel
    ├── TeachersViewModel
    ├── DisciplinesViewModel
    ├── WorkLoadsViewModel
    ├── ThesisWorksViewModel
    ├── StudentProfileViewModel
    ├── TeacherProfileViewModel
    └── GradeInputViewModel
        ├── использует → [Сервисы]
        └── работает с → [Модели данных]

[Views]
    ├── MainWindow
    ├── FacultiesView
    ├── DepartmentsView
    ├── GroupsView
    ├── StudentsView
    ├── TeachersView
    ├── DisciplinesView
    ├── WorkLoadsView
    ├── ThesisWorksView
    ├── StudentProfileView
    ├── TeacherProfileView
    └── GradeInputWindow
        └── отображает → [ViewModels]

[Вспомогательные]
    ├── ValidationHelper
    ├── EnumHelper
    ├── EnumExtensions
    └── ViewLocator
```

---

## 6. СТРУКТУРА ДЛЯ UML ДИАГРАММ ПОСЛЕДОВАТЕЛЬНОСТИ

### 6.1 Типичные сценарии использования

**Сценарий 1: Добавление студента**
```
User → StudentsView → StudentsViewModel → DataService → Student
                                                          ↓
                                                    ObservableCollection<Student>
```

**Сценарий 2: Просмотр профиля студента**
```
User → StudentsView → StudentsViewModel → MainWindowViewModel
                                           ↓
                                    StudentProfileViewModel
                                           ↓
                                    DataService
                                           ↓
                                    Student, Group, Discipline, StudentGrade
```

**Сценарий 3: Добавление учебной нагрузки**
```
User → WorkLoadsView → WorkLoadsViewModel → DataService
                                             ↓
                                    Проверка правил валидации
                                             ↓
                                    WorkLoad → ObservableCollection<WorkLoad>
```

**Сценарий 4: Сохранение данных**
```
User → MainWindow → MainWindowViewModel → DataService
                                          ↓
                                    SaveAllData()
                                          ↓
                                    Сохранение в текстовые файлы
```

---

## 7. КАРДИНАЛЬНОСТЬ СВЯЗЕЙ

### 7.1 Основные связи с указанием кратности

| Родительский класс | Связь | Дочерний класс | Кратность |
|-------------------|-------|----------------|-----------|
| Faculty | содержит | Department | 1..* |
| Faculty | содержит | Group | 1..* |
| Department | содержит | Teacher | 1..* |
| Group | содержит | Student | 1..* |
| Group | содержит | Discipline | 1..* |
| Student | имеет | StudentGrade | 0..* |
| Student | имеет | ThesisWork | 0..1 |
| Teacher | ведет | WorkLoad | 0..* |
| Teacher | связан | TeacherDiscipline | 0..* |
| Teacher | руководит | ThesisWork | 0..* |
| Discipline | оценивается | StudentGrade | 0..* |
| Discipline | преподается | WorkLoad | 0..* |
| Discipline | связана | TeacherDiscipline | 0..* |

### 7.2 Связи через внешние ключи (Guid)

Все связи между моделями реализованы через идентификаторы Guid:
- `Department.FacultyId` → `Faculty.Id`
- `Group.FacultyId` → `Faculty.Id`
- `Student.GroupId` → `Group.Id`
- `Teacher.DepartmentId` → `Department.Id`
- `Discipline.GroupId` → `Group.Id`
- `WorkLoad.TeacherId` → `Teacher.Id`
- `WorkLoad.DisciplineId` → `Discipline.Id`
- `WorkLoad.GroupId` → `Group.Id`
- `StudentGrade.StudentId` → `Student.Id`
- `StudentGrade.DisciplineId` → `Discipline.Id`
- `ThesisWork.StudentId` → `Student.Id`
- `ThesisWork.SupervisorId` → `Teacher.Id`
- `TeacherDiscipline.TeacherId` → `Teacher.Id`
- `TeacherDiscipline.DisciplineId` → `Discipline.Id`

---

## 8. РЕКОМЕНДАЦИИ ДЛЯ ПОСТРОЕНИЯ UML ДИАГРАММ

### 8.1 Диаграмма классов (Class Diagram)

**Рекомендуется создать несколько диаграмм:**

1. **Диаграмма моделей данных:**
   - Все классы Models
   - Перечисления (Enums)
   - Связи между моделями
   - Атрибуты и методы

2. **Диаграмма ViewModels:**
   - ViewModelBase и все наследники
   - Зависимости от DataService
   - Зависимости от Models

3. **Диаграмма Views:**
   - Все классы Views
   - Зависимости от ViewModels
   - Наследование от UserControl/Window

4. **Общая диаграмма архитектуры:**
   - Все пакеты
   - Основные связи между пакетами

### 8.2 Диаграмма компонентов (Component Diagram)

Показать:
- Пакет Models
- Пакет ViewModels
- Пакет Views
- Пакет Services
- Пакет Helpers
- Зависимости между пакетами

### 8.3 Диаграмма последовательности (Sequence Diagram)

Рекомендуемые сценарии:
1. Запуск приложения
2. Добавление нового факультета
3. Добавление студента
4. Просмотр профиля студента
5. Ввод оценки студента
6. Сохранение данных

### 8.4 Диаграмма вариантов использования (Use Case Diagram)

Основные актеры:
- Пользователь системы

Основные варианты использования:
- Управление факультетами
- Управление кафедрами
- Управление группами
- Управление студентами
- Управление преподавателями
- Управление дисциплинами
- Управление учебной нагрузкой
- Управление дипломными работами
- Просмотр профилей
- Ввод оценок

---

## 9. ОСОБЕННОСТИ РЕАЛИЗАЦИИ

### 9.1 Паттерны проектирования

1. **MVVM (Model-View-ViewModel):**
   - Models - данные
   - Views - представление
   - ViewModels - логика

2. **Repository Pattern:**
   - DataService выступает как репозиторий для всех моделей

3. **Observer Pattern:**
   - Использование ObservableCollection для автоматического обновления UI

4. **Dependency Injection:**
   - DataService передается в ViewModels через конструктор

### 9.2 Реактивное программирование

- Все ViewModels наследуются от ViewModelBase
- Использование ReactiveCommand для команд
- Автоматическое обновление UI при изменении данных

### 9.3 Сериализация данных

- Все модели имеют методы `ToFileString()` и `FromFileString()`
- Данные сохраняются в текстовые файлы в формате разделителей (|)

---

## 10. ПРИМЕРЫ СВЯЗЕЙ ДЛЯ ДИАГРАММ

### 10.1 Пример связи "один ко многим"

```
Faculty
    +Guid Id
    +string Name
    +string Dean
    +List<Guid> DepartmentIds
    +List<Guid> GroupIds
    |
    | (1..*)
    |
    v
Department
    +Guid Id
    +string Name
    +string Head
    +Guid FacultyId  ← внешний ключ
```

### 10.2 Пример связи "многие ко многим"

```
Teacher                    TeacherDiscipline                    Discipline
    +Guid Id                   +Guid Id                            +Guid Id
    +string LastName            +Guid TeacherId  ← FK               +string Name
    +string FirstName           +Guid DisciplineId ← FK            +int Course
    +Guid DepartmentId          |                                   +int Semester
                                | (связующая таблица)
                                |
                                v
                    Реализует связь многие-ко-многим
```

### 10.3 Пример наследования

```
ViewModelBase
    +RaiseAndSetIfChanged()
    |
    ↑
    |
MainWindowViewModel
    +DataService _dataService
    +ViewModelBase CurrentPage
    +ShowFacultiesCommand
    +FacultiesViewModel
    +DepartmentsViewModel
    +GroupsViewModel
    +StudentsViewModel
    +TeachersViewModel
    +DisciplinesViewModel
    +WorkLoadsViewModel
    +ThesisWorksViewModel
```

---

Этот документ содержит всю необходимую информацию для построения UML диаграмм различных типов. Используйте его как справочник при создании диаграмм в инструментах типа PlantUML, Draw.io, Visual Paradigm или других UML редакторах.

