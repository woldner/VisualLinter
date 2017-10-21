using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Windows;

namespace jwldnr.VisualLinter.Options
{
    internal class OptionsDialogPage : UIElementDialogPage
    {
        protected override UIElement Child => _optionsDialogControl
            ?? (_optionsDialogControl = new OptionsDialogPageControl());

        internal const string PageName = "Options";

        private OptionsDialogPageControl _optionsDialogControl;

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            _optionsDialogControl.ViewModel.Initiailize();
        }

        protected override void OnApply(PageApplyEventArgs args)
        {
            if (args.ApplyBehavior == ApplyKind.Apply)
                _optionsDialogControl.ViewModel.Apply();

            base.OnApply(args);
        }
    }
}