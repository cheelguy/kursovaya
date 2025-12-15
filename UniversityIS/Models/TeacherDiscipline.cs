using System;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Модель связи преподавателя с дисциплиной
    // Определяет, какие дисциплины может вести конкретный преподаватель
    // Используется для фильтрации дисциплин при добавлении учебной нагрузки
    public class TeacherDiscipline : ReactiveObject
    {
        private Guid _id;
        private Guid _teacherId;
        private Guid _disciplineId;

        public TeacherDiscipline()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public Guid TeacherId
        {
            get => _teacherId;
            set => this.RaiseAndSetIfChanged(ref _teacherId, value);
        }

        public Guid DisciplineId
        {
            get => _disciplineId;
            set => this.RaiseAndSetIfChanged(ref _disciplineId, value);
        }

        public string ToFileString()
        {
            return $"{Id}|{TeacherId}|{DisciplineId}";
        }

        public static TeacherDiscipline FromFileString(string line)
        {
            var parts = line.Split('|');
            return new TeacherDiscipline
            {
                Id = Guid.Parse(parts[0]),
                TeacherId = Guid.Parse(parts[1]),
                DisciplineId = Guid.Parse(parts[2])
            };
        }
    }
}
