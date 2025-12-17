using System;
using ReactiveUI;

namespace UniversityIS.Models
{
    // Базовый класс для всех моделей данных приложения
    // Наследуется от ReactiveObject для поддержки реактивного программирования
    // Содержит общие свойства и методы для всех моделей
    public abstract class ModelBase : ReactiveObject
    {
        private Guid _id;
        
        // Уникальный идентификатор модели
        public Guid Id 
        { 
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }
        
        protected ModelBase()
        {
            Id = Guid.NewGuid();
        }
        
        // Абстрактный метод преобразования объекта в строку для сохранения в текстовый файл
        // Каждый дочерний класс должен реализовать свой формат сериализации
        public abstract string ToFileString();
        
        // Виртуальный метод для возврата строкового представления объекта
        // Может быть переопределен в дочерних классах
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}

