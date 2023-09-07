using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ninject.Extensions.AmbientScopes
{
    public class NinjectServiceScopeAdapter : IServiceScope
    {

        private readonly IKernel _kernel;

        private readonly AmbientScopeManager _ambientScopeManager;

        private readonly AmbientScope _ambientScope;

        public IServiceProvider ServiceProvider { get; }

        public NinjectServiceScopeAdapter(IKernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _ambientScopeManager = _kernel.Get<AmbientScopeManager>();
            _ambientScope = new AmbientScope();
            ServiceProvider = _ambientScopeManager.ExecuteInScope(_ambientScope, () => _kernel.Get<NinjectServiceProviderAdapter>());
        }

        public void Dispose()
        {
            _ambientScope.Dispose();
        }

    }
}
