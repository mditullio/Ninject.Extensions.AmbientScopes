using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Extensions.AmbientScopes.Tests.Mocks
{

    /// <summary>
    /// Mock service which throws <see cref="InvalidOperationException"/> on constructor.
    /// </summary>
    public class MyBrokenService
    {

        public MyBrokenService() {
            throw new InvalidOperationException();
        }

    }
}
