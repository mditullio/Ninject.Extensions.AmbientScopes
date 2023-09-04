using Microsoft.Extensions.DependencyInjection;
using Ninject.Extensions.AmbientScopes.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ninject.Extensions.AmbientScopes.Tests.AmbientScopeNinjectTests;

namespace Ninject.Extensions.AmbientScopes.Tests
{
    public class NinjectServiceScopeFactoryAdapterTests
    {

        [Fact]
        public async Task ServiceScopeAdaptersResolveInstancesInIsolation()
        {
            var kernel = new StandardKernel();
            kernel.UseAmbientScopes();
            kernel.Bind<IServiceScopeFactory>().To<NinjectServiceScopeFactoryAdapter>().InSingletonScope();
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();

            IServiceScope serviceScope1 = kernel.CreateScope();
            Assert.Null(kernel.Get<AmbientScopeProvider>().Current);

            IServiceScope serviceScope2 = kernel.CreateScope();
            Assert.Null(kernel.Get<AmbientScopeProvider>().Current);

            MyServiceA instance1 = serviceScope1.ServiceProvider.GetService<MyServiceA>();
            MyServiceA instance2 = await Task.Run(() => serviceScope1.ServiceProvider.GetService<MyServiceA>());
            MyServiceA instance3 = serviceScope2.ServiceProvider.GetService<MyServiceA>();
            MyServiceA instance4 = await Task.Run(() => serviceScope2.ServiceProvider.GetService<MyServiceA>());

            Assert.Same(instance1, instance2);
            Assert.Same(instance3, instance4);
            Assert.NotSame(instance1, instance3);
            Assert.False(instance1.IsDisposed);
            Assert.False(instance2.IsDisposed);

            serviceScope1.Dispose();
            serviceScope2.Dispose();

            Assert.True(instance1.IsDisposed);
            Assert.True(instance2.IsDisposed);
        }

    }
}
