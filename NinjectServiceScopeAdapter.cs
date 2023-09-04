using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Ninject.Extensions.AmbientScopes
{
    public class NinjectServiceScopeAdapter : IServiceScope
    {
        public IServiceProvider ServiceProvider { get; }

        public IKernel Kernel { get; }

        public AmbientScopeProvider AmbientScopeProvider { get; }

        public AmbientScope AmbientScope { get; }

        public NinjectServiceScopeAdapter(IKernel kernel)
        {
            Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            AmbientScopeProvider = kernel.Get<AmbientScopeProvider>();
            AmbientScope = AmbientScopeProvider.BeginScope();
            AmbientScopeProvider.ResetScope(AmbientScope.Parent);
            ServiceProvider = new NinjectServiceProviderAdapter(Kernel, AmbientScopeProvider, AmbientScope);
        }

        public void Dispose()
        {
            AmbientScope.Dispose();
        }

    }
}
