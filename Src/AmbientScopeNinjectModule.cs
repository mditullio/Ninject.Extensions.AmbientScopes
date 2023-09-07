using Microsoft.Extensions.DependencyInjection;
using Ninject.Activation;
using Ninject.Modules;
using System;

namespace Ninject.Extensions.AmbientScopes
{
    public class AmbientScopeNinjectModule : NinjectModule
    {

        public static readonly string MODULE_NAME = typeof(AmbientScopeNinjectModule).FullName;

        public override string Name => MODULE_NAME;

        public override void Load()
        {
            // Ambient scope manager
            Kernel.Bind<AmbientScopeManager>().ToSelf().InSingletonScope();

            // Service scope factory
            Kernel.Bind<IServiceScopeFactory>().ToMethod(ResolveServiceScopeFactory).InSingletonScope();
            Kernel.Bind<NinjectServiceScopeFactoryAdapter>().ToSelf().InSingletonScope();

            // Service scope
            Kernel.Bind<IServiceScope>().ToMethod(ResolveServiceScope).InTransientScope();
            Kernel.Bind<NinjectServiceScopeAdapter>().ToSelf().InTransientScope();

            // Service provider
            Kernel.Bind<IServiceProvider>().ToMethod(ResolveServiceProvider).InAmbientOrSingletonScope();
            Kernel.Bind<NinjectServiceProviderAdapter>().ToSelf().InAmbientOrSingletonScope();
        }

        private static IServiceProvider ResolveServiceProvider(IContext context)
        {
            var ambientScope = context.Kernel.GetAmbientScope();
            if (ambientScope != null)
            {
                return context.Kernel.Get<NinjectServiceProviderAdapter>();
            }
            else
            {
                return context.Kernel;
            }
        }

        private static IServiceScope ResolveServiceScope(IContext context)
        {
            return context.Kernel.Get<NinjectServiceScopeAdapter>();
        }

        private static IServiceScopeFactory ResolveServiceScopeFactory(IContext context)
        {
            return context.Kernel.Get<NinjectServiceScopeFactoryAdapter>();
        }

    }
}
