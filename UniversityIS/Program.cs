using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace UniversityIS;

// Точка входа в приложение
// Инициализирует Avalonia UI и запускает приложение
sealed class Program
{
    // Главный метод приложения - точка входа
    // Настраивает и запускает Avalonia приложение
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Настройка Avalonia приложения
    // Конфигурирует платформу, шрифты, логирование и ReactiveUI
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
