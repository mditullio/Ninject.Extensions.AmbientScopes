using Ninject.Extensions.AmbientScopes.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Extensions.AmbientScopes.Tests
{
    public partial class AmbientScopeNinjectTests
    {

        [Fact]
        public void ConstantValuesAreSameAcrossScopes()
        {
            var kernel = new StandardKernel();
            var myConstantValue = new MyServiceA(kernel);
            kernel.UseAmbientScopes();
            kernel.Bind<MyServiceA>().ToConstant(myConstantValue).InTransientScope();

            MyServiceA firstInstance;
            MyServiceA secondInstance;

            firstInstance = kernel.Get<MyServiceA>();

            using (kernel.BeginAmbientScope())
            {
                secondInstance = kernel.Get<MyServiceA>();
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
            kernel.Bind<MyServiceA>().ToSelf().InSingletonScope();

            MyServiceA firstInstance;
            MyServiceA secondInstance;

            firstInstance = kernel.Get<MyServiceA>();

            using (kernel.BeginAmbientScope())
            {
                secondInstance = kernel.Get<MyServiceA>();
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
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();

            MyServiceA firstInstance;
            MyServiceA secondInstance;

            using (kernel.BeginAmbientScope())
            {
                firstInstance = kernel.Get<MyServiceA>();
                secondInstance = kernel.Get<MyServiceA>();
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
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();

            Assert.Null(kernel.Get<AmbientScopeProvider>().Current);
            var firstInstance = kernel.Get<MyServiceA>();
            var secondInstance = kernel.Get<MyServiceA>();

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
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();

            MyServiceA firstInstance;
            MyServiceA secondInstance;

            using (kernel.BeginAmbientScope())
            {
                firstInstance = kernel.Get<MyServiceA>();
            }

            using (kernel.BeginAmbientScope())
            {
                secondInstance = kernel.Get<MyServiceA>();
            }

            Assert.NotSame(firstInstance, secondInstance);
            Assert.False(kernel.IsDisposed);
            Assert.True(firstInstance.IsDisposed);
            Assert.True(secondInstance.IsDisposed);
        }

    }
}
