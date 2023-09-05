using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Extensions.AmbientScopes.Tests.Mocks
{
    public class MyServiceB : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public MyServiceC MyServiceC { get; }

        public MyServiceB(MyServiceC myServiceC)
        {
            MyServiceC = myServiceC;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

    }
}
