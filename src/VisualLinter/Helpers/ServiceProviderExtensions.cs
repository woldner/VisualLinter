using Microsoft.VisualStudio.ComponentModelHost;
using System;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class ServiceProviderExtensions
    {
        internal static T GetMefService<T>(this IServiceProvider serviceProvider) where T : class
        {
            var componentModel = serviceProvider.GetService(typeof(SComponentModel)) as IComponentModel;

            return componentModel?.GetService<T>();
        }
    }
}
