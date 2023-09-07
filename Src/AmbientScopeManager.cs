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

        /// <summary>
        /// Executes a provided function within a specified ambient scope, ensuring proper scope management.
        /// <para>
        /// This method sets the specified <paramref name="ambientScope"/> as the current ambient scope,
        /// allowing the provided function to execute within that scope. After the function execution,
        /// the method ensures that the previous ambient scope (if any) is correctly restored.
        /// </para>
        /// </summary>
        public T ExecuteInScope<T>(AmbientScope ambientScope, Func<T> func)
        {
            var previousScope = SetCurrent(ambientScope);
            try
            {
                return func();
            }
            finally
            {
                SetCurrent(previousScope);
            }
        }

        /// <summary>
        /// Executes a provided action within a specified ambient scope, ensuring proper scope management.
        /// <para>
        /// This method sets the specified <paramref name="ambientScope"/> as the current ambient scope,
        /// allowing the provided action to execute within that scope. After the function execution,
        /// the method ensures that the previous ambient scope (if any) is correctly restored.
        /// </para>
        /// </summary>
        public void ExecuteInScope(AmbientScope ambientScope, Action action)
        {
            var previousScope = SetCurrent(ambientScope);
            try
            {
                action();
            }
            finally
            {
                SetCurrent(previousScope);
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
