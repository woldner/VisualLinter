using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Windows;

namespace jwldnr.VisualLinter.Views
{
    internal class GeneralOptionsDialogPage : UIElementDialogPage
    {
        internal const string PageName = "General";

        private GeneralOptionsDialogPageControl _generalOptionsDialogControl;

        protected override UIElement Child => _generalOptionsDialogControl
            ?? (_generalOptionsDialogControl = new GeneralOptionsDialogPageControl());

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            _generalOptionsDialogControl.ViewModel.Initiailize();
        }

        protected override void OnApply(PageApplyEventArgs args)
        {
            if (args.ApplyBehavior == ApplyKind.Apply)
                _generalOptionsDialogControl.ViewModel.Apply();

            base.OnApply(args);
        }
    }
}