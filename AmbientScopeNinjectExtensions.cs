using Ninject.Extensions;
using Ninject.Syntax;
using System;

namespace Ninject.Extensions.AmbientScopes
{
    public static class AmbientScopeNinjectExtensions
    {

        /// <summary>
        /// Binds a type to an ambient scope. 
        /// Instances of the bound type will be created and managed within the current ambient scope.
        /// </summary>
        public static IBindingNamedWithOrOnSyntax<T> InAmbientScope<T>(this IBindingInSyntax<T> bindingInSyntax)
        {
            if (bindingInSyntax is null)
            {
                throw new ArgumentNullException(nameof(bindingInSyntax));
            }

            return bindingInSyntax.InScope(ctx => ctx.Kernel.Get<AmbientScopeProvider>().Current);
        }

        /// <summary>
        /// Sets up the ambient scope infrastructure within the Ninject kernel.
        /// </summary>
        public static void UseAmbientScopes(this IKernel kernel)
        {
            if (kernel is null)
            {
                throw new ArgumentNullException(nameof(kernel));
            }

            kernel.Bind<AmbientScopeProvider>().ToSelf().InSingletonScope();
        }

        /// <summary>
        /// Creates and begins a new ambient scope.
        /// </summary>
        public static AmbientScope BeginAmbientScope(this IKernel kernel)
        {
            if (kernel is null)
            {
                throw new ArgumentNullException(nameof(kernel));
            }

            return kernel.Get<AmbientScopeProvider>().BeginScope();
        }

    }
}
