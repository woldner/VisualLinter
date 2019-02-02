using jwldnr.VisualLinter.Linting;
using jwldnr.VisualLinter.Tagging;
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
        private Mock<IVisualLinterOptions> _mockOptions;
        private Mock<ITextDocument> _mockTextDocument;

        private TaggerProvider _provider;

        [TestMethod]
        public void CreateTagger_should_create_tagger_for_enabled_file_extensions()
        {
            _mockOptions
                .Setup(o => o.EnableHtmlLanguageSupport)
                .Returns(true);

            _mockOptions
                .Setup(o => o.EnableJavaScriptLanguageSupport)
                .Returns(true);

            _mockOptions
                .Setup(o => o.EnableReactLanguageSupport)
                .Returns(true);

            _mockOptions
                .Setup(o => o.EnableVueLanguageSupport)
                .Returns(true);

            var tagger1 = CreateTagger("foo.html");
            Assert.IsNotNull(tagger1);

            var tagger2 = CreateTagger("foo.js");
            Assert.IsNotNull(tagger2);

            var tagger3 = CreateTagger("foo.jsx");
            Assert.IsNotNull(tagger3);

            var tagger4 = CreateTagger("foo.vue");
            Assert.IsNotNull(tagger4);
        }

        [TestMethod]
        public void CreateTagger_should_create_tagger_for_javascript()
        {
            var tagger = CreateTagger("foo.js");
            Assert.IsNotNull(tagger);
        }

        [TestMethod]
        public void CreateTagger_should_return_null_for_disabled_file_extensions()
        {
            _mockOptions
                .Setup(o => o.EnableHtmlLanguageSupport)
                .Returns(false);

            _mockOptions
                .Setup(o => o.EnableJavaScriptLanguageSupport)
                .Returns(false);

            _mockOptions
                .Setup(o => o.EnableReactLanguageSupport)
                .Returns(false);

            _mockOptions
                .Setup(o => o.EnableVueLanguageSupport)
                .Returns(false);

            var tagger1 = CreateTagger("foo.html");
            Assert.IsNull(tagger1);

            var tagger2 = CreateTagger("foo.js");
            Assert.IsNull(tagger2);

            var tagger3 = CreateTagger("foo.jsx");
            Assert.IsNull(tagger3);

            var tagger4 = CreateTagger("foo.vue");
            Assert.IsNull(tagger4);
        }

        [TestMethod]
        public void CreateTagger_should_return_null_for_not_supported_file_extensions()
        {
            var tagger1 = CreateTagger("foo.js");
            Assert.IsNotNull(tagger1);

            var tagger2 = CreateTagger("foo.cs");
            Assert.IsNull(tagger2);

            var tagger3 = CreateTagger("foo.java");
            Assert.IsNull(tagger3);
        }

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

            _mockOptions = new Mock<IVisualLinterOptions>();
            _mockOptions
                .Setup(o => o.EnableJavaScriptLanguageSupport)
                .Returns(true);

            var visualLinterOptions = _mockOptions.Object;

            var mockLinter = new Mock<ILinter>();
            var linter = mockLinter.Object;

            _provider = new TaggerProvider(
                tableManagerProvider,
                textDocumentFactoryService,
                visualLinterOptions,
                linter);
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
