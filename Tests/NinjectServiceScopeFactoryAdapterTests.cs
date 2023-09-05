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
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceB>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceC>().ToSelf().InAmbientScope();

            // Creating ServiceScopes does not affect current scope

            IServiceScope serviceScope1 = kernel.CreateScope();
            Assert.Null(kernel.Get<AmbientScopeManager>().Current);

            IServiceScope serviceScope2 = kernel.CreateScope();
            Assert.Null(kernel.Get<AmbientScopeManager>().Current);

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
            kernel.UseAmbientScopes();
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

    }
}
