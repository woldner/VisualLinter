using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Windows;

namespace jwldnr.VisualLinter.Views
{
    internal class AdvancedOptionsDialogPage : UIElementDialogPage
    {
        internal const string PageName = "Advanced";

        private AdvancedOptionsDialogPageControl _advancedOptionsDialogControl;

        protected override UIElement Child => _advancedOptionsDialogControl
            ?? (_advancedOptionsDialogControl = new AdvancedOptionsDialogPageControl());

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            _advancedOptionsDialogControl.ViewModel.Initiailize();
        }

        protected override void OnApply(PageApplyEventArgs args)
        {
            if (args.ApplyBehavior == ApplyKind.Apply)
                _advancedOptionsDialogControl.ViewModel.Apply();

            base.OnApply(args);
        }
    }
}