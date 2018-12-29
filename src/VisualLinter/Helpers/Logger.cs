using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;

namespace jwldnr.VisualLinter.Helpers
{
    public interface ILogger
    {
        void WriteLine(string message);
    }

    [Export(typeof(ILogger))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class Logger : ILogger
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        internal Logger([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ??
                throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void WriteLine(string message)
        {
            OutputWindowHelper.WriteLine(_serviceProvider, message);
        }
    }
}
