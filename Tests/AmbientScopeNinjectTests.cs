using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Extensions.AmbientScopes.Tests
{
    public class AmbientScopeNinjectTests
    {

        public class MyService : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public IKernel Kernel { get; private set; }

            public MyService(IKernel kernel)
            {
                Kernel = kernel;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [Fact]
        public void ConstantValuesAreSameAcrossScopes()
        {
            var kernel = new StandardKernel();
            var myConstantValue = new MyService(kernel);
            kernel.UseAmbientScopes();
            kernel.Bind<MyService>().ToConstant(myConstantValue).InTransientScope();

            MyService firstInstance;
            MyService secondInstance;

            firstInstance = kernel.Get<MyService>();

            using (kernel.BeginAmbientScope())
            {
                secondInstance = kernel.Get<MyService>();
            }

            Assert.Same(firstInstance, secondInstance);
            Assert.False(kernel.IsDisposed);
            Assert.False(firstInstance.IsDisposed);
        }

        [Fact]
        public void SingletonObjectsAreSameAcrossScopes()
        {
            var kernel = new StandardKernel();
            kernel.UseAmbientScopes();
            kernel.Bind<MyService>().ToSelf().InSingletonScope();

            MyService firstInstance;
            MyService secondInstance;

            firstInstance = kernel.Get<MyService>();

            using (kernel.BeginAmbientScope())
            {
                secondInstance = kernel.Get<MyService>();
            }

            Assert.Same(firstInstance, secondInstance);
            Assert.False(kernel.IsDisposed);
            Assert.False(firstInstance.IsDisposed);
        }

        [Fact]
        public void AmbientObjectsAreSameWithinSameScope()
        {
            var kernel = new StandardKernel();
            kernel.UseAmbientScopes();
            kernel.Bind<MyService>().ToSelf().InAmbientScope();

            MyService firstInstance;
            MyService secondInstance;

            using (kernel.BeginAmbientScope())
            {
                firstInstance = kernel.Get<MyService>();
                secondInstance = kernel.Get<MyService>();
            }

            Assert.Same(firstInstance, secondInstance);
            Assert.False(kernel.IsDisposed);
            Assert.True(firstInstance.IsDisposed);
        }

        [Fact]
        public void AmbientObjectsAreDifferentWithoutScope()
        {
            var kernel = new StandardKernel();
            kernel.UseAmbientScopes();
            kernel.Bind<MyService>().ToSelf().InAmbientScope();

            Assert.Null(kernel.Get<AmbientScopeProvider>().Current);
            var firstInstance = kernel.Get<MyService>();
            var secondInstance = kernel.Get<MyService>();

            Assert.NotSame(firstInstance, secondInstance);
            Assert.False(kernel.IsDisposed);
            Assert.False(firstInstance.IsDisposed);
            Assert.False(secondInstance.IsDisposed);
        }

        [Fact]
        public void AmbientObjectsAreDifferentAcrossScopes()
        {
            var kernel = new StandardKernel();
            kernel.UseAmbientScopes();
            kernel.Bind<MyService>().ToSelf().InAmbientScope();

            MyService firstInstance;
            MyService secondInstance;

            using (kernel.BeginAmbientScope())
            {
                firstInstance = kernel.Get<MyService>();
            }

            using (kernel.BeginAmbientScope())
            {
                secondInstance = kernel.Get<MyService>();
            }

            Assert.NotSame(firstInstance, secondInstance);
            Assert.False(kernel.IsDisposed);
            Assert.True(firstInstance.IsDisposed);
            Assert.True(secondInstance.IsDisposed);
        }

    }
}
