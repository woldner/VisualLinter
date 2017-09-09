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
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TextViewRole(PredefinedTextViewRoles.Analyzable)]
    internal sealed class TaggerProvider : IViewTaggerProvider, ITableDataSource, IDisposable
    {
        public string DisplayName => "VisualLinter";
        public string Identifier => "VisualLinter";
        public string SourceTypeIdentifier => StandardTableDataSources.ErrorTableDataSource;

        private readonly List<SinkManager> _managers = new List<SinkManager>();
        private readonly TaggerManager _taggers = new TaggerManager();
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private readonly IVisualLinterOptions _visualLinterOptions;

        private ITableManager _tableManager;

        [ImportingConstructor]
        internal TaggerProvider(
            [Import] ITableManagerProvider tableManagerProvider,
            [Import] ITextDocumentFactoryService textDocumentFactoryService,
            [Import] IVisualLinterOptions visualLinterOptions)
        {
            _tableManager = tableManagerProvider
                .GetTableManager(StandardTables.ErrorsTable);

            _textDocumentFactoryService = textDocumentFactoryService;

            _visualLinterOptions = visualLinterOptions;

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
            if (".js" != extension)
                return null;

            lock (_taggers)
            {
                if (!_taggers.Exists(filePath))
                    return new LinterTagger(this, new Linter(_visualLinterOptions), buffer, document) as ITagger<T>;

                var result = _taggers.TryGetValue(filePath, out var tagger);
                if (false == result)
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