namespace Ninject.Extensions.AmbientScopes.Tests.Mocks
{
    public class MyServiceA : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public IKernel Kernel { get; private set; }

        public MyServiceA(IKernel kernel)
        {
            Kernel = kernel;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
