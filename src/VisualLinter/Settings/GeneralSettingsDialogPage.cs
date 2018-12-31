using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Shell;

namespace jwldnr.VisualLinter.Settings
{
    internal class GeneralSettingsDialogPage : UIElementDialogPage
    {
        public const string PageName = "General";

        private GeneralSettingsDialogControl _control;

        protected override UIElement Child
        {
            get
            {
                if (null != _control)
                    return _control;

                var sited = null != Site;
                Debug.Assert(sited, $"page {PageName} was not sited");

                var settings = Site.GetMefService<IVisualLinterSettings>();
                var logger = Site.GetMefService<ILogger>();

                _control = new GeneralSettingsDialogControl(settings, logger);
                return _control;
            }
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            _control.Load();
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
                _control.Save();

            base.OnApply(e);
        }
    }
}
