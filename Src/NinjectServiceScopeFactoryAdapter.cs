using Microsoft.Extensions.DependencyInjection;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject.Extensions.AmbientScopes
{
    public class NinjectServiceScopeFactoryAdapter : IServiceScopeFactory
    {
        public IKernel Kernel { get; }

        public NinjectServiceScopeFactoryAdapter(IKernel kernel)
        {
            Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        public IServiceScope CreateScope()
        {
            return new NinjectServiceScopeAdapter(Kernel);
        }
    }
}
