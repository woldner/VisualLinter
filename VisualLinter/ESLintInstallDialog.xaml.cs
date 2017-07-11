using System.ComponentModel;
using System.Windows;
using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Shell;

namespace jwldnr.VisualLinter
{
    public partial class ESLintInstallDialog
    {
        public ESLintInstallDialog()
        {
            InitializeComponent();
        }

        private void ClickYes(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            var settings = ServiceProvider.GlobalProvider.GetMefService<IVisualLinterSettings>();
            settings.SkipInstallDialog = SkipInstallDialogCheckBox.IsChecked ?? false;
        }
    }
}