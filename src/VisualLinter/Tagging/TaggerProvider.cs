using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using jwldnr.VisualLinter.Helpers;
using jwldnr.VisualLinter.Linting;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace jwldnr.VisualLinter.Tagging
{
    [Export(typeof(IViewTaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType("text")]
    [ContentType("projection")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TextViewRole(PredefinedTextViewRoles.Analyzable)]
    public sealed class TaggerProvider : IViewTaggerProvider, ITableDataSource
    {
        private readonly Dictionary<string, Func<bool>> _extensions =
            new Dictionary<string, Func<bool>>(StringComparer.OrdinalIgnoreCase);

        private readonly ILinter _linter;
        private readonly ILogger _logger;

        private readonly ISet<SinkManager> _managers = new HashSet<SinkManager>();
        private readonly ITableManager _tableManager;

        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private readonly ISet<MessageTracker> _trackers = new HashSet<MessageTracker>();

        [ImportingConstructor]
        public TaggerProvider(
            [Import] ITableManagerProvider tableManagerProvider,
            [Import] ITextDocumentFactoryService textDocumentFactoryService,
            [Import] IVisualLinterSettings settings,
            [Import] ILogger logger,
            [Import] ILinter linter)
        {
            _tableManager = tableManagerProvider
                .GetTableManager(StandardTables.ErrorsTable);

            _textDocumentFactoryService = textDocumentFactoryService;

            _logger = logger;
            _linter = linter;

            _extensions.Add(".html", () => settings.EnableHtmlLanguageSupport);
            _extensions.Add(".js", () => settings.EnableJavaScriptLanguageSupport);
            _extensions.Add(".jsx", () => settings.EnableReactLanguageSupport);
            _extensions.Add(".vue", () => settings.EnableVueLanguageSupport);

            var columns = new[]
            {
                StandardTableColumnDefinitions.BuildTool,
                StandardTableColumnDefinitions.Column,
                StandardTableColumnDefinitions.DetailsExpander,
                StandardTableColumnDefinitions.DocumentName,
                StandardTableColumnDefinitions.ErrorCategory,
                StandardTableColumnDefinitions.ErrorCode,
                StandardTableColumnDefinitions.ErrorSeverity,
                StandardTableColumnDefinitions.ErrorSource,
                StandardTableColumnDefinitions.Line,
                StandardTableColumnDefinitions.ProjectName,
                StandardTableColumnDefinitions.Text
            };

            _tableManager.AddSource(this, columns);
        }

        public string DisplayName => "VisualLinter";
        public string Identifier => "VisualLinter";
        public string SourceTypeIdentifier => StandardTableDataSources.ErrorTableDataSource;

        public IDisposable Subscribe(ITableDataSink sink)
        {
            return new SinkManager(this, sink);
        }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer != textView.TextBuffer || typeof(IErrorTag) != typeof(T))
                return null;

            if (false == TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out var document))
                return null;

            var filePath = document.FilePath;
            var extension = Path.GetExtension(filePath)?.ToLowerInvariant();

            if (null == extension)
                return null;

            if (false == _extensions.ContainsKey(extension))
                return null;

            if (false == _extensions.TryGetValue(extension, out var enabled))
                return null;

            if (false == enabled())
                return null;

            var tracker = buffer.Properties.GetOrCreateSingletonProperty(typeof(MessageTracker),
                () => new MessageTracker(document, _logger, this));

            return new MessageTagger(tracker) as ITagger<T>;
        }

        internal void AddSinkManager(SinkManager manager)
        {
            lock (_managers)
            {
                _managers.Add(manager);

                foreach (var tracker in _trackers)
                    manager.AddFactory(tracker.Factory);
            }
        }

        internal void RemoveSinkManager(SinkManager manager)
        {
            lock (_managers)
            {
                _managers.Remove(manager);
            }
        }

        internal void UpdateAllSinks()
        {
            lock (_managers)
            {
                foreach (var manager in _managers)
                    manager.UpdateSink();
            }
        }

        internal void Analyze(IMessageTracker tracker, string filePath, CancellationToken token)
        {
            try
            {
                _linter.LintAsync(tracker, filePath, token);
            }
            catch (Exception e)
            {
                _logger.WriteLine(e.Message);
            }
        }

        internal void AddTracker(MessageTracker tracker)
        {
            lock (_managers)
            {
                _trackers.Add(tracker);

                foreach (var manager in _managers)
                    manager.AddFactory(tracker.Factory);
            }
        }

        internal void RemoveTracker(MessageTracker tracker)
        {
            lock (_managers)
            {
                _trackers.Remove(tracker);

                foreach (var manager in _managers)
                    manager.RemoveFactory(tracker.Factory);
            }
        }

        private bool TryGetTextDocument(ITextBuffer buffer, out ITextDocument document)
        {
            return _textDocumentFactoryService.TryGetTextDocument(buffer, out document);
        }
    }
}
