using Microsoft.VisualStudio.ComponentModelHost;
using System;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class ServiceProviderExtensions
    {
        internal static T GetMefService<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            var componentModel = serviceProvider.GetService(typeof(SComponentModel)) as IComponentModel;

            return componentModel?.GetService<T>();
        }

        internal static TU GetService<T, TU>(this IServiceProvider serviceProvider)
            where T : class
            where TU : class
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return serviceProvider.GetService(typeof(T)) as TU;
        }
    }
}
