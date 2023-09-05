using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject.Extensions.AmbientScopes
{
    public class NinjectServiceProviderAdapter : IServiceProvider
    {

        public IKernel Kernel { get; }

        public AmbientScopeManager AmbientScopeManager { get; }

        public AmbientScope AmbientScope { get; }

        public NinjectServiceProviderAdapter(IKernel kernel, AmbientScopeManager scopeFactory, AmbientScope ambientScope)
        {
            Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            AmbientScopeManager = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            AmbientScope = ambientScope ?? throw new ArgumentNullException(nameof(ambientScope));
        }

        public object GetService(Type serviceType)
        {
            var previousScope = AmbientScopeManager.SetCurrent(AmbientScope);
            try
            {
                var service = Kernel.GetService(serviceType);
                return service;
            }
            finally
            {
                AmbientScopeManager.SetCurrent(previousScope);
            }
        }

    }
}
