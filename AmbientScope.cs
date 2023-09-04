using Ninject.Extensions;
using Ninject.Infrastructure.Disposal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ninject.Extensions.AmbientScopes
{
    public class AmbientScope : IDisposable, INotifyWhenDisposed
    {

        private AmbientScope _parent = null;

        public event EventHandler Disposed;

        public bool IsDisposed { get; private set; }

        internal AmbientScope Parent
        {
            get => _parent;
        }

        internal AmbientScope(AmbientScope parent)
        {
            _parent = parent;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

    }

}
