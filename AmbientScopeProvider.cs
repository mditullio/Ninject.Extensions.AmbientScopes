using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Ninject.Extensions.AmbientScopes
{
    public class AmbientScopeProvider
    {

        private readonly object _lock = new object();

        private readonly AsyncLocal<AmbientScope> _current = new AsyncLocal<AmbientScope>();

        public AmbientScope Current
        {
            get => _current.Value;
        }

        public AmbientScope BeginScope()
        {
            lock (_lock)
            {
                var newScope = new AmbientScope(_current.Value);
                newScope.Disposed += OnAmbientScopeDisposed;
                _current.Value = newScope;
                return newScope;
            }
        }

        private void OnAmbientScopeDisposed(object sender, EventArgs args)
        {
            lock (_lock)
            {
                if (sender == _current.Value)
                {
                    _current.Value = GetValidAncestor(_current.Value);
                }
            }
        }

        private static AmbientScope GetValidAncestor(AmbientScope disposedScope)
        {
            AmbientScope validAncestor = disposedScope.Parent;            
            while (validAncestor != null && validAncestor.IsDisposed)
            {
                validAncestor = validAncestor.Parent;
            }
            return validAncestor;
        }

    }
}
