namespace Ninject.Extensions.AmbientScopes.Tests.Mocks
{
    public class MyServiceA : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public IKernel Kernel { get; }
        public MyServiceB MyServiceB { get; }
        public MyServiceC MyServiceC { get; }

        public MyServiceA(IKernel kernel, MyServiceB myServiceB, MyServiceC myServiceC)
        {
            Kernel = kernel;
            MyServiceB = myServiceB;
            MyServiceC = myServiceC;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
