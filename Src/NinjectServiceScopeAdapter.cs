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

        public AmbientScopeManager AmbientScopeManager { get; }

        public AmbientScope AmbientScope { get; }

        public NinjectServiceScopeAdapter(IKernel kernel)
        {
            Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            AmbientScopeManager = kernel.Get<AmbientScopeManager>();
            AmbientScope = new AmbientScope();
            ServiceProvider = new NinjectServiceProviderAdapter(Kernel, AmbientScopeManager, AmbientScope);
        }

        public void Dispose()
        {
            AmbientScope.Dispose();
        }

    }
}
