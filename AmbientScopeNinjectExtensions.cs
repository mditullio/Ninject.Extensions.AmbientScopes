using Ninject.Extensions;
using Ninject.Syntax;

namespace Ninject.Extensions.AmbientScopes
{
    public static class AmbientScopeNinjectExtensions
    {

        public static IBindingNamedWithOrOnSyntax<T> InAmbientScope<T>(this IBindingInSyntax<T> bindingInSyntax)
        {
            return bindingInSyntax.InScope(ctx => ctx.Kernel.Get<AmbientScopeProvider>().Current);
        }

        public static void UseAmbientScopes(this IKernel kernel)
        {
            kernel.Bind<AmbientScopeProvider>().ToSelf().InSingletonScope();
        }

        public static AmbientScope BeginAmbientScope(this IKernel kernel)
        {
            return kernel.Get<AmbientScopeProvider>().BeginScope();
        }

    }
}
