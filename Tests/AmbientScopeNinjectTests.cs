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
            kernel.LoadAmbientScopeModule();

            var myServiceC = new MyServiceC();
            var myServiceB = new MyServiceB(myServiceC);
            var myServiceA = new MyServiceA(kernel, myServiceB, myServiceC);

            kernel.Bind<MyServiceA>().ToConstant(myServiceA).InTransientScope();

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
            kernel.LoadAmbientScopeModule();
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
            kernel.LoadAmbientScopeModule();
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceB>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceC>().ToSelf().InSingletonScope();

            MyServiceA firstInstance;
            MyServiceA secondInstance;
            MyServiceA thirdInstance;

            using (kernel.BeginAmbientScope())
            {
                firstInstance = kernel.Get<MyServiceA>();
                secondInstance = kernel.Get<MyServiceA>();
                thirdInstance = firstInstance.Kernel.Get<MyServiceA>();
            }

            Assert.Same(firstInstance, secondInstance);
            Assert.Same(firstInstance, thirdInstance);
            Assert.Same(firstInstance.MyServiceB, secondInstance.MyServiceB);
            Assert.Same(firstInstance.MyServiceC, firstInstance.MyServiceB.MyServiceC);
            Assert.True(firstInstance.IsDisposed);
            Assert.True(firstInstance.MyServiceB.IsDisposed);
            Assert.False(firstInstance.MyServiceC.IsDisposed);
            Assert.False(kernel.IsDisposed);
        }

        [Fact]
        public void AmbientObjectsAreDifferentWithoutScope()
        {
            var kernel = new StandardKernel();
            kernel.LoadAmbientScopeModule();
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceB>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceC>().ToSelf().InAmbientScope();

            Assert.Null(kernel.Get<AmbientScopeManager>().Current);
            var firstInstance = kernel.Get<MyServiceA>();
            var secondInstance = kernel.Get<MyServiceA>();

            Assert.NotSame(firstInstance, secondInstance);
            Assert.NotSame(firstInstance.MyServiceC, firstInstance.MyServiceB.MyServiceC);
            Assert.False(kernel.IsDisposed);
            Assert.False(firstInstance.IsDisposed);
            Assert.False(secondInstance.IsDisposed);
        }

        [Fact]
        public void AmbientObjectsAreDifferentAcrossScopes()
        {
            var kernel = new StandardKernel();
            kernel.LoadAmbientScopeModule();
            kernel.Bind<MyServiceA>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceB>().ToSelf().InAmbientScope();
            kernel.Bind<MyServiceC>().ToSelf().InSingletonScope();

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
            Assert.NotSame(firstInstance.MyServiceB, secondInstance.MyServiceB);
            Assert.Same(firstInstance.MyServiceC, firstInstance.MyServiceB.MyServiceC);
            Assert.Same(firstInstance.MyServiceB.MyServiceC, secondInstance.MyServiceB.MyServiceC);
            Assert.True(firstInstance.IsDisposed);
            Assert.True(secondInstance.IsDisposed);
            Assert.False(firstInstance.MyServiceC.IsDisposed);
            Assert.False(kernel.IsDisposed);
        }

    }
}
