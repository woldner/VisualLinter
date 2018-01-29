using Microsoft.VisualStudio.Shell;
using System.Windows;

namespace jwldnr.VisualLinter.Views
{
    internal class GeneralOptionsDialogPage : UIElementDialogPage
    {
        internal const string PageName = "General";

        private readonly GeneralOptionsDialogPageControl _generalOptionsDialogControl;

        protected override UIElement Child => _generalOptionsDialogControl;

        public GeneralOptionsDialogPage()
        {
            _generalOptionsDialogControl = new GeneralOptionsDialogPageControl();
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();

            _generalOptionsDialogControl.ViewModel.Load();
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();

            _generalOptionsDialogControl.ViewModel.Save();
        }
    }
}
