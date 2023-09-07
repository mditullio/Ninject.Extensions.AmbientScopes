using Microsoft.Extensions.DependencyInjection;
using Ninject.Extensions.AmbientScopes.Tests.Mocks;

namespace Ninject.Extensions.AmbientScopes.Tests
{
    public class NinjectServiceScopeFactoryAdapterTests
    {

        [Fact]
        public async Task CanResolveServiceProviderWithoutScope()
        {
            var kernel = new StandardKernel();
            kernel.Bind<MyServiceA>().ToSelf().InSingletonScope();
            kernel.Bind<MyServiceB>().ToSelf().InSingletonScope();
            kernel.Bind<MyServiceC>().ToSelf().InSingletonScope();

            IServiceProvider serviceProvider1 = kernel;
            IServiceProvider serviceProvider2 = kernel.GetService<IServiceProvider>();
            IServiceProvider serviceProvider3 = kernel.GetService<IServiceProvider>();
            await Task.Yield();

            MyServiceA instance1 = serviceProvider1.GetService<MyServiceA>();
            MyServiceA instance2 = serviceProvider2.GetService<MyServiceA>();
            MyServiceA instance3 = serviceProvider3.GetService<MyServiceA>();
            await Task.Yield();

            Assert.Equal(instance1, instance2);
            Assert.Equal(instance1, instance3);
            Assert.Equal(serviceProvider1, serviceProvider2);
            Assert.Equal(serviceProvider1, serviceProvider3);
        }

        [Fact]
        public void CanResolveServiceProviderWithinScope()
        {
            var kernel = new StandardKernel();

            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceB>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceC>().ToSelf().InSingletonScope();

            var serviceScope = kernel.GetService<IServiceScope>();
            var serviceProvider1 = serviceScope.ServiceProvider;
            var serviceProvider2 = serviceScope.ServiceProvider.GetService<IServiceProvider>();

            var instanceWithoutScope = kernel.GetService<MyServiceA>();
            var instanceInScope1 = serviceProvider1.GetService<MyServiceA>();
            var instanceInScope2 = serviceProvider2.GetService<MyServiceA>();

            Assert.NotEqual(instanceWithoutScope, instanceInScope1);
            Assert.Equal(instanceInScope1, instanceInScope2);
            Assert.Equal(serviceProvider1, serviceProvider2);
        }

        [Fact]
        public async Task CreatingServiceScopeDoesNotAffectAmbientScope()
        {
            var kernel = new StandardKernel();
            kernel.LoadAmbientScopeModule();
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceB>().ToSelf().InSingletonScope();
            kernel.Bind<MyServiceC>().ToSelf().InSingletonScope();

            IServiceScope serviceScope = kernel.CreateScope();
            await Task.Yield();

            var instanceWithoutScope = kernel.Get<MyServiceA>();
            await Task.Yield();

            var instanc1WithinScope1 = serviceScope.ServiceProvider.GetService<MyServiceA>();
            await Task.Yield();

            var instanceWithinScope2 = serviceScope.ServiceProvider.GetService<MyServiceA>();
            await Task.Yield();

            Assert.Null(kernel.Get<AmbientScopeManager>().Current);
            Assert.NotEqual(instanc1WithinScope1, instanceWithoutScope);
            Assert.Equal(instanc1WithinScope1, instanceWithinScope2);
        }

        [Fact]
        public async Task ServiceScopesResolveInstancesInIsolation()
        {
            var kernel = new StandardKernel();
            kernel.LoadAmbientScopeModule();
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceB>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceC>().ToSelf().InAmbientScope();

            IServiceScope serviceScope1 = kernel.CreateScope();
            IServiceScope serviceScope2 = kernel.CreateScope();

            // Instances resolved in same ServiceScope are same
            // Instances resovled in different ServiceScopes are different

            MyServiceA myServiceA1 = serviceScope1.ServiceProvider.GetService<MyServiceA>();
            MyServiceA myServiceA2 = await Task.Run(() => serviceScope1.ServiceProvider.GetService<MyServiceA>());

            MyServiceA myServiceA3 = serviceScope2.ServiceProvider.GetService<MyServiceA>();
            MyServiceA myServiceA4 = await Task.Run(() => serviceScope2.ServiceProvider.GetService<MyServiceA>());

            Assert.Same(myServiceA1, myServiceA2);
            Assert.Same(myServiceA1.MyServiceB.MyServiceC, myServiceA1.MyServiceC);

            Assert.Same(myServiceA3, myServiceA4);
            Assert.Same(myServiceA3.MyServiceB.MyServiceC, myServiceA3.MyServiceC);

            Assert.NotSame(myServiceA1, myServiceA3);
            Assert.NotSame(myServiceA1.MyServiceB.MyServiceC, myServiceA3.MyServiceC);

            Assert.False(myServiceA1.IsDisposed);
            Assert.False(myServiceA1.MyServiceB.IsDisposed);
            Assert.False(myServiceA1.MyServiceC.IsDisposed);

            Assert.False(myServiceA2.IsDisposed);
            Assert.False(myServiceA3.MyServiceB.IsDisposed);
            Assert.False(myServiceA3.MyServiceC.IsDisposed);

            // Disposing ServiceScopes will dispose automatically instances

            serviceScope1.Dispose();
            serviceScope2.Dispose();

            Assert.True(myServiceA1.IsDisposed);
            Assert.True(myServiceA1.MyServiceB.IsDisposed);
            Assert.True(myServiceA1.MyServiceC.IsDisposed);

            Assert.True(myServiceA3.IsDisposed);
            Assert.True(myServiceA3.MyServiceB.IsDisposed);
            Assert.True(myServiceA3.MyServiceC.IsDisposed);
        }


        [Fact]
        public async Task AmbientScopeIsNotCorruptedWhenGetServiceThrowsException()
        {
            var kernel = new StandardKernel();
            kernel.LoadAmbientScopeModule();
            kernel.Bind<MyBrokenService>().ToSelf().InAmbientScope();

            // Assume there's an existing ambient scope
            var outerScope = kernel.BeginAmbientScope();
            await Task.Yield();

            // Creating ServiceScope does not affect current scope
            IServiceScope serviceScope1 = kernel.CreateScope();
            Assert.Equal(outerScope, kernel.Get<AmbientScopeManager>().Current);
            await Task.Yield();

            // If an error occurs when resolving an instance, the current scope is not affected
            Assert.Throws<InvalidOperationException>(() => serviceScope1.ServiceProvider.GetService<MyBrokenService>());
            Assert.Equal(outerScope, kernel.Get<AmbientScopeManager>().Current);
        }


        [Fact]
        public async Task CanGetCurrentScopeUsingKernelExtensions()
        {
            var kernel = new StandardKernel();
            kernel.LoadAmbientScopeModule();
            var ambientScopeManager = kernel.Get<AmbientScopeManager>();

            await Task.Yield();

            Assert.Equal(ambientScopeManager.Current, kernel.GetAmbientScope());

            using var scope = kernel.CreateScope();
            await Task.Yield();

            Assert.Equal(ambientScopeManager.Current, kernel.GetAmbientScope());
        }

    }
}
