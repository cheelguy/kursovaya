using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using UniversityIS.ViewModels;
using UniversityIS.Views;

namespace UniversityIS;

// Главный класс приложения
// Инициализирует XAML и создает главное окно с ViewModel
public partial class App : Application
{
    // Загружает XAML ресурсы приложения
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Вызывается после инициализации фреймворка
    // Создает главное окно и настраивает приложение
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Отключаем дублирующую валидацию от Avalonia
            // Используем только нашу собственную валидацию
            DisableAvaloniaDataAnnotationValidation();
            
            // Создаем главное окно и привязываем к нему ViewModel
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    // Отключает встроенную валидацию Avalonia для избежания конфликтов
    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}