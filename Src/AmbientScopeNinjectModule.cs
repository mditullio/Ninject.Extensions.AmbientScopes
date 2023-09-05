using Microsoft.Extensions.DependencyInjection;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject.Extensions.AmbientScopes
{
    public class AmbientScopeNinjectModule : NinjectModule
    {

        public static readonly string MODULE_NAME = typeof(AmbientScopeNinjectModule).FullName;

        public override string Name => MODULE_NAME;

        public override void Load()
        {
            Kernel.Bind<AmbientScopeManager>().ToSelf().InSingletonScope();
            Kernel.Bind<IServiceScopeFactory>().To<NinjectServiceScopeFactoryAdapter>().InSingletonScope();
        }
    }
}
