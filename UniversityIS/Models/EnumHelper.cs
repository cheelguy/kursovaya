using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityIS.Models
{
    // Обертка для enum значений с русским названием
    // Используется для отображения enum в ComboBox с локализованными названиями
    public class EnumItem<T> where T : Enum
    {
        public T Value { get; set; }
        public string Display { get; set; }
        
        public EnumItem(T value, string display)
        {
            Value = value;
            Display = display;
        }
        
        public override string ToString() => Display;
    }
    
    // Вспомогательный класс для создания списков enum значений с русскими названиями
    // Используется во всех ViewModel для заполнения ComboBox
    public static class EnumHelper
    {
        public static List<EnumItem<TeacherPosition>> GetTeacherPositions()
        {
            return Enum.GetValues<TeacherPosition>()
                .Select(p => new EnumItem<TeacherPosition>(p, p.ToRussianString()))
                .ToList();
        }
        
        public static List<EnumItem<AcademicDegree>> GetAcademicDegrees()
        {
            return Enum.GetValues<AcademicDegree>()
                .Select(d => new EnumItem<AcademicDegree>(d, d.ToRussianString()))
                .ToList();
        }
        
        public static List<EnumItem<AcademicTitle>> GetAcademicTitles()
        {
            return Enum.GetValues<AcademicTitle>()
                .Select(t => new EnumItem<AcademicTitle>(t, t.ToRussianString()))
                .ToList();
        }
        
        public static List<EnumItem<LessonType>> GetLessonTypes()
        {
            return Enum.GetValues<LessonType>()
                .Select(l => new EnumItem<LessonType>(l, l.ToRussianString()))
                .ToList();
        }
        
        public static List<EnumItem<ControlForm>> GetControlForms()
        {
            return Enum.GetValues<ControlForm>()
                .Select(c => new EnumItem<ControlForm>(c, c.ToRussianString()))
                .ToList();
        }
    }
}
