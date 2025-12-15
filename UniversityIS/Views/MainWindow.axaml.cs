using Avalonia.Controls;
using UniversityIS.ViewModels;

namespace UniversityIS.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Автосохранение при закрытии окна
        Closing += (sender, e) =>
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.OnClosing();
            }
        };
    }
}