using Microsoft.Extensions.DependencyInjection;
using Ninject.Activation;
using Ninject.Extensions;
using Ninject.Syntax;
using Ninject.Web.Common;
using System;
using System.Linq;

namespace Ninject.Extensions.AmbientScopes
{
    public static class AmbientScopeNinjectExtensions
    {

        /// <summary>
        /// Binds a type to an ambient scope. 
        /// Instances of the bound type will be created and managed within the current ambient scope.
        /// <para>
        /// If no ambient scope is defined, instances activated via this binding are transient.
        /// </para>
        /// </summary>
        public static IBindingNamedWithOrOnSyntax<T> InAmbientScope<T>(this IBindingInSyntax<T> bindingInSyntax)
        {
            if (bindingInSyntax is null)
            {
                throw new ArgumentNullException(nameof(bindingInSyntax));
            }

            return bindingInSyntax.InScope(GetAmbientScope);
        }

        /// <summary>
        /// Binds a type to an ambient scope. 
        /// Instances of the bound type will be created and managed within the current ambient scope.
        /// <para>
        /// If no ambient scope is defined, instances activated via this binding are in the request scope.
        /// </para>
        /// </summary>
        public static IBindingNamedWithOrOnSyntax<T> InAmbientOrRequestScope<T>(this IBindingInSyntax<T> bindingInSyntax)
        {
            if (bindingInSyntax is null)
            {
                throw new ArgumentNullException(nameof(bindingInSyntax));
            }

            return bindingInSyntax.InScope(GetAmbientOrRequestScope);
        }

        /// <summary>
        /// Sets up the ambient scope infrastructure within the Ninject kernel.
        /// </summary>
        public static void LoadAmbientScopeModule(this IKernel kernel)
        {
            if (kernel is null)
            {
                throw new ArgumentNullException(nameof(kernel));
            }
            if (!kernel.HasModule(AmbientScopeNinjectModule.MODULE_NAME))
            {
                kernel.Load<AmbientScopeNinjectModule>();
            }
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

            return kernel.Get<AmbientScopeManager>().BeginScope();
        }

        private static object GetAmbientOrRequestScope(IContext ctx)
        {
            return GetAmbientScope(ctx) ?? GetRequestScope(ctx);
        }

        private static object GetAmbientScope(IContext ctx)
        {
            return ctx.Kernel.Get<AmbientScopeManager>().Current;
        }

        private static object GetRequestScope(IContext ctx)
        {
            return ctx.Kernel.Components.GetAll<INinjectHttpApplicationPlugin>()
                .Select(c => c.GetRequestScope(ctx))
                .FirstOrDefault((object s) => s != null);
        }

    }
}
