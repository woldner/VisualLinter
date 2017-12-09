using jwldnr.VisualLinter.Linting;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace jwldnr.VisualLinter.Tagging
{
    [Export(typeof(IViewTaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType("any")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TextViewRole(PredefinedTextViewRoles.Analyzable)]
    public sealed class TaggerProvider : IViewTaggerProvider, ITableDataSource, IDisposable
    {
        private readonly ILinter _linter;
        private readonly List<SinkManager> _managers = new List<SinkManager>();
        private readonly Dictionary<string, Func<bool>> _optionsMap = new Dictionary<string, Func<bool>>();
        private readonly TaggerManager _taggers = new TaggerManager();

        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private ITableManager _tableManager;

        public string DisplayName => "VisualLinter";
        public string Identifier => "VisualLinter";
        public string SourceTypeIdentifier => StandardTableDataSources.ErrorTableDataSource;

        [ImportingConstructor]
        public TaggerProvider(
            [Import] ITableManagerProvider tableManagerProvider,
            [Import] ITextDocumentFactoryService textDocumentFactoryService,
            [Import] IVisualLinterOptions visualLinterOptions,
            [Import] ILinter linter)
        {
            _tableManager = tableManagerProvider
                .GetTableManager(StandardTables.ErrorsTable);

            _textDocumentFactoryService = textDocumentFactoryService;

            _linter = linter;

            _optionsMap.Add(".html", () => visualLinterOptions.EnableHtmlLanguageSupport);
            _optionsMap.Add(".js", () => visualLinterOptions.EnableJavaScriptLanguageSupport);
            _optionsMap.Add(".jsx", () => visualLinterOptions.EnableReactLanguageSupport);
            _optionsMap.Add(".vue", () => visualLinterOptions.EnableVueLanguageSupport);

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

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer != textView.TextBuffer || typeof(IErrorTag) != typeof(T))
                return null;

            if (!TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out var document))
                return null;

            var filePath = document.FilePath;
            var extension = Path.GetExtension(filePath)?.ToLowerInvariant();

            if (null == extension)
                return null;

            if (false == _optionsMap.ContainsKey(extension))
                return null;

            if (false == _optionsMap.TryGetValue(extension, out var optionValue))
                return null;

            if (false == optionValue())
                return null;

            lock (_taggers)
            {
                if (false == _taggers.Exists(filePath))
                    return new LinterTagger(this, _linter, buffer, document) as ITagger<T>;

                if (false == _taggers.TryGetValue(filePath, out var tagger))
                    return null;

                return tagger as ITagger<T>;
            }
        }

        public void Dispose()
        {
            _tableManager.RemoveSource(this);
            _tableManager = null;
        }

        public IDisposable Subscribe(ITableDataSink sink)
        {
            return new SinkManager(this, sink);
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

        internal void AddTagger(LinterTagger tagger)
        {
            lock (_managers)
            {
                _taggers.Add(tagger);

                foreach (var manager in _managers)
                    manager.AddFactory(tagger.Factory);
            }
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