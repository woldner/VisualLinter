﻿using jwldnr.VisualLinter.Tagging;
using Microsoft.VisualStudio.Shell.TableManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Moq;

namespace jwldnr.VisualLinter.Tests.Tagging
{
    [TestClass]
    public class TaggerProviderTests
    {
        private Mock<ITextDocument> _mockTextDocument;

        private TaggerProvider _provider;

        [TestInitialize]
        public void Initialize()
        {
            var mockTableManagerProvider = new Mock<ITableManagerProvider>();
            mockTableManagerProvider
                .Setup(t => t.GetTableManager(StandardTables.ErrorsTable))
                .Returns(new Mock<ITableManager>().Object);

            var tableManagerProvider = mockTableManagerProvider.Object;

            var mockTextDocumentFactoryService = new Mock<ITextDocumentFactoryService>();
            var textDocumentFactoryService = mockTextDocumentFactoryService.Object;

            _mockTextDocument = new Mock<ITextDocument>();

            // ReSharper disable once RedundantAssignment
            var textDocument = _mockTextDocument.Object;

            mockTextDocumentFactoryService
                .Setup(t => t.TryGetTextDocument(It.IsAny<ITextBuffer>(), out textDocument))
                .Returns(true);

            var mockIVisualLinterOptions = new Mock<IVisualLinterOptions>();
            mockIVisualLinterOptions.Setup(o => o.EnableJsLanguageSupport)
                .Returns(true);

            var visualLinterOptions = mockIVisualLinterOptions.Object;

            _provider = new TaggerProvider(
                tableManagerProvider,
                textDocumentFactoryService,
                visualLinterOptions);
        }

        [TestMethod]
        public void CreateTagger_should_create_tagger_for_javascript()
        {
            var tagger = CreateTagger("foo.js");
            Assert.IsNotNull(tagger);
        }

        [TestMethod]
        public void CreateTagger_should_return_null_for_anything_but_javascript()
        {
            var tagger1 = CreateTagger("foo.js");
            Assert.IsNotNull(tagger1);

            var tagger2 = CreateTagger("foo.cs");
            Assert.IsNull(tagger2);

            var tagger3 = CreateTagger("foo.java");
            Assert.IsNull(tagger3);
        }

        private ITagger<IErrorTag> CreateTagger(string filePath)
        {
            _mockTextDocument
                .Setup(d => d.FilePath)
                .Returns(filePath);

            var mockTextBuffer = new Mock<ITextBuffer>();
            var textBuffer = mockTextBuffer.Object;

            var mockTextDataModel = new Mock<ITextDataModel>();
            var textDataModel = mockTextDataModel.Object;

            var mockTextView = new Mock<ITextView>();
            mockTextView
                .Setup(t => t.TextBuffer)
                .Returns(textBuffer);
            mockTextView
                .Setup(t => t.TextDataModel)
                .Returns(textDataModel);

            var textView = mockTextView.Object;

            return _provider.CreateTagger<IErrorTag>(textView, textBuffer);
        }
    }
}
