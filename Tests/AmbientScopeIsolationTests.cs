namespace Ninject.Extensions.AmbientScopes.Tests
{

    public class AmbientScopeIsolationTests
    {

        private readonly AmbientScopeProvider _provider = new AmbientScopeProvider();

        [Fact]
        public void NewThreadsInheritCurrentScope()
        {
            using (AmbientScope parentScope = _provider.BeginScope())
            {
                AmbientScope currentScopeInThread1 = null;
                AmbientScope currentScopeInThread2 = null;

                // Create a new thread
                Thread newThread1 = new Thread(() =>
                {
                    // Capture current scope
                    currentScopeInThread1 = _provider.Current;
                });

                newThread1.Start();
                newThread1.Join();

                // Create a new thread
                Thread newThread2 = new Thread(() =>
                {
                    // Capture current scope
                    currentScopeInThread2 = _provider.Current;
                });

                newThread2.Start();
                newThread2.Join();

                // Ensure scopes are same
                Assert.Equal(_provider.Current, parentScope);
                Assert.Equal(_provider.Current, currentScopeInThread1);
                Assert.Equal(_provider.Current, currentScopeInThread2);
            }

            // Ensure the parent scope is now null
            Assert.Null(_provider.Current);
        }

        [Fact]
        public void NewThreadsCreateNestedScopeInIsolation()
        {
            using (var parentScope = _provider.BeginScope())
            {
                AmbientScope currentScopeInThread1 = null;
                AmbientScope currentScopeInThread2 = null;

                // Create a new threads
                Thread newThread1 = new Thread(() =>
                {
                    // Create nested scope without disposing it
                    var nestedScope = _provider.BeginScope();
                    currentScopeInThread1 = _provider.Current;
                });

                newThread1.Start();
                newThread1.Join();

                Thread newThread2 = new Thread(() =>
                {
                    // Create nested scope without disposing it
                    var nestedScope = _provider.BeginScope();
                    currentScopeInThread2 = _provider.Current;
                });

                newThread2.Start();
                newThread2.Join();

                // Ensure current scope is still the parent scope
                Assert.NotEqual(_provider.Current, currentScopeInThread1);
                Assert.NotEqual(_provider.Current, currentScopeInThread2);
                Assert.Equal(_provider.Current, parentScope);
                Assert.False(currentScopeInThread1.IsDisposed);
                Assert.False(currentScopeInThread2.IsDisposed);
            }

            // Ensure the parent scope is now null
            Assert.Null(_provider.Current);
        }

        [Fact]
        public async Task NewTasksInheritCurrentScope()
        {
            using (AmbientScope parentScope = _provider.BeginScope())
            {
                AmbientScope currentScopeInTask1 = null;
                AmbientScope currentScopeInTask2 = null;

                // Create a child task
                await Task.Run(() =>
                {
                    // Capture current scope
                    currentScopeInTask1 = _provider.Current;
                });

                // Create a child task
                await Task.Run(() =>
                {
                    // Capture current scope
                    currentScopeInTask2 = _provider.Current;
                });

                // Ensure scopes are same
                Assert.Equal(_provider.Current, parentScope);
                Assert.Equal(_provider.Current, currentScopeInTask1);
                Assert.Equal(_provider.Current, currentScopeInTask2);
            }

            // Ensure the parent scope is now null
            Assert.Null(_provider.Current);
        }

        [Fact]
        public async Task NewTasksCreateNestedScopesInIsolation()
        {
            using (var parentScope = _provider.BeginScope())
            {
                AmbientScope currentScopeInTask1 = null;
                AmbientScope currentScopeInTask2 = null;

                // Create a child task
                await Task.Run(() =>
                {
                    // Create nested scope without disposing it
                    var nestedScope = _provider.BeginScope();
                    currentScopeInTask1 = _provider.Current;
                });

                // Create a child task
                await Task.Run(() =>
                {
                    // Create nested scope without disposing it
                    var nestedScope = _provider.BeginScope();
                    currentScopeInTask2 = _provider.Current;
                });

                // Ensure parent scope is still the original scope
                Assert.NotEqual(_provider.Current, currentScopeInTask1);
                Assert.NotEqual(_provider.Current, currentScopeInTask2);
                Assert.Equal(_provider.Current, parentScope);
                Assert.False(currentScopeInTask1.IsDisposed);
                Assert.False(currentScopeInTask2.IsDisposed);
            }

            // Ensure the parent scope is now null
            Assert.Null(_provider.Current);
        }
    }

}
