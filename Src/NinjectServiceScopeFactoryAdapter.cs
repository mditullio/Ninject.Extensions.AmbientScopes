using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ninject.Extensions.AmbientScopes
{
    public class NinjectServiceScopeFactoryAdapter : IServiceScopeFactory
    {
        private readonly IKernel _kernel;

        public NinjectServiceScopeFactoryAdapter(IKernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        public IServiceScope CreateScope()
        {
            return _kernel.Get<NinjectServiceScopeAdapter>();
        }
    }
}
