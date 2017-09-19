using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Windows;

namespace jwldnr.VisualLinter
{
    internal class OptionsDialogPage : UIElementDialogPage
    {
        protected override UIElement Child => _optionsDialogControl
            ?? (_optionsDialogControl = new OptionsDialogPageControl());

        internal const string PageName = "General";

        private readonly IVisualLinterOptions _options;
        private OptionsDialogPageControl _optionsDialogControl;

        public OptionsDialogPage()
        {
            _options = ServiceProvider.GlobalProvider.GetMefService<IVisualLinterOptions>()
                ?? throw new Exception("fatal: unable to retrieve options");
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            _optionsDialogControl.UseGlobalEslint = _options.UseGlobalEslint;
            _optionsDialogControl.UsePersonalConfig = _options.UsePersonalConfig;
            _optionsDialogControl.DisableIgnorePath = _options.DisableIgnorePath;
        }

        protected override void OnApply(PageApplyEventArgs args)
        {
            if (args.ApplyBehavior == ApplyKind.Apply)
            {
                _options.UseGlobalEslint = _optionsDialogControl.UseGlobalEslint;
                _options.UsePersonalConfig = _optionsDialogControl.UsePersonalConfig;
                _options.DisableIgnorePath = _optionsDialogControl.DisableIgnorePath;
            }

            base.OnApply(args);
        }
    }
}