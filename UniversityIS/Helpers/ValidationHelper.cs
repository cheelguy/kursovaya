using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace UniversityIS.Helpers
{
    // Вспомогательный класс для валидации вводимых данных
    // Содержит методы проверки корректности различных типов данных
    public static class ValidationHelper
    {
        // Проверяет, что строка содержит только буквы, пробелы, дефисы и точки
        // Используется для валидации имен, фамилий и названий без цифр
        public static bool IsValidName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Разрешаем буквы (русские и латинские), пробелы, дефисы, точки
            return Regex.IsMatch(name, @"^[а-яА-ЯёЁa-zA-Z\s\.\-]+$");
        }

        // Проверяет, что строка содержит только буквы, цифры, пробелы, дефисы и точки
        // Используется для валидации названий групп, дисциплин и других сущностей с цифрами
        public static bool IsValidNameWithNumbers(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Разрешаем буквы, цифры, пробелы, дефисы, точки
            return Regex.IsMatch(name, @"^[а-яА-ЯёЁa-zA-Z0-9\s\.\-]+$");
        }

        // Проверяет, что строка является корректным годом
        // Допустимый диапазон: 1900-2100
        public static bool IsValidYear(string? year)
        {
            if (string.IsNullOrWhiteSpace(year))
                return false;

            if (!int.TryParse(year, out int yearValue))
                return false;

            return yearValue >= 1900 && yearValue <= 2100;
        }

        // Проверяет, что строка является корректным номером курса
        // Допустимый диапазон: 1-6
        public static bool IsValidCourse(string? course)
        {
            if (string.IsNullOrWhiteSpace(course))
                return false;

            if (!int.TryParse(course, out int courseValue))
                return false;

            return courseValue >= 1 && courseValue <= 6;
        }

        // Проверяет, что строка является корректным номером семестра
        // Допустимый диапазон: 1-10
        public static bool IsValidSemester(string? semester)
        {
            if (string.IsNullOrWhiteSpace(semester))
                return false;

            if (!int.TryParse(semester, out int semesterValue))
                return false;

            return semesterValue >= 1 && semesterValue <= 10;
        }

        // Проверяет, что строка является корректным количеством часов
        // Допустимый диапазон: 1-1000
        public static bool IsValidHours(string? hours)
        {
            if (string.IsNullOrWhiteSpace(hours))
                return false;

            if (!int.TryParse(hours, out int hoursValue))
                return false;

            return hoursValue > 0 && hoursValue <= 1000;
        }

        // Возвращает понятное сообщение об ошибке валидации
        // fieldName - название поля, errorType - тип ошибки
        public static string GetErrorMessage(string fieldName, string errorType)
        {
            return errorType switch
            {
                "empty" => $"Поле \"{fieldName}\" не может быть пустым.",
                "invalid_name" => $"Поле \"{fieldName}\" должно содержать только буквы, пробелы, дефисы и точки (без цифр).",
                "invalid_name_with_numbers" => $"Поле \"{fieldName}\" содержит недопустимые символы.",
                "invalid_year" => $"Поле \"{fieldName}\" должно содержать корректный год (1900-2100).",
                "invalid_course" => $"Поле \"{fieldName}\" должно быть числом от 1 до 6.",
                "invalid_semester" => $"Поле \"{fieldName}\" должно быть числом от 1 до 10.",
                "invalid_hours" => $"Поле \"{fieldName}\" должно быть положительным числом (1-1000).",
                "not_selected" => $"Необходимо выбрать \"{fieldName}\".",
                _ => "Неизвестная ошибка валидации."
            };
        }
    }
}
