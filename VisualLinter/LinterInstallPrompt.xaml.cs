using System.Windows;

namespace jwldnr.VisualLinter
{
    public partial class LinterInstallPrompt
    {
        public LinterInstallPrompt()
        {
            InitializeComponent();
        }

        private void ClickYes(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}