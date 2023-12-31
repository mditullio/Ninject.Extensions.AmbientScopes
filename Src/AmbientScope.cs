﻿using Ninject.Extensions;
using Ninject.Infrastructure.Disposal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ninject.Extensions.AmbientScopes
{
    /// <summary>
    /// The ambient scope provides deterministic disposal of objects, ensuring proper cleanup and resource management.
    /// </summary>
    public class AmbientScope : IDisposable, INotifyWhenDisposed
    {

        private AmbientScope _parent = null;

        public event EventHandler Disposed;

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// For nested ambient scopes, this reference to the previous scope.
        /// For the outer scope, this value is null.
        /// </summary>
        public AmbientScope Parent
        {
            get => _parent;
        }

        /// <summary>
        /// Creates an ambient scope, optionally with a parent scope.
        /// </summary>
        public AmbientScope(AmbientScope parent = null)
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
