using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Ninject.Extensions.AmbientScopes
{

    /// <summary>
    /// Provides management for ambient scopes, allowing objects to be tracked within specific lifecycles.
    /// </summary>
    public class AmbientScopeManager
    {

        private readonly object _lock = new object();

        private readonly AsyncLocal<AmbientScope> _current = new AsyncLocal<AmbientScope>();

        /// <summary>
        /// Gets the current ambient scope within the context of the executing code.
        /// </summary>
        public AmbientScope Current
        {
            get => _current.Value;
            set => SetCurrent(value);
        }

        /// <summary>
        /// Begins a new ambient scope, making it the current scope.
        /// </summary>
        public AmbientScope BeginScope()
        {
            var newScope = new AmbientScope(_current.Value);
            SetCurrent(newScope);
            return newScope;
        }

        /// <summary>
        /// Sets an existing ambient scope as the current scope.
        /// </summary>
        /// <returns>The previous ambient scope</returns>
        public AmbientScope SetCurrent(AmbientScope existingScope)
        {
            lock (_lock)
            {
                var oldScope = _current.Value;
                _current.Value = existingScope;
                if (existingScope != null)
                {
                    existingScope.Disposed -= OnAmbientScopeDisposed;
                    existingScope.Disposed += OnAmbientScopeDisposed;
                }
                return oldScope;
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
