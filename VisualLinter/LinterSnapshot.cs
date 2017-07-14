using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace jwldnr.VisualLinter
{
    internal class LinterSnapshot : WpfTableEntriesSnapshotBase
    {
        public override int Count => _markers.Count;
        public override int VersionNumber { get; }

        internal IEnumerable<MessageMarker> Markers => _readonlyMarkers;
        internal LinterSnapshot NextSnapshot;

        private readonly string _filePath;
        private readonly IList<MessageMarker> _markers;
        private readonly IReadOnlyCollection<MessageMarker> _readonlyMarkers;

        private string _projectName;

        internal LinterSnapshot(string filePath, int versionNumber, IEnumerable<MessageMarker> markers)
        {
            _filePath = filePath;
            VersionNumber = versionNumber;

            _markers = new List<MessageMarker>(markers);
            _readonlyMarkers = new ReadOnlyCollection<MessageMarker>(_markers);
        }

        public override bool CanCreateDetailsContent(int index)
        {
            // todo, when fix is available..
            // return return null != _markers[index].Message.Fix.Text;
            return false;
        }

        public override bool TryCreateDetailsStringContent(int index, out string content)
        {
            // todo, use the linter fix to provide a more detailed description
            content = _markers[index].Message.Message;
            return null != content;
        }

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if (index < 0 || _markers.Count <= index)
                return false;

            var marker = _markers[index];

            switch (columnName)
            {
                case StandardTableKeyNames.BuildTool:
                    // todo get analyzer name
                    content = Vsix.Name;
                    return true;

                case StandardTableKeyNames.Column:
                    var position = marker.Span.Start;
                    var line = position.GetContainingLine();
                    content = position.Position - line.Start.Position;
                    return true;

                case StandardTableKeyNames.DocumentName:
                    content = _filePath;
                    return null != content;

                case StandardTableKeyNames.ErrorCodeToolTip:
                case StandardTableKeyNames.HelpLink:
                    content = GetRuleUrl(marker.Message.RuleId);
                    return null != content;

                case StandardTableKeyNames.ErrorCode:
                    content = $"({marker.Message.RuleId})";
                    return true;

                case StandardTableKeyNames.ErrorSeverity:
                    content = GetErrorCategory(marker.Message.IsFatal);
                    return true;

                case StandardTableKeyNames.ErrorSource:
                    content = marker.Message.Source;
                    return null != content;

                case StandardTableKeyNames.Line:
                    content = marker.Span.Start.GetContainingLine().LineNumber;
                    return true;

                case StandardTableKeyNames.ProjectName:
                    if (string.IsNullOrEmpty(_projectName))
                        _projectName = ProjectHelper.GetProjectName(_filePath);
                    content = _projectName;
                    return null != content;

                case StandardTableKeyNames.Text:
                    content = marker.Message.Message;
                    return null != content;

                default:
                    return false;
            }
        }

        private static __VSERRORCATEGORY GetErrorCategory(bool isFatal)
        {
            return isFatal
                ? __VSERRORCATEGORY.EC_ERROR
                : __VSERRORCATEGORY.EC_WARNING;
        }

        private static string GetRuleUrl(string ruleId)
        {
            return $"http://eslint.org/docs/rules/{ruleId}";
        }
    }
}