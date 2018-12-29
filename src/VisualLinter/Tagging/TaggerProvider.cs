using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
    public interface ILinterProvider
    {
        void Accept(string filePath, IEnumerable<EslintMessage> messages);
    }

    [Export(typeof(IViewTaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType("text")]
    [ContentType("projection")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TextViewRole(PredefinedTextViewRoles.Analyzable)]
    public sealed class TaggerProvider : IViewTaggerProvider, ITableDataSource, ILinterProvider, IDisposable
    {
        private readonly Dictionary<string, Func<bool>> _extensions =
            new Dictionary<string, Func<bool>>(StringComparer.OrdinalIgnoreCase);

        private readonly ILinter _linter;
        private readonly ILogger _logger;

        private readonly List<SinkManager> _managers = new List<SinkManager>();
        private readonly TaggerManager _taggers = new TaggerManager();

        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private ITableManager _tableManager;

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
                StandardTableColumnDefinitions.Text
            };

            _tableManager.AddSource(this, columns);
        }

        public void Dispose()
        {
            _tableManager.RemoveSource(this);
            _tableManager = null;
        }

        public void Accept(string filePath, IEnumerable<EslintMessage> messages)
        {
            try
            {
                UpdateMessages(filePath, messages);
            }
            catch (Exception e)
            {
                _logger.WriteLine(e.Message);
            }
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

            lock (_taggers)
            {
                if (_taggers.TryGetValue(filePath, out var tagger))
                    return tagger as ITagger<T>;

                return new LinterTagger(buffer, document, _logger, this) as ITagger<T>;
            }
        }

        public void UpdateMessages(string filePath, IEnumerable<EslintMessage> messages)
        {
            lock (_taggers)
            {
                if (false == _taggers.TryGetValue(filePath, out var tagger))
                    return;

                tagger.UpdateMessages(messages);
            }
        }

        internal void AddSinkManager(SinkManager manager)
        {
            lock (_managers)
            {
                _managers.Add(manager);

                foreach (var tagger in _taggers.Values)
                    manager.AddFactory(tagger.Factory);
            }
        }

        internal Task AddTagger(LinterTagger tagger, Func<Task> callback)
        {
            lock (_managers)
            {
                _taggers.Add(tagger);

                foreach (var manager in _managers)
                    manager.AddFactory(tagger.Factory);
            }

            return callback();
        }

        internal async Task Analyze(string filePath, CancellationToken token)
        {
            lock (_taggers)
            {
                if (false == _taggers.Exists(filePath))
                    return;
            }

            await _linter.LintAsync(this, filePath, token)
                .ConfigureAwait(false);
        }

        internal void RemoveSinkManager(SinkManager manager)
        {
            lock (_managers)
            {
                _managers.Remove(manager);
            }
        }

        internal void RemoveTagger(LinterTagger tagger)
        {
            lock (_managers)
            {
                _taggers.Remove(tagger);

                foreach (var manager in _managers)
                    manager.RemoveFactory(tagger.Factory);
            }
        }

        internal void Rename(string oldPath, string newPath)
        {
            lock (_taggers)
            {
                _taggers.Rename(oldPath, newPath);
            }
        }

        internal void UpdateAllSinks(ITableEntriesSnapshotFactory factory)
        {
            lock (_managers)
            {
                foreach (var manager in _managers)
                    manager.UpdateSink(factory);
            }
        }

        private bool TryGetTextDocument(ITextBuffer buffer, out ITextDocument document)
        {
            return _textDocumentFactoryService.TryGetTextDocument(buffer, out document);
        }
    }
}
