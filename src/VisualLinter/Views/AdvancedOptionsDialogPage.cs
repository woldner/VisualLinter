using Microsoft.VisualStudio.Shell;
using System.Windows;

namespace jwldnr.VisualLinter.Views
{
    internal class AdvancedOptionsDialogPage : UIElementDialogPage
    {
        internal const string PageName = "Advanced";

        private readonly AdvancedOptionsDialogPageControl _advancedOptionsDialogControl;

        protected override UIElement Child => _advancedOptionsDialogControl;

        public AdvancedOptionsDialogPage()
        {
            _advancedOptionsDialogControl = new AdvancedOptionsDialogPageControl();
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();

            _advancedOptionsDialogControl.ViewModel.Load();
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();

            _advancedOptionsDialogControl.ViewModel.Save();
        }
    }
}
