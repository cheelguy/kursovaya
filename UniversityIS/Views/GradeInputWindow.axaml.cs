using Avalonia.Controls;
using Avalonia.Interactivity;

namespace UniversityIS.Views
{
    public partial class GradeInputWindow : Window
    {
        public GradeInputWindow()
        {
            InitializeComponent();
        }

        private void OnSaveClick(object? sender, RoutedEventArgs e)
        {
            Close(true); // Закрыть окно с результатом "сохранено"
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            Close(false); // Закрыть окно без сохранения
        }
    }
}
