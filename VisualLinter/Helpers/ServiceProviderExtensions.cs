using Microsoft.VisualStudio.ComponentModelHost;
using System;
using System.Linq;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class ServiceProviderExtensions
    {
        public static T GetMefService<T>(this IServiceProvider serviceProvider) where T : class
        {
            var componentModel = serviceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            return componentModel?.GetExtensions<T>().SingleOrDefault();
        }
    }
}