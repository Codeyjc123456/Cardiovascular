using System.Windows;

namespace Cardio.Views.SystemPage
{
    /// <summary>
    /// InputBoxWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputBoxWindow : Window
    {
        Window window = new Window();
        public string UserInput { get; private set; }

        public InputBoxWindow(string prompt)
        {
            this.Left = 1740/2;
            this.Top = 880/2;
            InitializeComponent();
            PromptTextBlock.Text = prompt;
            UserInput = string.Empty;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            UserInput = InputTextBox.Password;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
