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
            : this(ServiceProvider.GlobalProvider.GetMefService<IVisualLinterOptions>())
        {
        }

        public OptionsDialogPage(IVisualLinterOptions options)
        {
            _options = options
                ?? throw new ArgumentNullException(nameof(options));
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            _optionsDialogControl.UseGlobalConfig = _options.UseGlobalConfig;
        }

        protected override void OnApply(PageApplyEventArgs args)
        {
            if (args.ApplyBehavior == ApplyKind.Apply)
                _options.UseGlobalConfig = _optionsDialogControl.UseGlobalConfig;

            base.OnApply(args);
        }
    }
}