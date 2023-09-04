namespace Ninject.Extensions.AmbientScopes.Tests
{
    public class AmbientScopeUnitTests
    {

        protected readonly AmbientScopeProvider _provider = new AmbientScopeProvider();

        public class WhenBeginningScope : AmbientScopeUnitTests
        {

            [Fact]
            public async Task NewScopeBecomesCurrent()
            {
                // Act
                using var newScope = _provider.BeginScope();
                await Task.Yield();

                // Assert
                Assert.Equal(newScope, _provider.Current);
            }

            [Fact]
            public async Task NestedScopeBecomesCurrent()
            {
                // Arrange
                using var outerScope = _provider.BeginScope();
                await Task.Yield();

                // Act
                using var innerScope = _provider.BeginScope();
                await Task.Yield();

                // Assert
                Assert.Equal(innerScope, _provider.Current);
            }
        }

        public class WhenResettingScope : AmbientScopeUnitTests
        {

            [Fact]
            public async Task CurrentIsResetToOuterScope()
            {
                AmbientScope previousScope;

                // Creating outer scope
                var outerScope = _provider.BeginScope();
                Assert.Equal(outerScope, _provider.Current);
                await Task.Yield();

                // Creating inner scope
                var innerScope = _provider.BeginScope();
                Assert.Equal(innerScope, _provider.Current);
                await Task.Yield();

                // Manually resetting to outer scope
                previousScope = _provider.ResetScope(outerScope);
                Assert.Equal(previousScope, innerScope);
                Assert.Equal(outerScope, _provider.Current);

                // Disposing inner scope does not alter current scope
                innerScope.Dispose();
                Assert.False(outerScope.IsDisposed);
                Assert.True(innerScope.IsDisposed);
                Assert.Equal(outerScope, _provider.Current);
            }

            [Fact]
            public async Task CurrentIsResetToNull()
            {
                AmbientScope previousScope;

                // Creating first scope
                var firstScope = _provider.BeginScope();
                Assert.Equal(firstScope, _provider.Current);
                await Task.Yield();

                // Resetting to null (no ambient scope)
                previousScope = _provider.ResetScope(null);
                Assert.Equal(previousScope, firstScope);
                Assert.Null(_provider.Current);

                // Creating second scope
                var secondScope = _provider.BeginScope();
                Assert.Equal(secondScope, _provider.Current);
                await Task.Yield();

                // Resetting to first scope
                previousScope = _provider.ResetScope(firstScope);
                Assert.Equal(previousScope, secondScope);
                Assert.Equal(firstScope, _provider.Current);

                // Disposing first scope should reset current scope to null
                firstScope.Dispose();
                Assert.True(firstScope.IsDisposed);
                Assert.False(secondScope.IsDisposed);
                Assert.Null(_provider.Current);
            }

        }

        public class WhenDisposingScope : AmbientScopeUnitTests
        {

            [Fact]
            public async Task OuterScopeResetsToNull()
            {
                // Arrange
                var outerScope = _provider.BeginScope();
                await Task.Yield();

                // Act
                outerScope.Dispose();

                // Assert
                Assert.True(outerScope.IsDisposed);
                Assert.Null(_provider.Current);
            }


            [Fact]
            public async Task WhenPreviousIsValidThenNestedScopeResetsToPrevious()
            {
                // Arrange
                using var outerScope = _provider.BeginScope();
                await Task.Yield();

                var innerScope = _provider.BeginScope();
                await Task.Yield();

                // Act
                innerScope.Dispose();
                await Task.Yield();

                // Assert
                Assert.True(innerScope.IsDisposed);
                Assert.Equal(outerScope, _provider.Current);
            }


            [Fact]
            public async Task WhenPreviousIsDisposedThenNestedScopeResetsToNull()
            {
                // Begin scopes
                using var outerScope = _provider.BeginScope();
                await Task.Yield();

                var innerScope = _provider.BeginScope();
                await Task.Yield();

                // Disposing outer scope should not reset current scope
                outerScope.Dispose();
                await Task.Yield();
                Assert.True(outerScope.IsDisposed);
                Assert.Equal(innerScope, _provider.Current);

                // Disposing inner scope should reset current scope to null
                innerScope.Dispose();
                await Task.Yield();
                Assert.True(innerScope.IsDisposed);
                Assert.Null(_provider.Current);
            }

            [Fact]
            public async Task ScopeIsNotDisposedTwice()
            {
                // Arrange
                var scope = _provider.BeginScope();
                var disposeCount = 0;
                scope.Disposed += (s, e) => disposeCount++;
                await Task.Yield();

                // Act
                scope.Dispose();
                await Task.Yield();

                scope.Dispose();
                await Task.Yield();

                // Assert
                Assert.Null(_provider.Current);
                Assert.True(scope.IsDisposed);
                Assert.Equal(1, disposeCount);
            }

        }

    }
}