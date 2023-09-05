namespace Ninject.Extensions.AmbientScopes.Tests
{
    public class AmbientScopeUnitTests
    {

        public AmbientScopeManager ScopeManager { get; } = new AmbientScopeManager();

        public class WhenBeginningScope : AmbientScopeUnitTests
        {

            [Fact]
            public async Task NewScopeBecomesCurrent()
            {
                // Act
                using var newScope = ScopeManager.BeginScope();
                await Task.Yield();

                // Assert
                Assert.Equal(newScope, ScopeManager.Current);
            }

            [Fact]
            public async Task NestedScopeBecomesCurrent()
            {
                // Arrange
                using var outerScope = ScopeManager.BeginScope();
                await Task.Yield();

                // Act
                using var innerScope = ScopeManager.BeginScope();
                await Task.Yield();

                // Assert
                Assert.Equal(innerScope, ScopeManager.Current);
            }
        }

        public class WhenSettingCurrentScope : AmbientScopeUnitTests
        {

            [Fact]
            public async Task CurrentIsResetToOuterScope()
            {
                AmbientScope previousScope;

                // Creating outer scope
                var outerScope = ScopeManager.BeginScope();
                Assert.Equal(outerScope, ScopeManager.Current);
                await Task.Yield();

                // Creating inner scope
                var innerScope = ScopeManager.BeginScope();
                Assert.Equal(innerScope, ScopeManager.Current);
                await Task.Yield();

                // Manually resetting to outer scope
                previousScope = ScopeManager.SetCurrent(outerScope);
                Assert.Equal(previousScope, innerScope);
                Assert.Equal(outerScope, ScopeManager.Current);

                // Disposing inner scope does not alter current scope
                innerScope.Dispose();
                Assert.False(outerScope.IsDisposed);
                Assert.True(innerScope.IsDisposed);
                Assert.Equal(outerScope, ScopeManager.Current);
            }

            [Fact]
            public async Task CurrentIsResetToNull()
            {
                AmbientScope previousScope;

                // Creating first scope
                var firstScope = ScopeManager.BeginScope();
                Assert.Equal(firstScope, ScopeManager.Current);
                await Task.Yield();

                // Resetting to null (no ambient scope)
                previousScope = ScopeManager.SetCurrent(null);
                Assert.Equal(previousScope, firstScope);
                Assert.Null(ScopeManager.Current);

                // Creating second scope
                var secondScope = ScopeManager.BeginScope();
                Assert.Equal(secondScope, ScopeManager.Current);
                await Task.Yield();

                // Resetting to first scope
                previousScope = ScopeManager.SetCurrent(firstScope);
                Assert.Equal(previousScope, secondScope);
                Assert.Equal(firstScope, ScopeManager.Current);

                // Disposing first scope should reset current scope to null
                firstScope.Dispose();
                Assert.True(firstScope.IsDisposed);
                Assert.False(secondScope.IsDisposed);
                Assert.Null(ScopeManager.Current);
            }

        }

        public class WhenDisposingScope : AmbientScopeUnitTests
        {

            [Fact]
            public async Task OuterScopeResetsToNull()
            {
                // Arrange
                var outerScope = ScopeManager.BeginScope();
                await Task.Yield();

                // Act
                outerScope.Dispose();

                // Assert
                Assert.True(outerScope.IsDisposed);
                Assert.Null(ScopeManager.Current);
            }


            [Fact]
            public async Task NestedScopeResetsToPreviousWhenPreviousIsValid()
            {
                // Arrange
                using var outerScope = ScopeManager.BeginScope();
                await Task.Yield();

                var innerScope = ScopeManager.BeginScope();
                await Task.Yield();

                // Act
                innerScope.Dispose();
                await Task.Yield();

                // Assert
                Assert.True(innerScope.IsDisposed);
                Assert.Equal(outerScope, ScopeManager.Current);
            }


            [Fact]
            public async Task ThenNestedScopeResetsToNullWhenPreviousIsDisposed()
            {
                // Begin scopes
                using var outerScope = ScopeManager.BeginScope();
                await Task.Yield();

                var innerScope = ScopeManager.BeginScope();
                await Task.Yield();

                // Disposing outer scope should not reset current scope
                outerScope.Dispose();
                await Task.Yield();
                Assert.True(outerScope.IsDisposed);
                Assert.Equal(innerScope, ScopeManager.Current);

                // Disposing inner scope should reset current scope to null
                innerScope.Dispose();
                await Task.Yield();
                Assert.True(innerScope.IsDisposed);
                Assert.Null(ScopeManager.Current);
            }

            [Fact]
            public async Task ScopeIsNotDisposedTwice()
            {
                // Arrange
                var scope = ScopeManager.BeginScope();
                var disposeCount = 0;
                scope.Disposed += (s, e) => disposeCount++;
                await Task.Yield();

                // Act
                scope.Dispose();
                await Task.Yield();

                scope.Dispose();
                await Task.Yield();

                // Assert
                Assert.Null(ScopeManager.Current);
                Assert.True(scope.IsDisposed);
                Assert.Equal(1, disposeCount);
            }

        }

    }
}