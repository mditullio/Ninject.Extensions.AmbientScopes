using System;

namespace Ninject.Extensions.AmbientScopes
{
    public class NinjectServiceProviderAdapter : IServiceProvider
    {

        private readonly IKernel _kernel;

        private readonly AmbientScopeManager _ambientScopeManager;

        private readonly AmbientScope _ambientScope;

        public NinjectServiceProviderAdapter(IKernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _ambientScopeManager = _kernel.Get<AmbientScopeManager>();
            _ambientScope = _ambientScopeManager.Current;
        }

        public object GetService(Type serviceType)
        {
            return _ambientScopeManager.ExecuteInScope(_ambientScope, () => _kernel.GetService(serviceType));
        }

    }
}
