using System;
using Microsoft.VisualStudio.ComponentModelHost;

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

        internal static T GetService<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            if (null == serviceProvider)
                throw new ArgumentNullException(nameof(serviceProvider));

            return serviceProvider.GetService(typeof(T)) as T;
        }

        internal static TU GetService<T, TU>(this IServiceProvider serviceProvider)
            where T : class
            where TU : class
        {
            if (null == serviceProvider)
                throw new ArgumentNullException(nameof(serviceProvider));

            return serviceProvider.GetService(typeof(T)) as TU;
        }
    }
}
